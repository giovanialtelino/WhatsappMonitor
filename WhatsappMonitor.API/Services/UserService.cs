using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WhatsappMonitor.Shared.Models;
using WhatsappMonitor.Shared.Models.AuthAuto;
using WhatsappMonitor.API.Context;
using WhatsappMonitor.API.Helpers;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WhatsappMonitor.API.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);

        Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);

        Task<bool> RevokeToken(string token, string ipAddress);
        Task<List<User>> GetAll();
        Task<User> GetById(int id);

        Task<User> RegisterUser(AuthenticateRequest user);
    }

    public class UserService : IUserService
    {
        private MyDbContext _context;
        private readonly AppSettings _appSettings;

        public UserService(MyDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var user = await _context.Users.SingleOrDefaultAsync(e => e.Username == model.Username && e.Password == model.Password);

            if (user == null) return null;

            var jwtToken = generateJwtToken(user);
            var refreshToken = generateRefreshToken(ipAddress);

            user.RefreshToken = refreshToken;
            await _context.SaveChangesAsync();

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken.Token == token);

            if (user == null) return null;

            var refreshToken = user.RefreshToken;

            if (!refreshToken.IsActive) return null;

            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.Now;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshToken = newRefreshToken;
            await _context.SaveChangesAsync();

            var jwtToken = generateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public async Task<bool> RevokeToken(string token, string ipAddress)
        {
            var user = await _context.Users.SingleOrDefaultAsync(e => e.RefreshToken.Token == token);

            if (user == null) return false;

            var refreshToken = user.RefreshToken;

            if (!refreshToken.IsActive) return false;

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        private async Task<bool> AllowNewUser()
        {
            var counter = await _context.Users.CountAsync();

            if (counter > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<User> RegisterUser(AuthenticateRequest user)
        {
            if (await AllowNewUser() == true)
            {
                var newUser = new User();
                newUser.Password = user.Password;
                newUser.Username = user.Username;
                newUser.UserId = 0;

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return newUser;
            }
            else
            {
                return null;
            }
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.UserId == id);
        }

        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rng.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddYears(10),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}