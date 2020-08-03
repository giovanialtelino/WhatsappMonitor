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
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);

        Task<AuthenticateResponse> RefreshToken(int userId, string token);

        Task<bool> RevokeToken(int userId, string token);
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

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(e => e.Username == model.Username && e.Password == model.Password);

            if (user == null) return null;

            var jwtToken = generateJwtToken(user);

            var newRefreshToken = generateRefreshToken();

            var refreshTokens = await _context.RefreshTokens.Where(e => e.UserId == user.UserId).ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.MakeTokenInvalid();
            }

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public async Task<AuthenticateResponse> RefreshToken(int userId, string tokenString)
        {
            var user = await _context.Users.Include(r => r.RefreshToken).SingleOrDefaultAsync(u => u.UserId == userId);
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == tokenString && t.UserId == userId && t.Valid == true);

            if (user == null) return null;
            if (refreshToken == null) return null;

            var newRefreshToken = generateRefreshToken();

            foreach (var token in user.RefreshToken)
            {
                token.MakeTokenInvalid();
            }

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            var jwtToken = generateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public async Task<bool> RevokeToken(int userId, string tokenString)
        {
            var user = await _context.Users.Include(r => r.RefreshToken).SingleOrDefaultAsync(u => u.UserId == userId);
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == tokenString && t.UserId == userId && t.Valid == true);

            if (user == null) return false;
            if (refreshToken == null) return false;

            refreshToken.MakeTokenInvalid();
            await _context.SaveChangesAsync();

            return true;
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
                var newUser = new User(user.Username, user.Password);
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

        private RefreshToken generateRefreshToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rng.GetBytes(randomBytes);
                var token = Convert.ToBase64String(randomBytes);
                return new RefreshToken(token, DateTime.UtcNow);
            }
        }
    }
}
