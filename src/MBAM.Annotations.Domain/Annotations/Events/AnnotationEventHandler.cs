using MBAM.Annotations.Domain.Core.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace MBAM.Annotations.Domain.Annotations.Events
{
    public class AnnotationEventHandler :
                INotificationHandler<AnnotationRegistredEvent>,
                INotificationHandler<AnnotationUpdatedEvent>,
                INotificationHandler<AnnotationDeletedEvent>
    {

        public Task Handle(AnnotationUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Handle(AnnotationDeletedEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Handle(AnnotationRegistredEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
