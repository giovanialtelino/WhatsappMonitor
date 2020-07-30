using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatsappMonitor.Shared.Models;
using WhatsappMonitor.API.Context;
using Microsoft.EntityFrameworkCore;
using WhatsappMonitor.API.Repository;
using Microsoft.AspNetCore.Authorization;
using WhatsappMonitor.API.Services;
using WhatsappMonitor.Shared.Models.AuthAuto;

namespace WhatsappMonitor.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : Controller
    {
        private IUserService _userService;
        public UsersController(IUserService userService)
        {
            this._userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<AuthenticateResponse> Authenticate([FromBody] AuthenticateRequest auth)
        {
            var response = await _userService.Authenticate(auth, ipAddress());

            setTokenCookie(response.RefreshToken);
            return response;
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<AuthenticateResponse> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _userService.RefreshToken(refreshToken, ipAddress());

            setTokenCookie(response.RefreshToken);

            return response;
        }

        [HttpPost("revoke-token")]
        public async Task<string> RevokeToken([FromBody] RevokeTokenRequest revoke)
        {
            var token = revoke.Token ?? Request.Cookies["refreshToken"];

            var response = await _userService.RevokeToken(token, ipAddress());

            if (response)
            {
                return "Token revoked";
            }
            else
            {
                return "Error while revoking token";
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<AuthenticateResponse> RegisterUser([FromBody] AuthenticateRequest user)
        {
            var registerUser = await _userService.RegisterUser(user);

            var newAuth = new AuthenticateRequest();
            newAuth.Username = registerUser.Username;
            newAuth.Password = registerUser.Password;

            var auth = await _userService.Authenticate(newAuth, ipAddress());

            return auth;

        }

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }
}