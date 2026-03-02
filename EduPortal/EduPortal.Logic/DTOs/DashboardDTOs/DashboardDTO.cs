using EduPortal.Logic.DTOs.SharedDTO;

namespace EduPortal.Logic.DTOs.DashboardDTOs
{
    public class DashboardDTO
    {
        public DashboardProfileDTO? DashboardProfileDTO { get; set; }
        public IReadOnlyList<CourseListItemDTO>? EnrolledCourses { get; set; }
        public IReadOnlyList<CourseListItemDTO>? CompletedCourses { get; set; }
        public IReadOnlyList<CourseListItemDTO>? AvailableCourses { get; set; }
    }
}
