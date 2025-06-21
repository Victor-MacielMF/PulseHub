using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Domain.Entities;

namespace PulseHub.Application.Mappings.Profiles
{
    public class QueueMessageProfile : Profile
    {
        public QueueMessageProfile()
        {
            CreateMap<QueueMessage, QueueMessageResponseDto>().ReverseMap();
        }
    }
}
