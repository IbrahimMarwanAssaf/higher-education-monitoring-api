using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Dtos.Universities;
using UNIOOP.App.Models;
using UNIOOP.App.Helpers;
using UNIOOP.App.Repositories.Interfaces;

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

        public async Task<UniversityResponseDto?> GetSingleAsync(int universityId)
        {
            var university = await _universityRepository.GetByIdAsync(universityId);
            if (university == null)
            {
                return null;
            }

            return new UniversityResponseDto
            {
                UniversityID = university.UniversityID,
                UniversityName = university.UniversityName
            };
        }

        public async Task<UniversityResponseDto> CreateAsync(UniversityCreateUpdateDto dto)
        {
            var university = new University
            {
                UniversityName = InputNormalizationHelper.NormalizeText(dto.UniversityName)
            };

            await _universityRepository.AddAsync(university);
            await _universityRepository.SaveChangesAsync();

            return new UniversityResponseDto
            {
                UniversityID = university.UniversityID,
                UniversityName = university.UniversityName
            };
        }

        public async Task<bool> UpdateAsync(int universityId, UniversityCreateUpdateDto dto)
        {
            University? university = await _universityRepository.GetByIdForUpdateAsync(universityId);

            if (university == null)
            {
                return false;
            }

            university.UniversityName = InputNormalizationHelper.NormalizeText(dto.UniversityName);
            await _universityRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int universityId)
        {
            University? university = await _universityRepository.GetByIdForUpdateAsync(universityId);

            if (university == null)
            {
                return false;
            }

            _universityRepository.Remove(university);
            await _universityRepository.SaveChangesAsync();

            return true;
        }
    }
}