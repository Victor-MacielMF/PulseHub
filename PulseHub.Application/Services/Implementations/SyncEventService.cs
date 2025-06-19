using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Application.Services.Interfaces;
using PulseHub.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PulseHub.Application.Services.Implementations
{
    public class SyncEventService : ISyncEventService
    {
        private readonly ISyncEventRepository _syncEventRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SyncEventService(
            ISyncEventRepository syncEventRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _syncEventRepository = syncEventRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SyncEventResponseDto>> GetAllAsync()
        {
            var events = await _syncEventRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SyncEventResponseDto>>(events);
        }

        public async Task<SyncEventResponseDto?> GetByIdAsync(Guid syncEventId)
        {
            var syncEvent = await _syncEventRepository.GetByIdAsync(syncEventId);
            return syncEvent is null ? null : _mapper.Map<SyncEventResponseDto>(syncEvent);
        }

        public async Task DeleteAsync(Guid syncEventId)
        {
            var syncEvent = await _syncEventRepository.GetByIdAsync(syncEventId);

            if (syncEvent is null)
                throw new Exception("Evento de sincronização não encontrado.");

            _syncEventRepository.Delete(syncEvent);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
