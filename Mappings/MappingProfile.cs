using AutoMapper;
using UNIOOP.App.Dtos.Courses;
using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Dtos.Students;
using UNIOOP.App.Dtos.Enrollments;
using UNIOOP.App.Dtos.Teachers;
using UNIOOP.App.Dtos.Universities;
using UNIOOP.App.Models;

namespace UNIOOP.App.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<University, UniversityResponseDto>();

            CreateMap<UniversityCreateUpdateDto, University>();

            CreateMap<CreateStudentDto, Student>();

            CreateMap<UpdateStudentDto, Student>();

            CreateMap<Student, StudentResponseDto>()
                .ForMember(
                    dest => dest.UniversityName,
                    opt => opt.MapFrom(src => src.University.UniversityName)
                );

            CreateMap<CreateTeacherDto, Teacher>();

            CreateMap<UpdateTeacherDto, Teacher>();

            CreateMap<Teacher, TeacherResponseDto>()
                .ForMember(
                    dest => dest.UniversityName,
                    opt => opt.MapFrom(src => src.University.UniversityName)
                );

            CreateMap<CreateCourseDto, Course>();

            CreateMap<UpdateCourseDto, Course>();

            CreateMap<Course, CourseResponseDto>()
                .ForMember(
                    dest => dest.UniversityName,
                    opt => opt.MapFrom(src => src.University.UniversityName)
                )
                .ForMember(
                    dest => dest.TeacherName,
                    opt => opt.MapFrom(src =>
                        src.Teacher == null
                            ? null
                            : $"{src.Teacher.FName} {src.Teacher.LName}")
                );

            CreateMap<CreateGovernmentOfficerDto, GovernmentOfficer>();

            CreateMap<UpdateGovernmentOfficerDto, GovernmentOfficer>();

            CreateMap<GovernmentOfficer, GovernmentOfficerResponseDto>();

            CreateMap<CreateEnrollmentDto, StudentCourse>();

            CreateMap<StudentCourse, EnrollmentResponseDto>()
                .ForMember(
                    dest => dest.StudentName,
                    opt => opt.MapFrom(src =>
                        $"{src.Student.FName} {src.Student.LName}")
                )
                .ForMember(
                    dest => dest.CourseName,
                    opt => opt.MapFrom(src => src.Course.CourseName)
                );
        }
    }
}