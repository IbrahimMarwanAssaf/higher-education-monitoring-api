using Microsoft.AspNetCore.Identity;
using UNIOOP.App.Data;
using UNIOOP.App.Models;

namespace UNIOOP.App.Tests.Integration.Infrastructure
{
    public static class IntegrationTestSeeder
    {
        public static void Seed(
            DataContextEF context,
            string testPassword)
        {
            var passwordHasher = new PasswordHasher<UserAccount>();

            var testUsers = new[]
            {
                new GovernmentOfficer
                {
                    SSN = "TEST-SUPERADMIN-001",
                    FName = "Test",
                    LName = "SuperAdmin",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Email = "superadmin@test.local"
                },
                new GovernmentOfficer
                {
                    SSN = "TEST-ADMIN-001",
                    FName = "Test",
                    LName = "Admin",
                    DateOfBirth = new DateOnly(1990, 1, 2),
                    Email = "admin@test.local"
                },
                new GovernmentOfficer
                {
                    SSN = "TEST-MANAGER-001",
                    FName = "Test",
                    LName = "Manager",
                    DateOfBirth = new DateOnly(1990, 1, 3),
                    Email = "manager@test.local"
                },
                new GovernmentOfficer
                {
                    SSN = "TEST-USER-001",
                    FName = "Test",
                    LName = "User",
                    DateOfBirth = new DateOnly(1990, 1, 4),
                    Email = "user@test.local"
                }
            };

            var roles = new[]
            {
                "SuperAdmin",
                "Admin",
                "Manager",
                "User"
            };

            for (int i = 0; i < testUsers.Length; i++)
            {
                var officer = testUsers[i];

                var account = new UserAccount
                {
                    Personnel = officer,
                    Role = roles[i]
                };

                account.PasswordHash = passwordHasher.HashPassword(
                    account,
                    testPassword);

                context.GovernmentOfficers.Add(officer);
                context.UserAccounts.Add(account);
            }

            context.SaveChanges();
        }
    }
}