using System;
using System.Collections.Generic;
using System.Linq;
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

                return this._signalRepository;
            }
        }

        public SignalService(IUnitOfWork uow)
            : base(uow)
        {

        }

        public void Add(IEnumerable<Signal> signals)
        {
            this.SignalRepository.Add(signals);
        }

        public void Truncate()
        {
            this.SignalRepository.Truncate();
        }

        public int Count()
        {
            return this.SignalRepository.Count();
        }

        public IEnumerable<Signal> GetAll()
        {
            return this.SignalRepository.GetAll();
        }
    }
}
