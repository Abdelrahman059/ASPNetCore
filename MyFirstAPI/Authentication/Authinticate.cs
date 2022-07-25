using DataAccess.Models;
using DataAccess.Repositoires;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace blogWebAPI.Authentication
{

    public class Authinticate
    {



        private readonly IUserRepository<Author> authorRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IConfiguration Configuration { get; }

        public Authinticate(IUserRepository<Author> authorRepository, IHttpContextAccessor httpContextAccessor, IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            this.authorRepository = authorRepository;
            this._httpContextAccessor = httpContextAccessor;
        }
        //public Authinticate(string key)
        //{
        //    this.key = key;
        //}

        public string Authintication(string userEmail, string password)
        {

            // Author user = authorRepository.Find(userEmail);
            if (authorRepository.Find(userEmail)?.UserEmail == null || authorRepository.Find(userEmail)?.Password != password)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("key"));
            var tokrnDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name ,userEmail)
                }),
                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
            };
           
            var token = tokenHandler.CreateToken(tokrnDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public string Decode(string JwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(JwtToken);
            var NeededEmail = token.Claims.First(claim => claim.Type == "unique_Value").Value;
            return NeededEmail;
        }

    }
}
