using System.Net.Mime;
using Asp.Versioning;
using Diary.Domain.Dto.RoleDto;
using Diary.Domain.Dto.UserRole;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diary.Api.Controllers;

/// <summary>
/// Контроллер для управления ролями и управления ролями для пользователей
/// </summary>
[Consumes(MediaTypeNames.Application.Json)]
[ApiController]
[Authorize(Roles = "Admin, Moderator")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Создание новой роли
    /// </summary>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     POST
    ///     {
    ///         "name": "Report #1",
    ///     }
    /// 
    /// </remarks>
    /// <returns></returns>
    [HttpPost("create-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<RoleDto>>> CreateRoleAsync([FromBody] RoleDto dto)
    {
        var report = await _roleService.CreateRoleAsync(dto);

        if (report.IsSuccess)
        {
            return Ok(report);
        }

        return BadRequest(report);
    }

    /// <summary>
    /// Удаление роли
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete-role/{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<RoleDto>>> DeleteRoleAsync(long id)
    {
        var report = await _roleService.RemoveRoleAsync(id);

        if (report.IsSuccess)
        {
            return Ok(report);
        }

        return BadRequest(report);
    }

    /// <summary>
    /// Обновление роли
    /// </summary>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     POST
    ///     {
    ///         "id": 0,
    ///         "name": "string",
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("update-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<RoleDto>>> UpdateRoleAsync(UpdateRoleDto dto)
    {
        var report = await _roleService.UpdateRoleAsync(dto);

        if (report.IsSuccess)
        {
            return Ok(report);
        }

        return BadRequest(report);
    }

    /// <summary>
    /// Добавление роли пользователю
    /// </summary>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     POST
    ///     {
    ///         "login": "svyatmtk",
    ///         "roleId": 2
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("add-role-for-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<RoleDto>>> AddUserRoleAsync([FromBody] CreateUserRole dto)
    {
        var report = await _roleService.AddUserRoleAsync(dto);

        if (report.IsSuccess)
        {
            return Ok(report);
        }

        return BadRequest(report);
    }

    /// <summary>
    /// Обновить роль пользователя
    /// </summary>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     POST
    ///     {
    ///         "login": "svyatmtk",
    ///         "oldRoleId": 2,
    ///         "newRoleId": 1
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("update-role-for-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<RoleDto>>> UpdateUserRoleAsync([FromBody] UpdateUserRole dto)
    {
        var report = await _roleService.UpdateUserRoleAsync(dto);

        if (report.IsSuccess)
        {
            return Ok(report);
        }

        return BadRequest(report);
    }

    /// <summary>
    /// Удаление роли у пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpDelete("remove-role-for-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<RoleDto>>> RemoveUserRoleAsync([FromBody] RemoveUserRole dto)
    {
        var report = await _roleService.RemoveUserRoleAsync(dto);

        if (report.IsSuccess)
        {
            return Ok(report);
        }

        return BadRequest(report);
    }
}