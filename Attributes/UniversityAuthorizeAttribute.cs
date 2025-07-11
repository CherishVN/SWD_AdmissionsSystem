using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AdmissionInfoSystem.Attributes
{
    public class UniversityAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
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
            
            // Debug: Log tất cả claims
            var logger = context.HttpContext.RequestServices.GetService<ILogger<UniversityAuthorizeAttribute>>();
            logger?.LogInformation("User claims:");
            foreach (var claim in user.Claims)
            {
                logger?.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }
            logger?.LogInformation("Role found: {Role}", userRole ?? "NULL");
            
            if (userRole != "university")
            {
                context.Result = new ObjectResult(new { 
                    message = "Chỉ tài khoản trường đại học mới có quyền truy cập tài nguyên này",
                    debug = $"Current role: {userRole ?? "NULL"}"
                })
                {
                    StatusCode = 403
                };
            }
        }
    }
} 