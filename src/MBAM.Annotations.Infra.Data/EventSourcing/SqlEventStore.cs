using MBAM.Annotations.Domain.Core.Events;
using MBAM.Annotations.Domain.Interfaces;
using MBAM.Annotations.Infra.Data.Repository.EventSourcing;
using Newtonsoft.Json;

namespace MBAM.Annotations.Infra.Data.EventSourcing
{
    public class SqlEventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;

        public SqlEventStore(IEventStoreRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
        }

        public void SaveEvent<T>(T evento) where T : Event
        {
            var serializedData = JsonConvert.SerializeObject(evento);

            var storedEvent = new StoredEvent(
                evento,
                serializedData);

            _eventStoreRepository.Store(storedEvent);
        }
    }
}
