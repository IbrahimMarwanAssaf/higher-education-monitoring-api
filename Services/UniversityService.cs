using UNIOOP.App.Data;
using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Dtos.Universities;
using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Models;
namespace UNIOOP.App.Services
{

    public class UniversityService : IUniversityService
    {
        private readonly DataContextEF _entityFramework;

        public UniversityService(DataContextEF context)
        {
            _entityFramework = context;
        }

        public async Task<List<University>> GetAllAsync()
        {
            return await _entityFramework.Universities.AsNoTracking().OrderBy(u => u.UniversityID)
            .Select(u => new University
            {
                UniversityID = u.UniversityID,
                UniversityName = u.UniversityName
            }).ToListAsync();
        }

        public async Task<University?> GetSingleAsync(int universityId)
        {
            return await _entityFramework.Universities.AsNoTracking()
                .Where(u => u.UniversityID == universityId)
                .Select(u => new University
                {
                    UniversityID = u.UniversityID,
                    UniversityName = u.UniversityName
                })
                .SingleOrDefaultAsync();
        }

        public async Task<University> CreateAsync(UniversityDto dto)
        {
            var university = new University
            {
                UniversityName = dto.UniversityName
            };

            _entityFramework.Universities.Add(university);

            await _entityFramework.SaveChangesAsync();

            return university;
        }

        public async Task<bool> UpdateAsync(int universityId, UniversityDto dto)
        {
            University? university = await _entityFramework.Universities.SingleOrDefaultAsync(u => u.UniversityID == universityId);

            if (university == null)
            {
                return false;
            }

            university.UniversityName = dto.UniversityName;

            await _entityFramework.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int universityId)
        {
            University? university = await _entityFramework.Universities.SingleOrDefaultAsync(u => u.UniversityID == universityId);

            if (university == null)
            {
                return false;
            }

            _entityFramework.Universities.Remove(university);

            await _entityFramework.SaveChangesAsync();

            return true;
        }
    }
}