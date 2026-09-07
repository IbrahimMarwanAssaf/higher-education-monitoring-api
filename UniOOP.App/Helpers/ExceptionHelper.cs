using UNIOOP.App.Exceptions;
using UNIOOP.App.Repositories.Interfaces;

namespace UNIOOP.App.Helpers;

public class ExceptionHelper
{
    private readonly IUniversityRepository _universityRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IPersonnelRepository _personnelRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public ExceptionHelper(
        IUniversityRepository universityRepository,
        IStudentRepository studentRepository,
        ITeacherRepository teacherRepository,
        ICourseRepository courseRepository,
        IPersonnelRepository personnelRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _universityRepository = universityRepository;
        _studentRepository = studentRepository;
        _teacherRepository = teacherRepository;
        _courseRepository = courseRepository;
        _personnelRepository = personnelRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public NotFoundException NotFound(string entity, object id) =>
        new($"{entity} with ID {id} was not found.");

    public NotFoundException SelectedNotFound(string entity, object id) =>
        new($"The selected {entity} with ID {id} does not exist.");

    public ConflictException Conflict(string message) => new(message);

    public BadRequestException BadRequest(string message) => new(message);

    public async Task EnsureUniversityExistsAsync(int universityId)
    {
        if (!await _universityRepository.ExistsAsync(universityId))
            throw SelectedNotFound("university", universityId);
    }

    public async Task EnsureStudentExistsAsync(int studentId)
    {
        if (!await _studentRepository.ExistsAsync(studentId))
            throw NotFound("Student", studentId);
    }

    public async Task EnsureTeacherExistsAsync(int teacherId)
    {
        if (!await _teacherRepository.ExistsAsync(teacherId))
            throw SelectedNotFound("teacher", teacherId);
    }

    public async Task EnsureCourseExistsAsync(int courseId)
    {
        if (!await _courseRepository.ExistsAsync(courseId))
            throw NotFound("Course", courseId);
    }

    public async Task EnsureSsnAvailableAsync(string ssn)
    {
        if (await _personnelRepository.SSNExistsAsync(ssn))
            throw Conflict($"The SSN {ssn} is already in use");
    }

    public async Task EnsureEmailAvailableAsync(string email, long? excludePersonnelId = null)
    {
        if (await _personnelRepository.EmailExistsAsync(email, excludePersonnelId))
            throw Conflict($"The email {email} is already in use");
    }

    public async Task EnsureTeacherBelongsToUniversityAsync(int teacherId, int universityId)
    {
        if (!await _teacherRepository.BelongsToUniversityAsync(teacherId, universityId))
            throw BadRequest($"The teacher with ID {teacherId} does not belong to university {universityId}.");
    }

    public async Task EnsureCourseNameAvailableAsync(string courseName, int universityId, int? excludeCourseId = null)
    {
        if (await _courseRepository.NameExistsAsync(courseName, universityId, excludeCourseId))
            throw Conflict($"The course '{courseName}' already exists in university {universityId}.");
    }

    public async Task EnsureUniversityNameAvailableAsync(string universityName, int? excludeUniversityId = null)
    {
        if (await _universityRepository.NameExistsAsync(universityName, excludeUniversityId))
            throw Conflict($"A university with the name '{universityName}' already exists.");
    }

    public async Task EnsureStudentCanBeDeletedAsync(int studentId)
    {
        if (await _studentRepository.HasEnrollmentsAsync(studentId))
            throw Conflict("The student cannot be deleted while enrolled in courses");
    }

    public async Task EnsureCourseCanBeDeletedAsync(int courseId)
    {
        if (await _courseRepository.HasEnrollmentsAsync(courseId))
            throw Conflict($"Course with ID {courseId} cannot be deleted while students are enrolled in it.");
    }

    public async Task EnsureUniversityCanBeDeletedAsync(int universityId)
    {
        if (await _universityRepository.HasDependenciesAsync(universityId))
            throw Conflict("The university cannot be deleted because it has related students, teachers or courses.");
    }

    public async Task EnsureEnrollmentDoesNotExistAsync(int studentId, int courseId)
    {
        if (await _enrollmentRepository.ExistsAsync(studentId, courseId))
            throw Conflict($"The student with ID {studentId} is already enrolled in the course with ID {courseId}.");
    }

    public async Task EnsureStudentAndCourseSameUniversityAsync(int studentId, int courseId)
    {
        if (!await _enrollmentRepository.StudentAndCourseSameUniversityAsync(studentId, courseId))
            throw BadRequest("The student and course must belong to the same university.");
    }
}
