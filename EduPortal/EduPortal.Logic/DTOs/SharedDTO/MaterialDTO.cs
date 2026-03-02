using EduPortal.Data.Models.Enum;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EduPortal.Logic.DTOs.SharedDTO
{
    public class MaterialDTO
    {
        [Range(0, int.MaxValue)]
        public int MaterialId { get; set; }
        public int CourseId { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string MaterialName { get; set; } = string.Empty;
        [Range(0, 12)]
        public int MaterialOrder { get; set; }
        public MaterialType MaterialType { get; set; }
        public bool IsCompleted { get; set; } = false;
        [DataType(DataType.Text)]
        public string? BookTitle { get; set; }
        [DataType(DataType.Text)]
        public string? Author { get; set; }
        [DataType(DataType.Text)]
        public string? FilePath { get; set; }
        public IFormFile? BookFile { get; set; }
        [DataType(DataType.Text)]
        public string? Format { get; set; }
        [DataType(DataType.Text)]
        public string? ResourceUrl { get; set; }
        [DataType(DataType.Text)]
        public string? Quality { get; set; }
        [Range(1, 1000)]
        public int? NumberOfPages { get; set; }
        [Range(0, int.MaxValue)]
        public int? PublicationYear { get; set; }
        [Range(0, 10000)]
        public double DurationInSeconds { get; set; } = 0;

        public TimeSpan Duration
        {
            get => TimeSpan.FromSeconds(DurationInSeconds);
            set => DurationInSeconds = value.TotalSeconds;
        }
    }
}
