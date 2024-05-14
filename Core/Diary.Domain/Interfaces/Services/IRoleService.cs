using Diary.Domain.Dto.RoleDto;
using Diary.Domain.Dto.UserRole;
using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Services;
/// <summary>
/// Сервис для управления ролями
/// </summary>
public interface IRoleService
{   /// <summary>
    /// Создание роли
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> CreateRoleAsync(RoleDto dto);
    /// <summary>
    /// Обновление роли
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> UpdateRoleAsync(UpdateRoleDto dto);
    /// <summary>
    /// Удаление роли
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> RemoveRoleAsync(long id);
    /// <summary>
    /// Добавление роли пользователю
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<UserRoleDto>> AddUserRoleAsync(CreateUserRole dto);
    /// <summary>
    /// Удалить роль пользователю 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<UserRoleDto>> RemoveUserRoleAsync(RemoveUserRole dto);
    /// <summary>
    /// Обновить роль пользователю
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<UserRoleDto>> UpdateUserRoleAsync(UpdateUserRole dto);
}