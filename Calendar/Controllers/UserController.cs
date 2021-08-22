using Calendar.Auth;
using Calendar.EF;
using Calendar.EF.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationContext Db;
        private readonly IOptions<AuthOption> authOption;
        public UserController(ApplicationContext context, IOptions<AuthOption> authOption)
        {
            this.Db = context;
            this.authOption = authOption;
        }


        [HttpPost("registration")]
        [AllowAnonymous]
        public IActionResult Registration([FromBody] UserModel model)
        {

            var NewUser = new User
            {
                Password = HeshPassword(model.Password),
                Nickname = model.Nickname,
            };
           

            if (Db.Users.FirstOrDefault(x=>x.Nickname==NewUser.Nickname)==null)
            {
                Db.Users.Add(NewUser);
                Db.SaveChanges();

                return Login(model);
            }
            else
            {
                return BadRequest("User with this Emailalready exists.");
            }
        }

        [HttpPost("cheknickname")]
        [AllowAnonymous]
        async public Task<IActionResult> CheckNewNickname([FromBody] String newNickname)
        {
            bool state = !await Db.Users.AnyAsync(x => x.Nickname == newNickname);
            return Ok(new { original = state });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] UserModel model)
        {

            var identity = GetIdentity(model.Nickname, HeshPassword(model.Password));

            if (identity == null)
            {
                return Unauthorized();
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var authOptions = this.authOption.Value;

            var jwt = new JwtSecurityToken(
                    issuer: authOptions.Issuer,
                    audience: authOptions.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromHours(authOptions.TokenLifetime)),
                    signingCredentials: new SigningCredentials(authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);


            // сериализация ответа
            Response.ContentType = "application/json";

            Console.WriteLine("Login succeed");
            return Ok(new { token = encodedJwt });
        }

        [HttpPost("deleteuser")]
        async public Task<IActionResult> DeleteUser()
        {
            var Userid = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
            var DeleteвUser = await Db.Users.FirstOrDefaultAsync(x => x.Id == Userid);
            Db.Entry(DeleteвUser).State= EntityState.Deleted;
            await Db.SaveChangesAsync();
            return Ok();
        }

        private ClaimsIdentity GetIdentity(string nickname, string password)// тип ClaimsIdentity
        {

            var user = Db.Users.FirstOrDefault(x=>x.Password == password && x.Nickname == nickname);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Nickname),
                    new Claim("Id", user.Id.ToString()),
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
        
        private string HeshPassword(string password)
        {
            byte[] arrInput = new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (int i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
    }
}
