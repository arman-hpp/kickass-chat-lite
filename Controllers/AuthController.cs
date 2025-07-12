using System;
using KickassChat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Linq;

namespace KickassChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("[action]")]
        public IActionResult Login([FromBody] LoginInputModel request)
        {
            if(string.IsNullOrEmpty(request?.UserName))
                return BadRequest("UserName is empty.");

            if (string.IsNullOrEmpty(request.RoomName))
                return BadRequest("RoomName is empty.");
            
            var rawToken = $"{request.UserName}::{request.RoomName}::{Guid.NewGuid()}";
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawToken));

            return Ok(new { token });
        }

        [HttpGet("[action]")]
        public IActionResult GetMe()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var parts = decoded.Split("::");

                if (parts.Length != 3)
                    return Unauthorized();

                var userName = parts[0];
                var roomName = parts[1];
                var userId = parts[2];

                return Ok(new { userName, roomName, userId });
            }
            catch
            {
                return Unauthorized();
            }
        }
    }
}
