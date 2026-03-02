using EduPortal.Data.Models.Entites;
using EduPortal.Data.Models.Enum;
using EduPortal.Logic.DTOs.CourseDetailsDTOs;
using EduPortal.Logic.DTOs.SharedDTO;
using EduPortal.Logic.Services.CourseDetailsService;
using EduPortal.Repo.CourseRepo;
using EduPortal.Repo.MaterialRepo;
using EduPortal.Repo.SkillRepo;
using EduPortal.Repo.UserRepo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace EduPortal.Tests.Unit.ServiceTests
{
    [TestFixture]
    public class CourseDetailsServiceTests
    {
        private IUserRepository _userRepoSub;
        private ICourseRepository _courseRepoSub;
        private ISkillRepository _skillRepoSub;
        private IMaterialRepository _materialRepoSub;
        private IWebHostEnvironment _envSub;
        private CourseDetailsService _sut;

        [SetUp]
        public void Setup()
        {
            _userRepoSub = Substitute.For<IUserRepository>();
            _courseRepoSub = Substitute.For<ICourseRepository>();
            _skillRepoSub = Substitute.For<ISkillRepository>();
            _materialRepoSub = Substitute.For<IMaterialRepository>();
            _envSub = Substitute.For<IWebHostEnvironment>();

            _sut = new CourseDetailsService(
                _userRepoSub, _courseRepoSub, _skillRepoSub, _materialRepoSub, _envSub);
        }

        [Category("Unit")]
        [Test]
        public async Task GetCourseDetailsAsync_WhenCourseExists_ReturnsCorrectlyMappedDTO()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var courseId = 1;

            var course = new Course
            {
                CourseId = courseId,
                Name = "Mastering NSubstitute",
                CreatorId = Guid.NewGuid(),
                Skills = new List<Skill> { new Skill { SkillId = 1, SkillName = "Testing" } },
                Materials = new List<Material>
                {
                    new VideoMaterial { MaterialId = 10, MaterialType = MaterialType.Video },
                    new BookMaterial { MaterialId = 20, MaterialType = MaterialType.Book }
                }
            };

            _courseRepoSub.GetCourseWithDetailsByIdAsync(courseId).Returns(course);

            var completedMaterials = new HashSet<int> { 10 };
            _userRepoSub.GetCompletedMaterialIdsForCourseAsync(userId, courseId).Returns(completedMaterials);

            // ACT
            var result = await _sut.GetCourseDetailsAsync(userId, courseId);

            // ASSERT
            Assert.IsNotNull(result);
            Assert.That(result.CourseName, Is.EqualTo("Mastering NSubstitute"));
            Assert.That(result.Skills.Count, Is.EqualTo(1));
            Assert.That(result.Materials.Count, Is.EqualTo(2));

            Assert.That(result.OverallProgressPercentage, Is.EqualTo(50.0));
        }

        [Category("Unit")]
        [Test]
        public async Task DeleteCourseDetailsAsync_WhenUserIsNotTheCreator_ReturnsFalseAndDoesNotDelete()
        {
            // ARRANGE
            var actualCreatorId = Guid.NewGuid();
            var hackerUserId = Guid.NewGuid();

            var dto = new CourseDetailsDTO { CourseId = 99 };
            var course = new Course { CourseId = 99, CreatorId = actualCreatorId };

            _courseRepoSub.GetCourseWithDetailsByIdAsync(dto.CourseId).Returns(course);

            // ACT
            var result = await _sut.DeleteCourseDetailsAsync(dto, hackerUserId);

            // ASSERT
            Assert.IsFalse(result);

            await _courseRepoSub.DidNotReceive().DeleteAsync(Arg.Any<Course>());
        }

        [Category("Unit")]
        [Test]
        public async Task SaveBookFileAsync_WhenFileExtensionIsUnsupported_ReturnsFalse()
        {
            // ARRANGE
            var fakeFile = Substitute.For<IFormFile>();
            fakeFile.Length.Returns(1024);
            fakeFile.FileName.Returns("malware.exe");

            var dto = new MaterialDTO { BookFile = fakeFile };

            _envSub.WebRootPath.Returns("C:\\TestPath");

            // ACT
            var result = await _sut.SaveBookFileAsync(dto);

            // ASSERT
            Assert.IsFalse(result);
        }
    }
}