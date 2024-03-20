using AuthenticationApi.Dtos;
using AuthenticationApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public UserController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await _authenticationService.Login(request);
            // Console.WriteLine(request.Username);
            // Console.WriteLine(request.Password);
            SignInReponse response = new SignInReponse() {UserName = request.Username,Token = token};
            
            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var token = await _authenticationService.Register(request);

            SignInReponse response = new SignInReponse() {UserName = request.Username,Token = token};

            // Console.WriteLine(request.Username);
            // Console.WriteLine(request.Password);
            // Console.WriteLine(request.Email);
            return Ok(response);
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }


    }
}