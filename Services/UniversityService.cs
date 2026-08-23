using AutoMapper;
using UNIOOP.App.Caching;
using UNIOOP.App.Dtos.Universities;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly IUniversityRepository _universityRepository;
        private readonly ExceptionHelper _exceptionHelper;
        private readonly IMapper _mapper;
        private readonly IInMemoryCacheService _cacheService;

        public UniversityService(IUniversityRepository universityRepository,
            ExceptionHelper exceptionHelper,
            IMapper mapper,
            IInMemoryCacheService cacheService
            )
        {
            _universityRepository = universityRepository;
            _exceptionHelper = exceptionHelper;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<List<UniversityResponseDto>> GetAllAsync()
        {
            const string cacheKey = "Universities:All";

            List<UniversityResponseDto>? universities = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                {
                    var universityEntities = await _universityRepository.GetAllAsync();
                    return _mapper.Map<List<UniversityResponseDto>>(universityEntities);
                });

            if (universities != null)
            {
                return universities;
            }
            else
            {
                return new List<UniversityResponseDto>();
            }
        }

        public async Task<UniversityResponseDto> GetSingleAsync(int universityId)
        {
            string cacheKey = $"University:{universityId}";

            UniversityResponseDto? university = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                {
                    var university = await _universityRepository.GetByIdAsync(universityId);

                    if (university is null)
                    {
                        return null;
                    }

                    return _mapper.Map<UniversityResponseDto>(university);
                });

            if (university is null)
            {
                throw _exceptionHelper.NotFound("University", universityId);
            }

            return university;
        }

        public async Task<UniversityResponseDto> CreateAsync(UniversityCreateUpdateDto dto)
        {
            string normalizedName = InputNormalizationHelper.NormalizeText(dto.UniversityName);

            await _exceptionHelper.EnsureUniversityNameAvailableAsync(normalizedName);

            var university = new University
            {
                UniversityName = normalizedName
            };

            await _universityRepository.AddAsync(university);
            await _universityRepository.SaveChangesAsync();
            _cacheService.Remove("Universities:All");

            return await GetSingleAsync(university.UniversityID);
        }

        public async Task UpdateAsync(int universityId, UniversityCreateUpdateDto dto)
        {
            University? existingUniversity = await _universityRepository
                .GetByIdForUpdateAsync(universityId);

            if (existingUniversity is null)
            {
                throw _exceptionHelper.NotFound("University", universityId);
            }

            string normalizedName = InputNormalizationHelper.NormalizeText(dto.UniversityName);

            await _exceptionHelper.EnsureUniversityNameAvailableAsync(normalizedName, universityId);

            existingUniversity.UniversityName = normalizedName;

            await _universityRepository.SaveChangesAsync();
            _cacheService.Remove($"University:{universityId}");
            _cacheService.Remove("Universities:All");
        }

        public async Task DeleteAsync(int universityId)
        {
            University? existingUniversity = await _universityRepository
                .GetByIdForUpdateAsync(universityId);

            if (existingUniversity is null)
            {
                throw _exceptionHelper.NotFound("University", universityId);
            }

            await _exceptionHelper.EnsureUniversityCanBeDeletedAsync(universityId);

            _universityRepository.Remove(existingUniversity);
            await _universityRepository.SaveChangesAsync();
            _cacheService.Remove($"University:{universityId}");
            _cacheService.Remove("Universities:All");
        }
    }
}