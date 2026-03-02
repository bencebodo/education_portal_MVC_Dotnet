using EduPortal.WebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;
using System.Security.Claims;

namespace EduPortal.WebMVC.Filters
{
    public class RequireUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var _))
            {
                context.Result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = new ViewDataDictionary<ErrorViewModel>(
                        context.Controller is Controller c ? c.ViewData : new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState),
                        new ErrorViewModel
                        {
                            RequestId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier,
                        })
                };
            }

            base.OnActionExecuting(context);
        }
    }
}
