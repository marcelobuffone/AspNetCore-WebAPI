using System;
using System.Collections.Generic;
using AutoMapper;
using MBAM.Annotations.Domain.Annotations.Commands;
using MBAM.Annotations.Domain.Annotations.Repository;
using MBAM.Annotations.Domain.Core.Notifications;
using MBAM.Annotations.Domain.Interfaces;
using MBAM.Annotations.Services.Api.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MBAM.Annotations.Services.Api.Controllers
{
    public class AnnotationsController : BaseController
    {

        private readonly IMediatorHandler _mediator;
        private readonly IAnnotationRepository _annotationRepository;
        private readonly IMapper _mapper;

        public AnnotationsController(INotificationHandler<DomainNotification> notifications,
                                     IMediatorHandler mediator,
                                     IAnnotationRepository annotationRepository,
                                     IMapper mapper) : base(notifications, mediator)
        {
            _annotationRepository = annotationRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("annotations")]
        public IEnumerable<AnnotationViewModel> Get()
        {
            return _mapper.Map<IEnumerable<AnnotationViewModel>>(_annotationRepository.GetAll());
        }

        [HttpGet]
        [Route("annotations/{id:guid}")]
        public IEnumerable<AnnotationViewModel> Get(Guid id)
        {
            return _mapper.Map<IEnumerable<AnnotationViewModel>>(_annotationRepository.GetById(id));
        }

        [HttpPost]
        [Route("annotations")]
        public IActionResult Post([FromBody]AnnotationHistoryViewModel annotationHistoryViewModel)
        {
            if (!IsValidModelState())
            {
                return Response();
            }

            var annotationCommand = _mapper.Map<RegisterAnnotationCommand>(annotationHistoryViewModel);

            _mediator.SendCommand(annotationCommand);
            return Response(annotationCommand);
        }

        [HttpPut]
        [Route("annotations")]
        public IActionResult Put([FromBody]AnnotationHistoryViewModel annotationHistoryViewModel)
        {
            if (!IsValidModelState())
            {
                return Response();
            }

            var annotationCommand = _mapper.Map<UpdateAnnotationCommand>(annotationHistoryViewModel);

            _mediator.SendCommand(annotationCommand);
            return Response(annotationCommand);
        }

        [HttpDelete]
        [Route("annotations/{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var annotationViewModel = new AnnotationViewModel { Id = id };
            var annotationCommand = _mapper.Map<DeleteAnnotationCommand>(annotationViewModel);

            _mediator.SendCommand(annotationCommand);
            return Response(annotationCommand);
        }

        private bool IsValidModelState()
        {
            if (ModelState.IsValid) return true;

            NotifyInvalidErrorModel();
            return false;
        }

    }
}
