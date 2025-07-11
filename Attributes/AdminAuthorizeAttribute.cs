using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AdmissionInfoSystem.Attributes
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Bạn cần đăng nhập để truy cập tài nguyên này" });
                return;
            }

            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            
            if (userRole != "admin")
            {
                context.Result = new ObjectResult(new { message = "Chỉ admin mới có quyền truy cập tài nguyên này" })
                {
                    StatusCode = 403
                };
            }
        }
    }
} 