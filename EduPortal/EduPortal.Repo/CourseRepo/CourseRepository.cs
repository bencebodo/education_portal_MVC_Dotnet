using EduPortal.Data.EPDbContext;
using EduPortal.Data.Models.Entites;
using EduPortal.Repo.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace EduPortal.Repo.CourseRepo
{
    public class CourseRepository(EduPortalDbContext context) : EFBaseRepository<Course>(context), ICourseRepository
    {

        public async Task<IEnumerable<Course>> GetCoursesByCreatorIdAsync(Guid creatorId)
        {
            return await GetDbContext().Courses
                .Where(c => c.CreatorId == creatorId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetAllCoursesWithDetailsAsync()
        {
            return await GetDbContext().Courses
                .Include(c => c.Materials)
                .Include(c => c.Skills)
                .Include(c => c.Creator)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseWithDetailsByIdAsync(int courseId)
        {
            var course = await GetDbContext().Courses
                .Include(c => c.Skills)
                .Include(c => c.Creator)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course != null)
            {
                course.Materials = await GetDbContext().Entry(course)
                    .Collection(c => c.Materials)
                    .Query()
                    .OrderBy(m => m.MaterialOrder)
                    .ToListAsync();
            }

            return course;
        }

        public async Task<IEnumerable<Course>> GetCoursesBySkillAsync(int skillId)
        {
            return await GetDbContext().Courses
                .Where(c => c.Skills.Any(s => s.SkillId == skillId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> SearchCoursesAsync(string keyword)
        {
            var lowerKeyword = keyword.ToLower();
            return await GetDbContext().Courses
                .Where(c => (c.Name != null && c.Name.ToLower().Contains(lowerKeyword)) ||
                            (c.Description != null && c.Description.ToLower().Contains(lowerKeyword)))
                .ToListAsync();
        }
        public async Task<Course> AddOrUpdateCourseAsync(Course course)
        {
            var db = GetDbContext();
            var existingCourse = await GetCourseWithDetailsByIdAsync(course.CourseId);

            if (existingCourse == null)
            {
                existingCourse = new Course();
                db.Courses.Add(existingCourse);
            }

            existingCourse.Name = course.Name;
            existingCourse.Description = course.Description;
            existingCourse.CreatorId = course.CreatorId;

            await UpdateCourseSkills(existingCourse, course.Skills);
            await UpdateCourseMaterials(existingCourse, course.Materials);

            await SaveAsync();
            return existingCourse;
        }


        private async Task UpdateCourseSkills(Course existingCourse, ICollection<Skill> skills)
        {
            var db = GetDbContext();

            var skillIds = skills.Where(s => s.SkillId > 0).Select(s => s.SkillId).ToHashSet();
            var skillNames = skills.Select(s => s.SkillName).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existingSkills = await db.Skills
                .Where(s => skillIds.Contains(s.SkillId) || skillNames.Contains(s.SkillName))
                .ToListAsync();

            var finalSkills = skills.Select(dto =>
            {
                var match = existingSkills.FirstOrDefault(s => s.SkillId == dto.SkillId || s.SkillName.Equals(dto.SkillName, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    return match;
                }

                var newSkill = new Skill
                {
                    SkillName = dto.SkillName,
                };
                db.Skills.Add(newSkill);
                return newSkill;
            }).ToList();

            existingCourse.Skills ??= new List<Skill>();
            existingCourse.Skills.Clear();

            foreach (var skill in finalSkills)
            {
                existingCourse.Skills.Add(skill);
            }
        }

        private async Task UpdateCourseMaterials(Course existingCourse, ICollection<Material> materials)
        {
            var db = GetDbContext();

            var materialIds = materials.Where(m => m.MaterialId > 0).Select(m => m.MaterialId).ToHashSet();
            var materialNames = materials.Select(m => m.MaterialName).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existingMaterials = await db.Materials
                .Where(m => materialIds.Contains(m.MaterialId) || materialNames.Contains(m.MaterialName))
                .ToListAsync();

            var finalMaterials = materials.Select(dto =>
            {
                var match = existingMaterials.FirstOrDefault(m => m.MaterialId == dto.MaterialId || m.MaterialName.Equals(dto.MaterialName, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    return MapExistingMaterialToEntity(match, dto);
                }

                var newMaterial = MapNewMaterialToEntity(dto);
                return newMaterial;
            }).ToList();

            existingCourse.Materials ??= new List<Material>();
            existingCourse.Materials.Clear();

            foreach (var material in finalMaterials)
            {
                existingCourse.Materials.Add(material);
            }
        }

        private Material MapExistingMaterialToEntity(Material existingMaterial, Material material)
        {
            existingMaterial.MaterialName = material.MaterialName;
            existingMaterial.MaterialOrder = material.MaterialOrder;
            switch (existingMaterial)
            {
                case BookMaterial bookEntity when material is BookMaterial bookDto:
                    bookEntity.Title = bookDto.Title;
                    bookEntity.Author = bookDto.Author;
                    bookEntity.NumberOfPages = bookDto.NumberOfPages;
                    bookEntity.PublicationYear = bookDto.PublicationYear;
                    bookEntity.FilePath = bookDto.FilePath;
                    bookEntity.Format = bookDto.Format;
                    break;
                case ArticleMaterial articleEntity when material is ArticleMaterial articleDto:
                    articleEntity.PublicationYear = articleDto.PublicationYear;
                    articleEntity.ResourceURL = articleDto.ResourceURL;
                    break;
                case VideoMaterial videoEntity when material is VideoMaterial videoDto:
                    videoEntity.ResourceURL = videoDto.ResourceURL;
                    videoEntity.Duration = videoDto.Duration;
                    videoEntity.Quality = videoDto.Quality;
                    break;
            }
            return existingMaterial;
        }
        private Material MapNewMaterialToEntity(Material material)
        {
            switch (material)
            {
                case BookMaterial book:
                    return new BookMaterial
                    {
                        MaterialName = book.MaterialName,
                        MaterialOrder = book.MaterialOrder,
                        MaterialType = book.MaterialType,
                        Title = book.Title,
                        Author = book.Author,
                        FilePath = book.FilePath,
                        Format = book.Format,
                        NumberOfPages = book.NumberOfPages,
                        PublicationYear = book.PublicationYear
                    };
                case ArticleMaterial article:
                    return new ArticleMaterial
                    {
                        MaterialName = article.MaterialName,
                        MaterialOrder = article.MaterialOrder,
                        MaterialType = article.MaterialType,
                        PublicationYear = article.PublicationYear,
                        ResourceURL = article.ResourceURL
                    };
                case VideoMaterial video:
                    return new VideoMaterial
                    {
                        MaterialName = video.MaterialName,
                        MaterialOrder = video.MaterialOrder,
                        MaterialType = video.MaterialType,
                        ResourceURL = video.ResourceURL,
                        Duration = video.Duration,
                        Quality = video.Quality
                    };
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
