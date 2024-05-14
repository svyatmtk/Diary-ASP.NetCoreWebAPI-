using System.Net.Mime;
using Diary.Domain.Dto;
using Diary.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diary.Api.Controllers;

/// <summary>
/// Контроллер обновления refresh токена
/// </summary>
[Consumes(MediaTypeNames.Application.Json)]
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /// <summary>
    /// Обновление refresh токена
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenDto>> RefreshToken([FromBody] TokenDto dto)
    {
        var report = await _tokenService.RefreshToken(dto);

        if (report.IsSuccess)
        {
            return Ok(report);
        }

        return BadRequest(report);
    }
}