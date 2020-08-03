using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<AuthenticateResponse>> Authenticate([FromBody] AuthenticateRequest auth)
        {
            var response = await _userService.Authenticate(auth);

            if (response == null) return BadRequest(new { message = "Password or user are invalid" });

            setTokenCookie(response.RefreshToken);
            return response;
        }

        [AllowAnonymous]
        [HttpPost("refresh/{id}")]
        public async Task<ActionResult<AuthenticateResponse>> Refresh(int id)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _userService.RefreshToken(id, refreshToken);

            if (response == null) return BadRequest(new { message = "Refresh token or user are invalid, login again" });

            setTokenCookie(response.RefreshToken);

            return response;
        }

        [HttpPost("revoke-token/{id}")]
        public async Task<ActionResult<string>> RevokeToken([FromBody] RevokeTokenRequest stringToken, int id)
        {
            if(string.IsNullOrWhiteSpace(stringToken.Token)) return BadRequest(new {message = "No refresh token provided"});

            var response = await _userService.RevokeToken(id, stringToken.Token);

            if (response == false) return BadRequest(new { message = "Error while revoking token" });
            return "Token revoked";
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthenticateResponse>> RegisterUser([FromBody] AuthenticateRequest user)
        {
            var registerUser = await _userService.RegisterUser(user);

            if (registerUser == null) return BadRequest(new { message = "There is already a user registered" });

            var newAuth = new AuthenticateRequest();
            newAuth.Username = registerUser.Username;
            newAuth.Password = registerUser.Password;

            var auth = await _userService.Authenticate(newAuth);

            return auth;
        }

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddYears(10)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}