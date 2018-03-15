namespace EventsGateway.Common.Threading
{
    using System;
    using System.Threading;

    using _THREADING = System.Threading.Tasks;

    /* Task wrappers. Do separate tasks in thread pool */

    public class TaskWrapper
    {
        private static int _unique_id = 0;

        private readonly int _id;
        private _THREADING.TaskStatus _status;
        private ManualResetEvent _completed;


        protected readonly Action _action;

        public static TaskWrapper Run(Action action)
        {
            var t = new TaskWrapper(action);

            t.Start();

            return t;
        }

        public static void BatchWaitAll(params TaskWrapper[] tasks)
        {
            // we can wait on 64 handles at the most            
            const int maxHandles = 64;

            int remainder = tasks.Length % maxHandles;
            int loops = tasks.Length / maxHandles;

            WaitHandle[] wh = null;
            if (tasks.Length > maxHandles)
            {

                wh = new WaitHandle[maxHandles];

                for (int i = 0; i < loops; ++i)
                {
                    for (int j = 0; j < maxHandles; ++j)
                    {
                        wh[j] = tasks[(i * maxHandles) + j]._completed;
                    }

                    AutoResetEvent.WaitAll(wh, Timeout.Infinite);
                }
            }

            if (remainder > 0)
            {
                wh = new WaitHandle[remainder];

                for (int j = 0; j < remainder; ++j)
                {
                    wh[j] = tasks[(loops * maxHandles) + j]._completed;
                }

                AutoResetEvent.WaitAll(wh, Timeout.Infinite);
            }
        }

        protected TaskWrapper()
        {
            _id = Interlocked.Increment(ref _unique_id);
            _status = _THREADING.TaskStatus.Created;
            _completed = new ManualResetEvent(false);
        }

        protected TaskWrapper(Action action)
            : this()
        {
            _action = action;
        }

        public virtual void Start()
        {
            ThreadPool.QueueUserWorkItem(Execute);
        }

        public void Wait()
        {
            _completed.WaitOne();
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public _THREADING.TaskStatus Status
        {
            get
            {
                return _status;
            }
        }

        protected void SetStatus(_THREADING.TaskStatus status)
        {
            _status = status;
        }

        protected bool IsRunningOrDone()
        {
            return _status == _THREADING.TaskStatus.WaitingToRun ||
                   _status == _THREADING.TaskStatus.Running ||
                   _status == _THREADING.TaskStatus.Faulted ||
                   _status == _THREADING.TaskStatus.RanToCompletion;
        }

        protected void SetCompleted()
        {
            _completed.Set();
        }

        protected void WaitCompleted()
        {
            _completed.WaitOne();
        }

        private void Execute(object state)
        {
            _status = _THREADING.TaskStatus.Running;

            try
            {
                _action();
            }
            catch
            {
                _status = _THREADING.TaskStatus.Faulted;
            }

            _status = _THREADING.TaskStatus.RanToCompletion;

            _completed.Set();
        }
    }

    public class TaskWrapper<TResult> : TaskWrapper
    {
        private Func<TResult> _func;
        private object _cont;
        private TResult _result = default(TResult);
        private readonly object _syncRoot = new object();

        public static TaskWrapper<TResult> Run(Func<TResult> function)
        {
            var t = new TaskWrapper<TResult>(function);

            t.Start();

            return t;
        }

        private TaskWrapper(Func<TResult> func)
            : base()
        {
            _func = func;
        }

        public override void Start()
        {
            ThreadPool.QueueUserWorkItem(Execute);
        }

        private TaskWrapper<TOutput> MakeTask<TInput, TOutput>(Func<TaskWrapper<TResult>, TOutput> continuationFunction)
        {
            return new TaskWrapper<TOutput>(() =>
           {
               return continuationFunction(this);
           });
        }

        public TaskWrapper<TNewResult> ContinueWith<TNewResult>(Func<TaskWrapper<TResult>, TNewResult> continuationFunction)
        {
            _cont = MakeTask<TaskWrapper<TResult>, TNewResult>(continuationFunction);

            lock (_syncRoot)
            {
                if (IsRunningOrDone())
                {
                    // Task is executing or done, schedule againto 
                    // make sure continuation will be served
                    Start();
                }
            }

            return (TaskWrapper<TNewResult>)_cont;
        }

        public TResult Result
        {
            get
            {
                return _result;
            }
        }

        private void Execute(object state)
        {
            //
            // we want to execute _func only once 
            //
            Func<TResult> f = null;
            lock (_syncRoot)
            {
                if (_func != null)
                {
                    f = _func;

                    _func = null;

                    SetStatus(_THREADING.TaskStatus.WaitingToRun);
                }
            }

            if (f != null)
            {
                try
                {
                    SetStatus(_THREADING.TaskStatus.Running);

                    _result = f();
                }
                catch
                {
                    SetStatus(_THREADING.TaskStatus.Faulted);
                }

                SetStatus(_THREADING.TaskStatus.RanToCompletion);

                SetCompleted();
            }

            //
            // we want to execute _cont only once 
            //
            TaskWrapper cont = null;
            lock (_syncRoot)
            {
                if (_cont != null)
                {
                    cont = (TaskWrapper)_cont;

                    _cont = null;
                }
            }

            if (cont != null)
            {
                // do not start the continuation before the task is completed
                WaitCompleted();

                ((TaskWrapper)cont).Start();
            }
        }
    }
}