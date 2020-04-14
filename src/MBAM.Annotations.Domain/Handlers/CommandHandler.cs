using FluentValidation.Results;
using MBAM.Annotations.Domain.Core.Notifications;
using MBAM.Annotations.Domain.Interfaces;
using MediatR;

namespace MBAM.Annotations.Domain.CommandHandlers
{
    public abstract class CommandHandler
    {
        private readonly IUnitOfWork _uow;
        private readonly IMediatorHandler _mediator;
        private readonly DomainNotificationHandler _notifications;

        public CommandHandler(IUnitOfWork uow,
                              IMediatorHandler mediator,
                              INotificationHandler<DomainNotification> notifications)
        {
            _uow = uow;
            _mediator = mediator;
            _notifications = (DomainNotificationHandler)notifications;
        }

        protected void NotificationValidationsError(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                _mediator.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));
            }
        }

        protected bool Commit()
        {
            if (_notifications.HasNotifications())
                return false;

            var commandResponse = _uow.Commit();
            if (commandResponse.Success) return true;

            _mediator.RaiseEvent(new DomainNotification("Commit", "Error in commit"));

            return false;
        }

    }
}
