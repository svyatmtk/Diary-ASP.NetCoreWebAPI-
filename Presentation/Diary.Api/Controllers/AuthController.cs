using System.Net.Mime;
using Asp.Versioning;
using Diary.Domain.Dto;
using Diary.Domain.Dto.UserDto;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using Microsoft.AspNetCore.Mvc;

namespace Diary.Api.Controllers;

/// <summary>
/// Контроль регистрации и авторизации
/// </summary>
[Consumes(MediaTypeNames.Application.Json)]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{Version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Route("register-user")]
    [HttpPost]
    public async Task<ActionResult<BaseResult>> Register([FromBody] RegisterUserDto dto)
    {
        var report = await _authService.Register(dto);

        if (report.IsSuccess)
        {
            return Ok(report);
        }

        return BadRequest(report);
    }

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Route("login-user")]
    [HttpPost]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginUserDto dto)
    {
        var report = await _authService.Login(dto);

        if (report.IsSuccess)
        {
            return Ok(report);
        }

        return BadRequest(report);
    }
}