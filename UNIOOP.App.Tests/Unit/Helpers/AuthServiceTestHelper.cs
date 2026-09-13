using Microsoft.AspNetCore.Identity;
using Moq;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Tests.Unit.Helpers
{
    public class AuthServiceTestHelper
    {
        public Mock<IUserAccountRepository> RepositoryMock { get; }
        public Mock<ITokenService> TokenServiceMock { get; }
        public PasswordHasher<UserAccount> PasswordHasher { get; }
        public AuthService Service { get; }

        public AuthServiceTestHelper()
        {
            RepositoryMock = new Mock<IUserAccountRepository>();
            TokenServiceMock = new Mock<ITokenService>();
            PasswordHasher = new PasswordHasher<UserAccount>();

            var universityRepositoryMock = new Mock<IUniversityRepository>();

            var studentRepositoryMock = new Mock<IStudentRepository>();

            var teacherRepositoryMock = new Mock<ITeacherRepository>();

            var courseRepositoryMock = new Mock<ICourseRepository>();

            var personnelRepositoryMock = new Mock<IPersonnelRepository>();

            var enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();

            var exceptionHelper = new ExceptionHelper(universityRepositoryMock.Object,
                studentRepositoryMock.Object,
                teacherRepositoryMock.Object,
                courseRepositoryMock.Object,
                personnelRepositoryMock.Object,
                enrollmentRepositoryMock.Object);

            Service = new AuthService(RepositoryMock.Object,
                exceptionHelper,
                PasswordHasher,
                TokenServiceMock.Object);
        }

        public UserAccount CreateUserAccount(long personnelId = 1, string role = "SuperAdmin", string password = "TestPassword123!")
        {
            var userAccount = new UserAccount
            {
                PersonnelID = personnelId,
                Role = role
            };

            userAccount.PasswordHash = PasswordHasher.HashPassword(userAccount, password);

            return userAccount;
        }
    }
}
