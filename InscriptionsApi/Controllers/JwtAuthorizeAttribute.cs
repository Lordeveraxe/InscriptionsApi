using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Web.Http.Controllers;
using static System.Net.Mime.MediaTypeNames;

namespace InscriptionsApi.Controllers
{
    public class JwtAuthorizeAttribute
    {
        private readonly RequestDelegate _next;

        public JwtAuthorizeAttribute(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.ToString().Contains("/Users/ObtenerToken/"))
            {
                await _next(context);
                return;
                // Lógica específica para "/api/users"
            }
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                if (jwt.ValidTo < DateTime.UtcNow)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token has expired.");
                    await context.Response.CompleteAsync();
                    return;
                }
            }
            

            await _next(context);
        }
    }
}