using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InscriptionsApi.Models;
using InscriptionsApiLocal.Models;
using InscriptionsApi.Controllers.DTO;
using InscriptionsApi.Controllers.Encryption;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Policy;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Configuration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace InscriptionsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly InscriptionsUniversityContext _context;
        private IConfiguration _configuration = null;
        public UsersController(InscriptionsUniversityContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // GET: api/Users
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("{name}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserName(String name)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = _context.Users.FirstOrDefault(i => i.UserName == name);


            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("ObtenerToken/{name},{passwordUser}")]
        [AllowAnonymous]
        public async Task<ActionResult<FormatToken>> AuthenticationGetToken(String name, [DataType(DataType.Password)] string passwordUser)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            User user = _context.Users.FirstOrDefault(i => i.UserName == name);

            if (user == null)
            {
                return NotFound();
            }
            var credential = _context.Credentials.FirstOrDefault(i => i.UserId == user.UserId);

            if (credential == null)
            {
                return NotFound();
            }

            var hash = HashEncryption.CheckHash(passwordUser, credential.UserPassword, credential.CredentialSalt);
            if (!hash)
            {
                return Unauthorized();
            }
            if (user.UserState.Equals(0))
            {
                return Forbid();
            }

            var jwt = new JwtData
            {
                Key = Environment.GetEnvironmentVariable("Jwt_Key"),
                Issuer = Environment.GetEnvironmentVariable("Jwt_Issuer"),
                Audience = Environment.GetEnvironmentVariable("Jwt_Audience"),
                Subject = Environment.GetEnvironmentVariable("Jwt_Subject")
            };
            var claims = new[]
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id", user.UserId.ToString()),
                new Claim("Correo", user.UserEmail),
                new Claim("Nombre", user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes((jwt.Key)));
            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: singIn
                );
            var report = new FormatToken()
            {
                succes = true,
                message = "exito",
                token = new JwtSecurityTokenHandler().WriteToken(token)
            };
            return report;
        }


        [HttpPut("EditarDatos/{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(int id, PutUserDataDTO userDTO)
        {
            var credential = _context.Credentials.FirstOrDefault(i => i.UserId == id);
            if (credential == null)
            {
                return Unauthorized();
            }
            var hash = HashEncryption.CheckHash(userDTO.PreviousPassword, credential.UserPassword, credential.CredentialSalt);
            if (!hash)
            {
                return Unauthorized();
            }

            var user = _context.Users.FirstOrDefault(i => i.UserId == id);
            user.UserName = userDTO.UserName;
            user.UserEmail = userDTO.UserEmail;
            user.UserState = userDTO.UserState;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPut("EditarContraseña/{id}")]
        [Authorize]
        public async Task<IActionResult> PutUserpassword(int id, PutUserPasswordDTO userDTO)
        {
            var credential = _context.Credentials.FirstOrDefault(i => i.UserId == id);
            if (credential == null)
            {
                return NotFound();
            }
            var hash = HashEncryption.CheckHash(userDTO.PreviousPassword, credential.UserPassword, credential.CredentialSalt);
            if (!hash)
            {
                return Unauthorized();
            }

            HashedFormat hashNewPassword = HashEncryption.Hash(userDTO.NewPassword);

            credential.UserPassword = hashNewPassword.Password;
            credential.CredentialSalt = hashNewPassword.HashAlgorithm;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(PostUserDTO userDTO)
        {
            var user = new User()
            {
                UserName = userDTO.UserName,
                UserEmail = userDTO.UserEmail,
                UserState = userDTO.UserState,

            };
            if (_context.Users == null)
            {
                return Problem("Entity set 'InscriptionsUniversityContext.Users'  is null.");
            }
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            User userAux = _context.Users.
                FirstOrDefault(i => i.UserName == userDTO.UserName);

            HashedFormat hash = HashEncryption.Hash(userDTO.UserPassword);

            var credential = new Credential()
            {
                UserId = userAux.UserId,
                UserPassword = hash.Password,
                CredentialSalt = hash.HashAlgorithm
            };


            _context.Credentials.Add(credential);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // DELETE: api/Users/5
        /*   [HttpDelete("{id}")]
           public async Task<IActionResult> DeleteUser(int id)
           {
               if (_context.Users == null)
               {
                   return NotFound();
               }
               var user = await _context.Users.FindAsync(id);
               if (user == null)
               {
                   return NotFound();
               }

               _context.Users.Remove(user);
               await _context.SaveChangesAsync();

               return NoContent();
           }*/

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
