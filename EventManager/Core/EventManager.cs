using EventsManager.Abstractions;
using EventsManager.CloudEventHub.Abstractions;
using EventsManager.LocalEventStorage.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventsManager.Core
{
    public class EventManager : IEventManager
    {
        public int IntervalMemToLocal { get; private set; } = 5;
        public int IntervalLocalToCloud { get; private set; } = 5;
        private ConcurrentBag<Signal> _EventCache = new ConcurrentBag<Signal>();
        private ConcurrentBag<Signal> _EventCacheTemp = new ConcurrentBag<Signal>();
        private bool movingToLocal = false;
        private bool movingToCloud = false;

        private ISignalService _Service;
        private CancellationTokenSource _TokenSource = new CancellationTokenSource();
        private ICloudAdapter _CloudAdapter;

        public EventManager(ISignalService service, ICloudAdapter cloudAdapter)
        {
            _Service = service;
            _CloudAdapter = cloudAdapter;
        }

        public void Start()
        {
            Task pushEventsToLocalStorageTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_TokenSource.IsCancellationRequested)
                        break;

                    if (movingToLocal || movingToCloud) // Must wait when any data transfer
                    {
                        Thread.Sleep(20);
                        continue;
                    }

                    movingToLocal = true;

                    Thread.Sleep(20);

                    List<Signal> extract = new List<Signal>();
                    Signal signal;

                    while (_EventCache.TryTake(out signal))
                        extract.Add(signal);

                    this._Service.Add(extract);

                    movingToLocal = false;

                    while (_EventCacheTemp.TryTake(out signal))
                        _EventCache.Add(signal);

                    Thread.Sleep(IntervalMemToLocal * 1000);
                }
            }, _TokenSource.Token);

            Task transferEventsToCloudStorageTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(IntervalLocalToCloud * 1000);
                    int attempt = 0;

                    while (movingToLocal || movingToCloud)
                    {
                        attempt++;
                        if (attempt > 100) // Do task anyway 
                            break;
                        Thread.Sleep(100);
                    }

                    movingToCloud = true;

                    IEnumerable<Signal> events = this._Service.GetAll();

                    foreach (var signal in events)
                        _CloudAdapter.Enqueue(signal.Data);

                    this._Service.Truncate();

                    movingToCloud = false;
                }
            }, _TokenSource.Token);
        }

        public void Stop()
        {
            _TokenSource.Cancel();
        }

        public void RegisterEvent(Signal e)
        {
            if (movingToLocal)
                _EventCacheTemp.Add(e);
            else _EventCache.Add(e);
        }
    }
}