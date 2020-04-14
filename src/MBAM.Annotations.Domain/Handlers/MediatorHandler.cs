using System.Threading.Tasks;
using MBAM.Annotations.Domain.Core.Commands;
using MBAM.Annotations.Domain.Core.Events;
using MBAM.Annotations.Domain.Interfaces;
using MediatR;

namespace MBAM.Annotations.Domain.Handlers
{
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;
        private readonly IEventStore _eventStore;

        public MediatorHandler(IMediator mediator
            , IEventStore eventStore
            )
        {
            _mediator = mediator;
            _eventStore = eventStore;
        }

        public async Task SendCommand<T>(T comando) where T : Command
        {
            await _mediator.Send(comando);
        }

        public async Task RaiseEvent<T>(T evento) where T : Event
        {
            if (!evento.MessageType.Equals("DomainNotification"))
                _eventStore?.SaveEvent(evento);

            await _mediator.Publish(evento);
        }
    }
}
