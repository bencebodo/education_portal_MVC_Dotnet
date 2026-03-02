using EduPortal.Logic.DTOs.DashboardDTOs;

namespace EduPortal.Logic.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardDataAsync(Guid userId);
        Task MarkEnrolledCourseAsync(Guid userId, int courseId);
    }
}
