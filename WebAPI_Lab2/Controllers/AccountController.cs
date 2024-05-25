using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_Lab2.Helpers;
using WebAPI_Lab2.Repository;

namespace WebAPI_Lab2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHelperRepository _helperRepository;

        public AccountController(IConfiguration configuration, IHelperRepository helperRepository)
        {
            _configuration = configuration;
            _helperRepository = helperRepository;
        }

        [HttpGet]
        public ActionResult Login(string username, string password)
        {
            // Retrieve JWT settings from appsettings.json
            var jwtSettings = _configuration.GetSection("JWT").Get<JwtSettings>();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return BadRequest("Username and password are required.");

            if (username == "islam ismail" && password == "123")
            {
                var userdata = new List<Claim>
                {
                    new Claim("Username", "islam"),
                    new Claim(ClaimTypes.MobilePhone, "01095042109")
                };

                var key = jwtSettings.Key;
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: userdata,
                    expires: DateTime.Now.AddDays(jwtSettings.DurationInDays),
                    signingCredentials: signingCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(tokenString);
            }
            else
                return Unauthorized("Invalid username or password.");
        }

        [Authorize]
        [HttpPost("Search")]
        public ActionResult Search(string keyword, string type)
        {
            if (string.IsNullOrEmpty(keyword))
                return BadRequest("Keyword is required.");

            switch (type.ToLower())
            {
                case "student":
                    var students = _helperRepository.SearchStudents(keyword);
                    return Ok(students);
                case "department":
                    var departments = _helperRepository.SearchDepartments(keyword);
                    return Ok(departments);
                default:
                    return BadRequest("Invalid search type. Supported types are 'student' and 'department'.");
            }
        }

    }
}
