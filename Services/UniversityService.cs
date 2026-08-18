using AutoMapper;
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

        public UniversityService(IUniversityRepository universityRepository,
            ExceptionHelper exceptionHelper,
            IMapper mapper)
        {
            _universityRepository = universityRepository;
            _exceptionHelper = exceptionHelper;
            _mapper = mapper;
        }

        public async Task<List<UniversityResponseDto>> GetAllAsync()
        {
            var universities = await _universityRepository.GetAllAsync();
            return _mapper.Map<List<UniversityResponseDto>>(universities);
        }

        public async Task<UniversityResponseDto> GetSingleAsync(int universityId)
        {
            var university = await _universityRepository.GetByIdAsync(universityId);

            if (university is null)
            {
                throw _exceptionHelper.NotFound("University", universityId);
            }

            return _mapper.Map<UniversityResponseDto>(university);
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
        }
    }
}