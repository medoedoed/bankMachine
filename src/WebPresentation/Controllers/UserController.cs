using System.Security.Claims;
using Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Results;

#pragma warning disable CA2007

namespace WebPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        ArgumentNullException.ThrowIfNull(userService);
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Login(string username, string password)
    {
        UserResult result = await _userService.Login(username, password);
        if (result is not UserResult.Success resultSuccess) return BadRequest();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, $"{resultSuccess.User.Id}", ClaimValueTypes.Integer64),
            new(ClaimTypes.Role, resultSuccess.User.Role),
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
            });

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register(string username, string password, string role)
    {
        StringResult result = await _userService.Register(username, password, role);
        if (result is StringResult.Failure failureResult) return BadRequest(failureResult.Message);
        return Ok();
    }
}