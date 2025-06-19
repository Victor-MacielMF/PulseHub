using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Domain.Entities;

namespace PulseHub.Application.Mappings.Profiles
{
    public class SyncEventProfile : Profile
    {
        public SyncEventProfile()
        {
            // Mapeia de SyncEvent para SyncEventResponseDto
            CreateMap<SyncEvent, SyncEventResponseDto>();

            // Mapeia de SyncEventResponseDto para SyncEvent (se precisar)
            CreateMap<SyncEventResponseDto, SyncEvent>();
        }
    }
}
