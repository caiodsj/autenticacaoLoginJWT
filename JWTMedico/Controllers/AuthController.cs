using JWTMedico.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTMedico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static Medico medico = new Medico();

        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("registrar")]
        public ActionResult<Medico> Registrar(Medico request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);
            medico.CRM = request.CRM;
            medico.PasswordHash = passwordHash;
            medico.Email = request.Email;
            return Ok(medico);
        }

        [HttpPost("login")]
        public ActionResult<Medico> Login(MedicoDTO request)
        {
            if (medico.CRM != request.CRM) return BadRequest("Medico não encontrado.");
            if (!BCrypt.Net.BCrypt.Verify(request.Password, medico.PasswordHash)) return BadRequest("Senha incorreta.");
            string token = CreateToken(medico);
            return Ok(token);
        }

        private string CreateToken(Medico medico)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.SerialNumber, medico.CRM),
                new Claim(ClaimTypes.Email, medico.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
