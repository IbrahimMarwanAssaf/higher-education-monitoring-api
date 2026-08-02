using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Helpers;

namespace UNIOOP.App.Services
{
    public class GovernmentOfficerService : IGovernmentOfficerService
    {
        private readonly DataContextEF _entityFramework;

        public GovernmentOfficerService(DataContextEF context)
        {
            _entityFramework = context;
        }

        public async Task<List<GovernmentOfficerResponseDto>> GetAllAsync()
        {
            return await (
                from governmentOfficer in _entityFramework.GovernmentOfficers.AsNoTracking()
                orderby governmentOfficer.OfficerID
                select new GovernmentOfficerResponseDto
                {
                    OfficerID = governmentOfficer.OfficerID,
                    SSN = governmentOfficer.SSN,
                    FName = governmentOfficer.FName,
                    LName = governmentOfficer.LName,
                    DateOfBirth = governmentOfficer.DateOfBirth,
                    Email = governmentOfficer.Email
                }).ToListAsync();
        }
        public async Task<GovernmentOfficerResponseDto?> GetSingleAsync(int governmentOfficerId)
        {
            return await (
             from governmentOfficer in _entityFramework.GovernmentOfficers.AsNoTracking()
             where governmentOfficer.OfficerID == governmentOfficerId
             select new GovernmentOfficerResponseDto
             {
                 OfficerID = governmentOfficer.OfficerID,
                 SSN = governmentOfficer.SSN,
                 FName = governmentOfficer.FName,
                 LName = governmentOfficer.LName,
                 DateOfBirth = governmentOfficer.DateOfBirth,
                 Email = governmentOfficer.Email
             }).SingleOrDefaultAsync();
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

            _entityFramework.GovernmentOfficers.Add(governmentOfficer);

            await _entityFramework.SaveChangesAsync();

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
            GovernmentOfficer? existingGovernmentOfficer = await _entityFramework.GovernmentOfficers
                .SingleOrDefaultAsync(s => s.OfficerID == governmentOfficerId);

            if (existingGovernmentOfficer is null)
            {
                return false;
            }

            existingGovernmentOfficer.FName = InputNormalizationHelper.NormalizeText(dto.FName);
            existingGovernmentOfficer.LName = InputNormalizationHelper.NormalizeText(dto.LName);
            existingGovernmentOfficer.Email = InputNormalizationHelper.NormalizeEmail(dto.Email);
            existingGovernmentOfficer.DateOfBirth = dto.DateOfBirth;


            await _entityFramework.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int governmentOfficerId)
        {
            GovernmentOfficer? existingGovernmentOfficer = await _entityFramework.GovernmentOfficers
                .SingleOrDefaultAsync(s => s.OfficerID == governmentOfficerId);

            if (existingGovernmentOfficer is null)
            {
                return false;
            }

            _entityFramework.GovernmentOfficers.Remove(existingGovernmentOfficer);

            await _entityFramework.SaveChangesAsync();

            return true;
        }

    }
}