using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Dtos.Universities;
using UNIOOP.App.Models;
using UNIOOP.App.Helpers;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Exceptions;

namespace UNIOOP.App.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly IUniversityRepository _universityRepository;

        public UniversityService(IUniversityRepository universityRepository)
        {
            _universityRepository = universityRepository;
        }

        public async Task<List<UniversityResponseDto>> GetAllAsync()
        {
            var universities = await _universityRepository.GetAllAsync();

            return universities.Select(u => new UniversityResponseDto
            {
                UniversityID = u.UniversityID,
                UniversityName = u.UniversityName
            }).ToList();
        }

        public async Task<UniversityResponseDto> GetSingleAsync(int universityId)
        {
            var university = await _universityRepository.GetByIdAsync(universityId);
            if (university == null)
            {
                throw new NotFoundException($"University with ID {universityId} was not found.");
            }

            return new UniversityResponseDto
            {
                UniversityID = university.UniversityID,
                UniversityName = university.UniversityName
            };
        }

        public async Task<UniversityResponseDto> CreateAsync(UniversityCreateUpdateDto dto)
        {
            string normalizedName = InputNormalizationHelper.NormalizeText(dto.UniversityName);

            if (await _universityRepository.NameExistsAsync(normalizedName))
            {
                throw new ConflictException($"A university with the name '{normalizedName}' already exists.");
            }

            var university = new University
            {
                UniversityName = normalizedName
            };

            await _universityRepository.AddAsync(university);
            await _universityRepository.SaveChangesAsync();

            return new UniversityResponseDto
            {
                UniversityID = university.UniversityID,
                UniversityName = university.UniversityName
            };
        }

        public async Task UpdateAsync(int universityId, UniversityCreateUpdateDto dto)
        {
            University? university = await _universityRepository.GetByIdForUpdateAsync(universityId);

            if (university is null)
            {
                throw new NotFoundException($"University with ID {universityId} was not found.");
            }

            string normalizedName = InputNormalizationHelper.NormalizeText(dto.UniversityName);

            if (await _universityRepository.NameExistsAsync(normalizedName, universityId))
            {
                throw new ConflictException($"A university with the name '{normalizedName}' already exists.");
            }

            university.UniversityName = normalizedName;
            await _universityRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int universityId)
        {
            University? university = await _universityRepository.GetByIdForUpdateAsync(universityId);

            if (university is null)
            {
                throw new NotFoundException($"University with ID {universityId} was not found.");
            }

            if (await _universityRepository.HasDependenciesAsync(universityId))
            {
                throw new ConflictException($"The university cannot be deleted because it has related students, teachers or courses.");
            }
            _universityRepository.Remove(university);
            await _universityRepository.SaveChangesAsync();
        }
    }
}