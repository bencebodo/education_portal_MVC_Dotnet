using EduPortal.Data.Models.Entites;
using EduPortal.Logic.Services.DashboardService;
using EduPortal.Repo.CourseRepo;
using EduPortal.Repo.UserRepo;
using NSubstitute;

namespace EduPortal.Tests.Unit.ServiceTests;

public class DashboardService_Tests
{
    private ICourseRepository _courseRepoSub;
    private IUserRepository _userRepoSub;
    private DashboardService _sut;

    [SetUp]
    public void Setup()
    {
        _courseRepoSub = Substitute.For<ICourseRepository>();
        _userRepoSub = Substitute.For<IUserRepository>();
        _sut = new DashboardService(_userRepoSub, _courseRepoSub);
    }

    [Category("Unit")]
    [Test]
    public async Task GetDashboardDataAsync_WithOngoingAndCompletedCourses_ReturnsValidData()
    {
        // ARRANGE
        Guid userId = Guid.NewGuid();

        var fakeUser = new User { Id = userId, FirstName = "Bence" };
        _userRepoSub.GetByIdAsync(userId).Returns(fakeUser);

        var enrolledCourses = new List<Course>
    {
        new Course { CourseId = 1 },
        new Course { CourseId = 2 }
    };

        var allCourses = new List<Course>
    {
        new Course { CourseId = 1, Name = "C# Basics",
            Materials = new List<Material> { new VideoMaterial(), new VideoMaterial(), new VideoMaterial(), new VideoMaterial(), new VideoMaterial() },
            Skills = new List<Skill>()
        },
        new Course { CourseId = 2, Name = "Playwright E2E",
            Materials = new List<Material> { new VideoMaterial(), new VideoMaterial() },
            Skills = new List<Skill>()
        },
        new Course { CourseId = 3, Name = "ASP.NET Core",
            Materials = new List<Material> { new VideoMaterial() },
            Skills = new List<Skill>()
        }
    };

        var userSkills = new List<Skill> { new Skill(), new Skill() };

        var completedMaterialCounts = new Dictionary<int, int>
    {
        { 1, 2 },
        { 2, 2 }
    };

        _userRepoSub.GetEnrolledCoursesByUserIdAsync(userId).Returns(enrolledCourses);
        _userRepoSub.GetUserSkillsByUserIdAsync(userId).Returns(userSkills);
        _courseRepoSub.GetAllCoursesWithDetailsAsync().Returns(allCourses);
        _userRepoSub.GetCompletedMaterialCountsByCourseAsync(userId).Returns(completedMaterialCounts);

        // ACT
        var result = await _sut.GetDashboardDataAsync(userId);

        // ASSERT
        Assert.IsNotNull(result);

        Assert.That(result.DashboardProfileDTO.FirstName, Is.EqualTo("Bence"));
        Assert.That(result.DashboardProfileDTO.TotalSkillsCount, Is.EqualTo(2));
        Assert.That(result.DashboardProfileDTO.CompletedCoursesCount, Is.EqualTo(1)); // Csak a 2-es kurzus lett 100%-os!

        Assert.That(result.EnrolledCourses.Count, Is.EqualTo(1));
        Assert.That(result.EnrolledCourses.First().CourseId, Is.EqualTo(1));
        Assert.That(result.EnrolledCourses.First().ProgressPercentage, Is.EqualTo(40)); // (2/5) * 100

        Assert.That(result.CompletedCourses.Count, Is.EqualTo(1));
        Assert.That(result.CompletedCourses.First().CourseId, Is.EqualTo(2));
        Assert.That(result.CompletedCourses.First().ProgressPercentage, Is.EqualTo(100)); // (2/2) * 100

        Assert.That(result.AvailableCourses.Count, Is.EqualTo(1));
        Assert.That(result.AvailableCourses.First().CourseId, Is.EqualTo(3));
        Assert.IsFalse(result.AvailableCourses.First().IsEnrolled);
    }
}
