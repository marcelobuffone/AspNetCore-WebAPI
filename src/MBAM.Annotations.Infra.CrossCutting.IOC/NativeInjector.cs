using MBAM.Annotations.Domain.Annotations.Commands;
using MBAM.Annotations.Domain.Annotations.Events;
using MBAM.Annotations.Domain.Annotations.Repository;
using MBAM.Annotations.Domain.Core.Notifications;
using MBAM.Annotations.Domain.Handlers;
using MBAM.Annotations.Domain.Interfaces;
using MBAM.Annotations.Infra.CrossCutting.Identity.Models;
using MBAM.Annotations.Infra.CrossCutting.Identity.Services;
using MBAM.Annotations.Infra.Data.Context;
using MBAM.Annotations.Infra.Data.EventSourcing;
using MBAM.Annotations.Infra.Data.Repository;
using MBAM.Annotations.Infra.Data.Repository.EventSourcing;
using MBAM.Annotations.Infra.Data.UoW;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MBAM.Annotations.Infra.CrossCutting.IOC
{
    public class NativeInjector
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // domain bus (Mediator)
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            //domain commands
            services.AddScoped<IRequestHandler<RegisterAnnotationCommand>, AnnotationCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateAnnotationCommand>, AnnotationCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteAnnotationCommand>, AnnotationCommandHandler>();

            //domain eventos
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            services.AddScoped<INotificationHandler<AnnotationRegistredEvent>, AnnotationEventHandler>();
            services.AddScoped<INotificationHandler<AnnotationUpdatedEvent>, AnnotationEventHandler>();
            services.AddScoped<INotificationHandler<AnnotationDeletedEvent>, AnnotationEventHandler>();
            
            //infra data
            services.AddScoped<IAnnotationRepository, AnnotationRespository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<AnnotationsContext>();

            // infra - data EventSourcing
            services.AddScoped<IEventStoreRepository, EventStoreSQLRepository>();
            services.AddScoped<IEventStore, SqlEventStore>();
            services.AddScoped<EventStoreSQLContext>();

            // infra - identity
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddScoped<IUser, AspNetUser>();
        }
    }
}
