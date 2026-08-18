using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Helpers;
using UNIOOP.App.Repositories.Interfaces;
using AutoMapper;
using UNIOOP.App.Exceptions;

namespace UNIOOP.App.Services
{
    public class GovernmentOfficerService : IGovernmentOfficerService
    {
        private readonly IGovernmentOfficerRepository _governmentOfficerRepository;
        private readonly IPersonnelRepository _personnelRepository;
        private readonly IMapper _mapper;

        public GovernmentOfficerService(IGovernmentOfficerRepository governmentOfficerRepository,
        IPersonnelRepository personnelRepository,
        IMapper mapper)
        {
            _governmentOfficerRepository = governmentOfficerRepository;
            _personnelRepository = personnelRepository;
            _mapper = mapper;
        }

        public async Task<List<GovernmentOfficerResponseDto>> GetAllAsync()
        {
            var governmentOfficers = await _governmentOfficerRepository.GetAllAsync();
            return _mapper.Map<List<GovernmentOfficerResponseDto>>(governmentOfficers);
        }

        public async Task<GovernmentOfficerResponseDto> GetSingleAsync(int governmentOfficerId)
        {
            var governmentOfficer = await _governmentOfficerRepository.GetByIdAsync(governmentOfficerId);
            if (governmentOfficer is null)
            {
                throw new NotFoundException($"Officer with ID {governmentOfficerId} was not found.");
            }
            return _mapper.Map<GovernmentOfficerResponseDto>(governmentOfficer);
        }

        public async Task<GovernmentOfficerResponseDto> CreateAsync(CreateGovernmentOfficerDto dto)
        {
            string normalizedSsn = InputNormalizationHelper.NormalizeText(dto.SSN);
            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(dto.Email);

            if (await _personnelRepository.SSNExistsAsync(normalizedSsn))
            {
                throw new ConflictException($"The SSN {normalizedSsn} is already in use");
            }

            if (await _personnelRepository.EmailExistsAsync(normalizedEmail))
            {
                throw new ConflictException($"The email {normalizedEmail} is already in use");
            }

            var governmentOfficer = new GovernmentOfficer
            {
                SSN = normalizedSsn,
                FName = InputNormalizationHelper.NormalizeText(dto.FName),
                LName = InputNormalizationHelper.NormalizeText(dto.LName),
                DateOfBirth = dto.DateOfBirth,
                Email = normalizedEmail
            };

            await _governmentOfficerRepository.AddAsync(governmentOfficer);
            await _governmentOfficerRepository.SaveChangesAsync();

            return await GetSingleAsync(governmentOfficer.OfficerID);
        }
        public async Task UpdateAsync(int governmentOfficerId, UpdateGovernmentOfficerDto dto)
        {
            GovernmentOfficer? existingGovernmentOfficer = await _governmentOfficerRepository
                .GetByIdForUpdateAsync(governmentOfficerId);

            if (existingGovernmentOfficer is null)
            {
                throw new NotFoundException($"Officer with ID {governmentOfficerId} was not found");
            }

            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(dto.Email);

            if (await _personnelRepository.EmailExistsAsync(normalizedEmail, existingGovernmentOfficer.PersonnelID))
            {
                throw new ConflictException($"Another person already uses this email: {normalizedEmail}");
            }

            existingGovernmentOfficer.FName = InputNormalizationHelper.NormalizeText(dto.FName);
            existingGovernmentOfficer.LName = InputNormalizationHelper.NormalizeText(dto.LName);
            existingGovernmentOfficer.Email = normalizedEmail;
            existingGovernmentOfficer.DateOfBirth = dto.DateOfBirth;

            await _governmentOfficerRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int governmentOfficerId)
        {
            GovernmentOfficer? existingGovernmentOfficer =
                await _governmentOfficerRepository.GetByIdForUpdateAsync(governmentOfficerId);

            if (existingGovernmentOfficer is null)
            {
                throw new NotFoundException($"Officer with ID {governmentOfficerId} was not found");
            }

            _governmentOfficerRepository.Remove(existingGovernmentOfficer);

            await _governmentOfficerRepository.SaveChangesAsync();
        }
    }
}