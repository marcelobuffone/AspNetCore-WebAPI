using MBAM.Annotations.Domain.Annotations.Events;
using MBAM.Annotations.Domain.Annotations.Repository;
using MBAM.Annotations.Domain.CommandHandlers;
using MBAM.Annotations.Domain.Core.Notifications;
using MBAM.Annotations.Domain.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MBAM.Annotations.Domain.Annotations.Commands
{
    public class AnnotationCommandHandler : CommandHandler,
        IRequestHandler<RegisterAnnotationCommand>,
        IRequestHandler<UpdateAnnotationCommand>,
        IRequestHandler<DeleteAnnotationCommand>
    {
        private readonly IAnnotationRepository _annotationRepository;
        private readonly IMediatorHandler _mediator;

        public AnnotationCommandHandler(IAnnotationRepository annotationRepository,
                                        IUnitOfWork uow,
                                        IMediatorHandler mediator,
                                        INotificationHandler<DomainNotification> notifications)
              : base(uow, mediator, notifications)
        {
            _annotationRepository = annotationRepository;
            _mediator = mediator;
        }

        public Task<Unit> Handle(RegisterAnnotationCommand request, CancellationToken cancellationToken)
        {
            var annotation = new Annotation(request.Title, request.Description);

            if (!AnnotationValid(annotation.AnnotationHistory.FirstOrDefault())) return Unit.Task;

            _annotationRepository.Add(annotation);

            if (Commit())
            {
                _mediator.RaiseEvent(new AnnotationRegistredEvent(annotation.Id));
            }
            return Unit.Task;
        }

        public Task<Unit> Handle(UpdateAnnotationCommand request, CancellationToken cancellationToken)
        {
            if (!AnnotationExisting(request.AggregateId, request.MessageType)) return Unit.Task;

            var history = new AnnotationHistory(request.Title, request.Description, request.AggregateId);

            if (!AnnotationValid(history)) return Unit.Task;

            _annotationRepository.AddHistory(history);

            if (Commit())
            {
                _mediator.RaiseEvent(new AnnotationUpdatedEvent(request.Title, request.Description, request.AggregateId));
            }
            return Unit.Task;
        }

        public Task<Unit> Handle(DeleteAnnotationCommand request, CancellationToken cancellationToken)
        {
            if (!AnnotationExisting(request.AggregateId, request.MessageType)) return Unit.Task;

            _annotationRepository.Remove(request.Id);

            if (Commit())
            {
                _mediator.RaiseEvent(new AnnotationDeletedEvent(request.AggregateId));
            }
            return Unit.Task;
        }
        
        private bool AnnotationValid(AnnotationHistory annotation)
        {
            if (annotation.IsValid()) return true;

            NotificationValidationsError(annotation.ValidationResult);
            return false;
        }

        private bool AnnotationExisting(Guid id, string messageType)
        {
            var annotation = _annotationRepository.GetById(id);

            if (annotation != null) return true;

            _mediator.RaiseEvent(new DomainNotification(messageType, "Annotation not found"));
            return false;
        }
    }
}
