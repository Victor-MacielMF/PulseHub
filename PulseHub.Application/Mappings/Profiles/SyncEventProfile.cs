using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Domain.Entities;

namespace PulseHub.Application.Mappings.Profiles
{
    public class SyncEventProfile : Profile
    {
        public SyncEventProfile()
        {
            CreateMap<SyncEvent, SyncEventResponseDto>();
            CreateMap<SyncEventResponseDto, SyncEvent>();
        }
    }
}
