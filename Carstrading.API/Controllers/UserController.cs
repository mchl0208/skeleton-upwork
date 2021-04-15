using AutoMapper;
using Carstrading.API.Dtos;
using Carstrading.BusinessLogic.Implementations;
using Carstrading.BusinessLogic.Interfaces;
using Carstrading.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Carstrading.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(
            IUserService userService,
            IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("Signup")]
        public IActionResult Signup([FromBody] UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            if (userDto.Password != userDto.ConfirmPassword) throw new AppException("password doesn't match");

            try
            {
                _userService.Create(user, userDto.Password);
                return Ok(new
                {
                    Success = true
                });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
