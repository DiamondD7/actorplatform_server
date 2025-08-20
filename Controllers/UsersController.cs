using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using UserAPI.Domain.Interfaces;
using UserAPI.Domain.Repositories;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("create-user")]
        public async Task<ActionResult<User>>CreateUser(User user)
        {
            //checkign if email is already registered in the DB
            if (!string.IsNullOrEmpty(user.Email))
            {
                var checkEmail = await _userRepository.CheckExistingEmailAsync(user.Email);
                if(checkEmail == false)
                {
                    return Conflict(new { code = 409, message = "Email is already registered", status = false });
                }
            }


            var isCreatedUser = await _userRepository.CreateUserAsync(user);

            if(isCreatedUser == false)
            {
                return BadRequest(new { code = 400, message = "Bad request", status = false });
            }

            return Ok(new { code = 200, message = "Successful request", status = true });
        }

        [HttpPost("check-login")]
        public async Task<ActionResult<User>>CheckLogin(User user)
        {
            if(user == null)
            {
                return BadRequest(new { code = 400, message = "Bad request", status = false });
            }


            var checking = await _userRepository.CheckLoginCredentials(user);

            if(checking == null)
            {
                return Unauthorized(new { code = 401, message = "Unauthorized. Invalid email or password", status = false });
            }

            return Ok(new { code = 200, message = "Successful request", status = true, data = checking });

        }


    }
}
