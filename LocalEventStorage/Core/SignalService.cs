using System.Collections.Generic;
using EventsManager.LocalEventStorage.Abstractions;

namespace EventsManager.LocalEventStorage.Core
{
    public class SignalService : BaseService<IUnitOfWork>, ISignalService
    {
        private ISignalRepository _signalRepository;

        protected ISignalRepository SignalRepository
        {
            get
            {
                if (_signalRepository == null)
                    _signalRepository = new SignalRepository((CacheContext)UnitOfWork?.GetContext());
                
                return _signalRepository;
            }
        }

        public SignalService(IUnitOfWork uow)
            : base(uow)
        { }

        public void Add(IEnumerable<Signal> signals)
        {
            SignalRepository.Add(signals);
        }

        public void Truncate()
        {
            SignalRepository.Truncate();
        }

        public int Count()
        {
            return SignalRepository.Count();
        }

        public IEnumerable<Signal> GetAll()
        {
            return SignalRepository.GetAll();
        }

        public void Dispose()
        {
            _uow.Dispose();
        }
    }
}
