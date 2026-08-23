using AutoMapper;
using UNIOOP.App.Caching;
using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Services
{
    public class GovernmentOfficerService : IGovernmentOfficerService
    {
        private readonly IGovernmentOfficerRepository _governmentOfficerRepository;
        private readonly ExceptionHelper _exceptionHelper;
        private readonly IMapper _mapper;
        private readonly IInMemoryCacheService _cacheService;

        public GovernmentOfficerService(IGovernmentOfficerRepository governmentOfficerRepository,
            ExceptionHelper exceptionHelper,
            IInMemoryCacheService cacheService,
            IMapper mapper)
        {
            _governmentOfficerRepository = governmentOfficerRepository;
            _exceptionHelper = exceptionHelper;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<List<GovernmentOfficerResponseDto>> GetAllAsync()
        {
            const string cacheKey = "GovernmentOfficers:All";

            List<GovernmentOfficerResponseDto>? governmentOfficers = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                    {
                        var officerEntities = await _governmentOfficerRepository.GetAllAsync();
                        return _mapper.Map<List<GovernmentOfficerResponseDto>>(officerEntities);
                    });

            if (governmentOfficers != null)
            {
                return governmentOfficers;
            }
            else
            {
                return new List<GovernmentOfficerResponseDto>();
            }
        }

        public async Task<GovernmentOfficerResponseDto> GetSingleAsync(int governmentOfficerId)
        {
            string cacheKey = $"GovernmentOfficer:{governmentOfficerId}";

            GovernmentOfficerResponseDto? governmentOfficer = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                    {
                        GovernmentOfficer? officerEntity = await _governmentOfficerRepository
                            .GetByIdAsync(governmentOfficerId);

                        if (officerEntity is null)
                        {
                            return null;
                        }

                        return _mapper.Map<GovernmentOfficerResponseDto>(officerEntity);
                    });

            if (governmentOfficer is null)
            {
                throw _exceptionHelper.NotFound("Officer", governmentOfficerId);
            }

            return governmentOfficer;
        }

        public async Task<GovernmentOfficerResponseDto> CreateAsync(
            CreateGovernmentOfficerDto dto)
        {
            string normalizedSsn = InputNormalizationHelper.NormalizeText(dto.SSN);
            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(dto.Email);

            await _exceptionHelper.EnsureSsnAvailableAsync(normalizedSsn);
            await _exceptionHelper.EnsureEmailAvailableAsync(normalizedEmail);

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

            await _cacheService.RemoveAsync("GovernmentOfficers:All");

            return await GetSingleAsync(governmentOfficer.OfficerID);
        }

        public async Task UpdateAsync(int governmentOfficerId, UpdateGovernmentOfficerDto dto)
        {
            GovernmentOfficer? existingGovernmentOfficer = await _governmentOfficerRepository
                .GetByIdForUpdateAsync(governmentOfficerId);

            if (existingGovernmentOfficer is null)
            {
                throw _exceptionHelper.NotFound("Officer", governmentOfficerId);
            }

            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(dto.Email);

            await _exceptionHelper.EnsureEmailAvailableAsync(normalizedEmail, existingGovernmentOfficer.PersonnelID);

            existingGovernmentOfficer.FName = InputNormalizationHelper.NormalizeText(dto.FName);
            existingGovernmentOfficer.LName = InputNormalizationHelper.NormalizeText(dto.LName);
            existingGovernmentOfficer.Email = normalizedEmail;
            existingGovernmentOfficer.DateOfBirth = dto.DateOfBirth;

            await _governmentOfficerRepository.SaveChangesAsync();

            await _cacheService.RemoveAsync($"GovernmentOfficer:{governmentOfficerId}");
            await _cacheService.RemoveAsync("GovernmentOfficers:All");
        }

        public async Task DeleteAsync(int governmentOfficerId)
        {
            GovernmentOfficer? existingGovernmentOfficer = await _governmentOfficerRepository
                .GetByIdForUpdateAsync(governmentOfficerId);

            if (existingGovernmentOfficer is null)
            {
                throw _exceptionHelper.NotFound("Officer", governmentOfficerId);
            }

            _governmentOfficerRepository.Remove(existingGovernmentOfficer);

            await _governmentOfficerRepository.SaveChangesAsync();

            await _cacheService.RemoveAsync($"GovernmentOfficer:{governmentOfficerId}");
            await _cacheService.RemoveAsync("GovernmentOfficers:All");
        }
    }
}