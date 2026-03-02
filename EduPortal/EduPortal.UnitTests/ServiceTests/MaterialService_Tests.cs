using EduPortal.Data.Models.Entites;
using EduPortal.Logic.Services.MaterialService;
using EduPortal.Repo.CourseRepo;
using EduPortal.Repo.UserRepo;
using NSubstitute;

namespace EduPortal.Tests.Unit.ServiceTests;

public class MaterialService_Tests
{
    private ICourseRepository _courseRepoSub;
    private IUserRepository _userRepoSub;
    private IMaterialService _sut;

    [SetUp]
    public void Setup()
    {
        _courseRepoSub = Substitute.For<ICourseRepository>();
        _userRepoSub = Substitute.For<IUserRepository>();
        _sut = new MaterialService(_userRepoSub, _courseRepoSub);
    }

    [Category("Unit")]
    [Test]
    public async Task GetMaterialDetailsAsync_WhenMaterialIsVideo_MapsVideoPropertiesCorrectly()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var courseId = 1;
        var materialId = 100;

        var videoMaterial = new VideoMaterial
        {
            MaterialId = materialId,
            MaterialName = "Intro to C#",
            Duration = TimeSpan.FromMinutes(10),
            ResourceURL = "http://video.link",
            Quality = "1080p"
        };

        var course = new Course
        {
            CourseId = courseId,
            Materials = new List<Material> { videoMaterial }
        };

        _courseRepoSub.GetCourseWithDetailsByIdAsync(courseId).Returns(course);

        _userRepoSub.GetCompletedMaterialIdsForCourseAsync(userId, courseId).Returns(new HashSet<int>());

        // ACT
        var result = await _sut.GetMaterialDetailsAsync(userId, courseId, materialId);

        // ASSERT
        Assert.IsNotNull(result);
        Assert.That(result.DurationInSeconds, Is.EqualTo(600));
        Assert.That(result.ResourceUrl, Is.EqualTo("http://video.link"));
        Assert.IsFalse(result.IsCompleted);
    }

    [Category("Unit")]
    [Test]
    public async Task MarkMaterialAsCompletedAsync_WhenItIsTheLastMaterial_CompletesCourseAndUpgradesSkills()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var courseId = 1;
        var materialId = 2;

        var course = new Course
        {
            CourseId = courseId,
            Materials = new List<Material>
                {
                    new VideoMaterial { MaterialId = 1 },
                    new VideoMaterial { MaterialId = 2 }
                }
        };

        _courseRepoSub.GetCourseWithDetailsByIdAsync(courseId).Returns(course);

        _userRepoSub.GetCompletedMaterialIdsForCourseAsync(userId, courseId)
            .Returns(new HashSet<int> { 1, 2 });

        // ACT
        await _sut.MarkMaterialAsCompletedAsync(userId, courseId, materialId);

        // ASSERT
        await _userRepoSub.Received(1).MarkMaterialAsCompletedAsync(userId, materialId);

        await _userRepoSub.Received(1).MarkCourseCompletedAsync(userId, courseId);
        await _userRepoSub.Received(1).AddOrUpgradeSkillsFromCourseAsync(userId, courseId);
    }

    [Category("Unit")]
    [Test]
    public async Task MarkMaterialAsCompletedAsync_WhenNotTheLastMaterial_DoesNotCompleteCourse()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var courseId = 1;

        var course = new Course
        {
            Materials = new List<Material> { new VideoMaterial { MaterialId = 1 }, new VideoMaterial { MaterialId = 2 } }
        };

        _courseRepoSub.GetCourseWithDetailsByIdAsync(courseId).Returns(course);

        _userRepoSub.GetCompletedMaterialIdsForCourseAsync(userId, courseId)
            .Returns(new HashSet<int> { 1 });

        // ACT
        await _sut.MarkMaterialAsCompletedAsync(userId, courseId, 1);

        // ASSERT
        await _userRepoSub.Received(1).MarkMaterialAsCompletedAsync(userId, 1);

        await _userRepoSub.DidNotReceive().MarkCourseCompletedAsync(userId, courseId);
        await _userRepoSub.DidNotReceive().AddOrUpgradeSkillsFromCourseAsync(Arg.Any<Guid>(), Arg.Any<int>());
    }
}
