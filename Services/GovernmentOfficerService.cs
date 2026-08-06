using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Helpers;
using UNIOOP.App.Repositories.Interfaces;
using AutoMapper;

namespace UNIOOP.App.Services
{
    public class GovernmentOfficerService : IGovernmentOfficerService
    {
        private readonly IGovernmentOfficerRepository _governmentOfficerRepository;
        private readonly IMapper _mapper;

        public GovernmentOfficerService(IGovernmentOfficerRepository governmentOfficerRepository, IMapper mapper)
        {
            _governmentOfficerRepository = governmentOfficerRepository;
            _mapper = mapper;
        }

        public async Task<List<GovernmentOfficerResponseDto>> GetAllAsync()
        {
            var governmentOfficers = await _governmentOfficerRepository.GetAllAsync();
            return _mapper.Map<List<GovernmentOfficerResponseDto>>(governmentOfficers);
        }

        public async Task<GovernmentOfficerResponseDto?> GetSingleAsync(int OfficerID)
        {
            var governmentOfficer = await _governmentOfficerRepository.GetByIdAsync(OfficerID);
            return governmentOfficer == null ? null : _mapper.Map<GovernmentOfficerResponseDto>(governmentOfficer);
        }

        public async Task<GovernmentOfficerResponseDto> CreateAsync(CreateGovernmentOfficerDto dto)
        {
            var governmentOfficer = new GovernmentOfficer
            {
                SSN = InputNormalizationHelper.NormalizeSsn(dto.SSN),
                FName = InputNormalizationHelper.NormalizeText(dto.FName),
                LName = InputNormalizationHelper.NormalizeText(dto.LName),
                DateOfBirth = dto.DateOfBirth,
                Email = InputNormalizationHelper.NormalizeEmail(dto.Email)
            };

            await _governmentOfficerRepository.AddAsync(governmentOfficer);

            await _governmentOfficerRepository.SaveChangesAsync();

            GovernmentOfficerResponseDto? createdOfficer = await GetSingleAsync(governmentOfficer.OfficerID);

            if (createdOfficer is null)
            {
                throw new InvalidOperationException(
                    "The government officer was created but could not be retrieved.");
            }

            return createdOfficer;
        }
        public async Task<bool> UpdateAsync(int governmentOfficerId, UpdateGovernmentOfficerDto dto)
        {
            GovernmentOfficer? existingGovernmentOfficer =
                await _governmentOfficerRepository.GetByIdForUpdateAsync(governmentOfficerId);

            if (existingGovernmentOfficer is null)
            {
                return false;
            }

            existingGovernmentOfficer.FName = InputNormalizationHelper.NormalizeText(dto.FName);
            existingGovernmentOfficer.LName = InputNormalizationHelper.NormalizeText(dto.LName);
            existingGovernmentOfficer.Email = InputNormalizationHelper.NormalizeEmail(dto.Email);
            existingGovernmentOfficer.DateOfBirth = dto.DateOfBirth;

            await _governmentOfficerRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int governmentOfficerId)
        {
            GovernmentOfficer? existingGovernmentOfficer =
                await _governmentOfficerRepository.GetByIdForUpdateAsync(governmentOfficerId);

            if (existingGovernmentOfficer is null)
            {
                return false;
            }

            _governmentOfficerRepository.Remove(existingGovernmentOfficer);

            await _governmentOfficerRepository.SaveChangesAsync();

            return true;
        }
    }
}