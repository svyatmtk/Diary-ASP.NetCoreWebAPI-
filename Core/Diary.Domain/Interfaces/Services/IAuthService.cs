using Diary.Domain.Dto;
using Diary.Domain.Dto.UserDto;
using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Services;

/// <summary>
/// Сервис для регистрации и авторизации пользователя
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<UserDto>> Register(RegisterUserDto dto);
    
    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<TokenDto>> Login(LoginUserDto dto);
}