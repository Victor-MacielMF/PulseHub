using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Domain.Entities;

namespace PulseHub.Application.Mappings.Profiles
{
    public class QueueMessageProfile : Profile
    {
        public QueueMessageProfile()
        {
            // Mapeia de QueueMessage para QueueMessageResponseDto
            CreateMap<QueueMessage, QueueMessageResponseDto>();

            // Mapeia de QueueMessageResponseDto para QueueMessage (se precisar)
            CreateMap<QueueMessageResponseDto, QueueMessage>();
        }
    }
}
