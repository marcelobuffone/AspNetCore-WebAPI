using MBAM.Annotations.Domain.Core.Events;

namespace MBAM.Annotations.Domain.Interfaces
{
    public interface IEventStore
    {
        void SaveEvent<T>(T evento) where T : Event;
    }
}
