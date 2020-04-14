using AutoMapper;
using MBAM.Annotations.Domain.Annotations.Commands;
using MBAM.Annotations.Services.Api.ViewModels;

namespace MBAM.Annotations.Services.Api.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<AnnotationHistoryViewModel, RegisterAnnotationCommand>()
                .ConstructUsing(c => new RegisterAnnotationCommand(c.Title, c.Description));

            CreateMap<AnnotationHistoryViewModel, UpdateAnnotationCommand>()
                .ConstructUsing(c => new UpdateAnnotationCommand(c.Title, c.Description, c.Id));

            CreateMap<AnnotationViewModel, DeleteAnnotationCommand>()
                .ConstructUsing(c => new DeleteAnnotationCommand(c.Id));
        }
    }
}
