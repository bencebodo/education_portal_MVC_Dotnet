using EduPortal.Data.Models.Entites;
using EduPortal.Data.Models.Enum;
using EduPortal.Logic.Services.ProfileDetailsService;
using EduPortal.Repo.UserRepo;
using NSubstitute;

namespace EduPortal.Tests.Unit.ServiceTests;

public class ProfileDetailsService_Tests
{
    private IUserRepository _userRepositorySub;
    private IProfileDetailsService _sut;

    [SetUp]
    public void Setup()
    {
        _userRepositorySub = Substitute.For<IUserRepository>();

        _sut = new ProfileDetailsService(_userRepositorySub);
    }

    [Category("Unit")]
    [Test]
    public async Task GetProfileDetailsAsync_WhenUserExists_MapsAndReturnsCorrectDTO()
    {
        // Arrange:
        var userId = Guid.NewGuid();

        var expectedUser = new User
        {
            Id = userId,
            FirstName = "Bence",
            LastName = "Bodó",
            Email = "bence@test.com",
            Skills = new List<Skill>
                {
                    new Skill { SkillName = "C#", SkillLevel = SkillLevel.Advanced },
                    new Skill { SkillName = "C#", SkillLevel = SkillLevel.Beginner }
                }
        };

        var enrolledCourses = new List<Course>
            {
                new Course { CourseId = 1, Name = "C# Basics" },
                new Course { CourseId = 2, Name = "Playwright E2E" }
            };

        var completedCourses = new List<Course>
            {
                new Course { CourseId = 1, IsCompleted = true }
            };

        _userRepositorySub.GetUserWithAllDetailsAsync(userId).Returns(expectedUser);
        _userRepositorySub.GetEnrolledCoursesByUserIdAsync(userId).Returns(enrolledCourses);

        // ACT
        var result = await _sut.GetProfileDetailsAsync(userId);

        // ASSERT
        Assert.IsNotNull(result);
        Assert.That(result.FirstName, Is.EqualTo("Bence"));

        Assert.That(result.SkillsWithLevel.Count, Is.EqualTo(1));
        Assert.That(result.SkillsWithLevel["C#"], Is.EqualTo(SkillLevel.Advanced));
    }

    [Category("Unit")]
    [Test]
    public void GetProfileDetailsAsync_WhenUserDoesNotExist_ThrowsNullReferenceException()
    {
        //Arrange
        var userId = Guid.NewGuid();

        _userRepositorySub.GetUserWithAllDetailsAsync(userId).Returns((User)null);

        // Act & Assert

        Assert.ThrowsAsync<NullReferenceException>(async () => await _sut.GetProfileDetailsAsync(userId));
    }
}
