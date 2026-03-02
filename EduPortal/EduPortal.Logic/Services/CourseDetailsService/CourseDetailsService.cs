using EduPortal.Data.Models.Entites;
using EduPortal.Data.Models.Enum;
using EduPortal.Logic.DTOs.CourseDetailsDTOs;
using EduPortal.Logic.DTOs.SharedDTO;
using EduPortal.Repo.CourseRepo;
using EduPortal.Repo.MaterialRepo;
using EduPortal.Repo.SkillRepo;
using EduPortal.Repo.UserRepo;
using Microsoft.AspNetCore.Hosting;

namespace EduPortal.Logic.Services.CourseDetailsService
{
    public class CourseDetailsService : ICourseDetailsService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IMaterialRepository _materialRepository;


        public CourseDetailsService(IUserRepository userRepo, ICourseRepository courseRepository, ISkillRepository skillRepository, IMaterialRepository materialRepository, IWebHostEnvironment webHostEnvironment)
        {
            _userRepository = userRepo;
            _courseRepository = courseRepository;
            _skillRepository = skillRepository;
            _materialRepository = materialRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<CourseDetailsDTO> GetCourseDetailsAsync(Guid userId, int courseId)
        {
            var course = await _courseRepository.GetCourseWithDetailsByIdAsync(courseId);

            var courseSkills = MapSkillsToDTOs(course?.Skills ?? new List<Skill>());

            var completedByUser = await _userRepository.GetCompletedMaterialIdsForCourseAsync(userId, courseId) ?? new HashSet<int>();

            return MapCourseEntityToDTO(course, completedByUser);
        }

        public async Task<CourseDetailsDTO> AddOrUpdateCourseDetailsAsync(CourseDetailsDTO dto, Guid userId)
        {
            var completedMaterialIds = await _userRepository.GetCompletedMaterialIdsForCourseAsync(userId, dto.CourseId) ?? new HashSet<int>();

            var course = MapCourseDTOToEntity(dto, userId, completedMaterialIds);

            var savedCourse = await _courseRepository.AddOrUpdateCourseAsync(course);

            var updatedDTO = MapCourseEntityToDTO(savedCourse, completedMaterialIds);

            return updatedDTO;
        }

        public async Task<bool> DeleteCourseDetailsAsync(CourseDetailsDTO dto, Guid creatorId)
        {
            var course = await _courseRepository.GetCourseWithDetailsByIdAsync(dto.CourseId);

            if (course == null || course.CreatorId != creatorId)
            {
                return false;
            }

            return await _courseRepository.DeleteAsync(course);
        }

        public async Task<List<SkillDTO>> GetAllSkillDTOsAsync()
        {
            var skills = await _skillRepository.GetAllAsync();

            return skills.Select(s => new SkillDTO
            {
                SkillId = s.SkillId,
                SkillName = s.SkillName
            }).ToList();
        }

        public async Task<List<MaterialDTO>> GetAllMaterialDTOsAsync()
        {
            var materials = await _materialRepository.GetAllAsync();
            return MapMaterialsToDTOs(materials.ToList(), new HashSet<int>());
        }

        public async Task<bool> SaveBookFileAsync(MaterialDTO dto)
        {
            if (dto.BookFile == null || dto.BookFile.Length == 0)
                return false;

            var extension = Path.GetExtension(dto.BookFile.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "books");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.BookFile.CopyToAsync(stream);
            }

            dto.FilePath = $"/uploads/books/{uniqueFileName}";
            dto.Format = extension.TrimStart('.').ToUpper();

            string[] supportedFormats = { "PDF", "EPUB", "DOCX", "DOC", "TXT" };

            if (!supportedFormats.Contains(dto.Format))
                return false;

            return true;
        }

        private static Course MapCourseDTOToEntity(CourseDetailsDTO dto, Guid userId, HashSet<int> completedMaterialIds)
        {
            return new Course
            {
                CourseId = dto.CourseId,
                Name = dto.CourseName,
                Description = dto.CourseDescription,
                CreatorId = userId,
                Skills = dto.Skills.Select(MapSkillDTOToEntity).ToList(),
                Materials = dto.Materials.Select(MapMaterialDTOToEntity).ToList(),
                IsCompleted = CalculateProgress(dto.Materials, completedMaterialIds) == 100
            };
        }

        private static CourseDetailsDTO MapCourseEntityToDTO(Course course, HashSet<int> completedMaterialIds)
        {
            return new CourseDetailsDTO
            {
                CourseId = course.CourseId,
                CourseName = course.Name,
                CourseDescription = course.Description,
                CreatorId = course.CreatorId,
                CreatorName = course.Creator?.FirstName + " " + course.Creator?.LastName ?? "Unknown",
                Skills = MapSkillsToDTOs(course.Skills),
                Materials = MapMaterialsToDTOs(course.Materials, completedMaterialIds),
                OverallProgressPercentage = CalculateProgress(MapMaterialsToDTOs(course.Materials, completedMaterialIds), completedMaterialIds),
                AcquirableSkills = course.Skills.Select(s => s.SkillName).ToList()
            };
        }

        private static Skill MapSkillDTOToEntity(SkillDTO dto)
        {
            return new Skill
            {
                SkillId = dto.SkillId,
                SkillName = dto.SkillName,
            };
        }

        private static List<SkillDTO> MapSkillsToDTOs(ICollection<Skill> skills)
        {
            return skills
                .Select(s => new SkillDTO
                {
                    SkillId = s.SkillId,
                    SkillName = s.SkillName,
                })
                .ToList();
        }

        private static List<MaterialDTO> MapMaterialsToDTOs(ICollection<Material> materials, HashSet<int> completedByUser)
        {
            return materials.OrderBy(m => m.MaterialOrder)
                .Select(m => m switch
            {
                BookMaterial book => new MaterialDTO
                {
                    MaterialId = book.MaterialId,
                    MaterialName = book.MaterialName,
                    MaterialOrder = book.MaterialOrder,
                    MaterialType = book.MaterialType,
                    IsCompleted = completedByUser.Contains(book.MaterialId),
                    BookTitle = book.Title,
                    Author = book.Author,
                    FilePath = book.FilePath,
                    Format = book.Format,
                    NumberOfPages = book.NumberOfPages,
                    PublicationYear = book.PublicationYear
                },
                ArticleMaterial article => new MaterialDTO
                {
                    MaterialId = article.MaterialId,
                    MaterialName = article.MaterialName,
                    MaterialOrder = article.MaterialOrder,
                    MaterialType = article.MaterialType,
                    IsCompleted = completedByUser.Contains(article.MaterialId),
                    PublicationYear = article.PublicationYear,
                    ResourceUrl = article.ResourceURL
                },
                VideoMaterial video => new MaterialDTO
                {
                    MaterialId = video.MaterialId,
                    MaterialName = video.MaterialName,
                    MaterialOrder = video.MaterialOrder,
                    MaterialType = video.MaterialType,
                    IsCompleted = completedByUser.Contains(video.MaterialId),
                    ResourceUrl = video.ResourceURL,
                    Duration = video.Duration,
                    Quality = video.Quality
                },
                _ => new MaterialDTO
                {
                    MaterialId = m.MaterialId,
                    MaterialName = m.MaterialName,
                    MaterialOrder = m.MaterialOrder,
                    MaterialType = m.MaterialType,
                    IsCompleted = completedByUser.Contains(m.MaterialId)
                }
            })
                .ToList();
        }
        private static Material MapMaterialDTOToEntity(MaterialDTO dto)
        {
            return dto.MaterialType switch
            {
                MaterialType.Book => new BookMaterial
                {
                    MaterialId = dto.MaterialId,
                    MaterialName = dto.MaterialName,
                    MaterialOrder = dto.MaterialOrder,
                    MaterialType = dto.MaterialType,
                    Title = dto.BookTitle ?? "",
                    Author = dto.Author ?? "",
                    FilePath = dto.FilePath ?? "",
                    Format = dto.Format ?? "",
                    NumberOfPages = dto.NumberOfPages ?? 0,
                    PublicationYear = dto.PublicationYear ?? 1901
                },
                MaterialType.Article => new ArticleMaterial
                {
                    MaterialId = dto.MaterialId,
                    MaterialName = dto.MaterialName,
                    MaterialOrder = dto.MaterialOrder,
                    MaterialType = dto.MaterialType,
                    ResourceURL = dto.ResourceUrl ?? "",
                    PublicationYear = dto.PublicationYear ?? 1901
                },
                MaterialType.Video => new VideoMaterial
                {

                    MaterialId = dto.MaterialId,
                    MaterialName = dto.MaterialName,
                    MaterialOrder = dto.MaterialOrder,
                    MaterialType = dto.MaterialType,
                    Duration = dto.Duration,
                    ResourceURL = dto.ResourceUrl ?? "",
                    Quality = dto.Quality ?? ""
                },
            };
        }

        private static double CalculateProgress(List<MaterialDTO> materialDtos, HashSet<int> completedMaterialIds)
        {
            int total = materialDtos.Count;
            if (total == 0) return 0;

            int completed = completedMaterialIds.Count;
            return completed / (double)total * 100;
        }
    }
}
