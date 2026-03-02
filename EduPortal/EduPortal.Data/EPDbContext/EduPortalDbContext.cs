using EduPortal.Data.Models.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduPortal.Data.EPDbContext
{
    public class EduPortalDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public EduPortalDbContext(DbContextOptions<EduPortalDbContext> options)
            : base(options)
        {
        }

        public DbSet<Skill> Skills { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Material> Materials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Creator)
                .WithMany()
                .HasForeignKey(c => c.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Skills)
                .WithMany(s => s.Courses)
                .UsingEntity<Dictionary<string, object>>(
                "CourseSkills",
                    j => j.HasOne<Skill>()
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .HasConstraintName("FK_CourseSkills_Skills_SkillsSkillId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Course>()
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .HasConstraintName("FK_CourseSkills_Courses_CoursesCourseId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("CourseId", "SkillId");
                    });

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Materials)
                .WithMany(m => m.Courses)
                .UsingEntity<Dictionary<string, object>>(
                "CourseMaterials",
                    j => j.HasOne<Material>()
                        .WithMany()
                        .HasForeignKey("MaterialId")
                        .HasConstraintName("FK_CourseMaterials_Materials_MaterialId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Course>()
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .HasConstraintName("FK_CourseMaterials_Courses_CourseId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("CourseId", "MaterialId");
                    });
            modelBuilder.Entity<User>()
                .HasMany(u => u.Courses)
                .WithMany(c => c.Users)
                .UsingEntity<Dictionary<string, object>>(
                "UserCourses",
                    j => j.HasOne<Course>()
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .HasConstraintName("FK_UserCourses_Courses_CourseId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<User>()
                        .WithMany()
                        .HasForeignKey("Id")
                        .HasConstraintName("FK_UserCourses_Users_UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("Id", "CourseId");
                    });

            modelBuilder.Entity<User>()
                .HasMany(u => u.Materials)
                .WithMany(m => m.Users)
                .UsingEntity<Dictionary<string, object>>(
                "UserMaterials",
                    j => j.HasOne<Material>()
                        .WithMany()
                        .HasForeignKey("MaterialId")
                        .HasConstraintName("FK_UserMaterials_Materials_MaterialId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<User>()
                        .WithMany()
                        .HasForeignKey("Id")
                        .HasConstraintName("FK_UserMaterials_Users_UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("Id", "MaterialId");
                    });

            modelBuilder.Entity<User>()
                .HasMany(u => u.Skills)
                .WithMany(s => s.Users)
                .UsingEntity<Dictionary<string, object>>(
                "UserSkills",
                    j => j.HasOne<Skill>()
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .HasConstraintName("FK_UserSkills_Skills_SkillId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<User>()
                        .WithMany()
                        .HasForeignKey("Id")
                        .HasConstraintName("FK_UserSkills_Users_UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("Id", "SkillId");
                    });

            modelBuilder.Entity<ArticleMaterial>()
                .ToTable("OnlineMaterials");

            modelBuilder.Entity<VideoMaterial>()
                .ToTable("VideoMaterials");

            modelBuilder.Entity<BookMaterial>()
                .ToTable("BookMaterials");
        }
    }
}
