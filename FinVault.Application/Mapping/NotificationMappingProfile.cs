using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.NotificationDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Notification DTOs to and from the Notification domain entity.</summary>
public sealed class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateNotificationDto, Notification>()
            .ForMember(dest => dest.NotificationId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateNotificationDto, Notification>()
            .ForMember(dest => dest.NotificationId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Notification, NotificationDto>();
    }
}
