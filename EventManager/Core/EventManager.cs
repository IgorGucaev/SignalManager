using EventsGateway.Common;
using EventsManager.Abstractions;
using EventsManager.CloudEventHub.Abstractions;
using EventsManager.CloudEventHub.Gateway.Utils;
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
        private ILogger _Logger;

        public EventManager(ISignalService service, ICloudAdapter cloudAdapter, ILogger logger)
        {
            _Service = service;
            _CloudAdapter = cloudAdapter;
            _Logger = logger;

            if (!Helpers.CheckForInternetConnection())
                throw new Exception("No internet connection!");
        }

        public void Start()
        {
            Task pushEventsToLocalStorageTask = Task.Factory.StartNew(() =>
            {
                List<Signal> extract = new List<Signal>();

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

                    Signal signal;

                    try
                    {
                        Thread.Sleep(20);

                        extract.Clear();
                        

                        while (_EventCache.TryTake(out signal))
                            extract.Add(signal);

                        _Service.Add(extract);
                    }
                    catch (Exception ex)
                    {
                        _Logger.LogError(ex.Message);
                    }
                    finally { movingToLocal = false; }

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

                    try
                    {
                        IEnumerable<Signal> events = _Service.GetAll();

                        foreach (var signal in events)
                            _CloudAdapter.Enqueue(signal.DeviceId, signal.Data);

                        _Service.Truncate();
                    }
                    catch (Exception ex)
                    {
                        _Logger.LogError(ex.Message);
                    }
                    finally
                    {
                        movingToCloud = false;
                    }
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

        public void Dispose()
        {
            _Service.Dispose();
            _CloudAdapter.Dispose();
        }
    }
}