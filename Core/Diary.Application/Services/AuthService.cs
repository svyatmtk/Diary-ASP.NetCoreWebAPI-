using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Diary.Application.Resources;
using Diary.Domain.Dto;
using Diary.Domain.Dto.UserDto;
using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Interfaces.UnitOfWork;
using Diary.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Diary.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<UserToken> _userTokenRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IBaseRepository<UserRole> _userRoleRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public AuthService(IBaseRepository<User> userRepository, ILogger logger, IMapper mapper, 
        IBaseRepository<UserToken> userTokenRepository, ITokenService tokenService, IBaseRepository<UserRole> userRoleRepository, 
        IBaseRepository<Role> roleRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _logger = logger;
        _mapper = mapper;
        _userTokenRepository = userTokenRepository;
        _tokenService = tokenService;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResult<UserDto>> Register(RegisterUserDto dto)
    {
        if (dto.Password != dto.PasswordConfirm)
        {
            return new BaseResult<UserDto>()
            {
                ErrorMessage = ErrorMessage.PasswordsNotMatch,
                ErrorCode = (int)ErrorCodes.PasswordsNotMatch
            };
        }
        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);
        if (user != null)
        {
            return new BaseResult<UserDto>()
            {
                ErrorMessage = ErrorMessage.UserAlreadyExists,
                ErrorCode = (int)ErrorCodes.UserAlreadyExists
            };
        }

        var hashUserPassword = HashPassword(dto.Password);

        using (var transactions = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                user = new User()
                {
                    Login = dto.Login,
                    Password = hashUserPassword
                };
                await _unitOfWork.Users.CreateAsync(user);
                await _unitOfWork.Users.SaveChangesAsync();

                var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == nameof(Roles.User));
                if (role == null)
                {
                    return new BaseResult<UserDto>()
                    {
                        ErrorMessage = ErrorMessage.RoleDoesNotExists,
                        ErrorCode = (int)ErrorCodes.RoleDoesNotExists
                    };
                }

                UserRole userRole = new UserRole()
                {
                    RoleId = role.Id,
                    UserId = user.Id
                };

                await _unitOfWork.UserRoles.CreateAsync(userRole);
                await _unitOfWork.SaveChangesAsync();
                
                await transactions.CommitAsync();
                
            }
            catch (Exception e)
            {
                await transactions.RollbackAsync();
            }
        }
        return new BaseResult<UserDto>()
        {
            Data = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
    {
        var user = await _userRepository.GetAll().Include(x => x.Roles).FirstOrDefaultAsync(x => x.Login == dto.Login);
        if (user == null)
        {
            return new BaseResult<TokenDto>()
            {
                ErrorMessage = ErrorMessage.UserNotFound, ErrorCode = (int)ErrorCodes.UserNotFound
            };
        }

        if (!IsPasswordMatch(user.Password, dto.Password))
        {
            return new BaseResult<TokenDto>()
            {
                ErrorMessage = ErrorMessage.PasswordIsWrong, ErrorCode = (int)ErrorCodes.PasswordIsWrong
            };
        }

        var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

        var userRoles = user.Roles;
        var claims = userRoles.Select(x => new Claim(ClaimTypes.Role, x.Name)).ToList();
        claims.Add(new Claim(ClaimTypes.Name, user.Login));

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        if (userToken == null)
        {
            userToken = new UserToken()
            {
                UserId = user.Id, RefreshToken = refreshToken, RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };
            await _userTokenRepository.CreateAsync(userToken);
            await _userTokenRepository.SaveChangesAsync();
        }
        else
        {
            userToken.RefreshToken = refreshToken;
            userToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _userTokenRepository.Update(userToken);
            await _userTokenRepository.SaveChangesAsync();
        }

        return new BaseResult<TokenDto>()
        {
            Data = new TokenDto() { AccessToken = accessToken, RefreshToken = refreshToken }
        };
    }

    private string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private bool IsPasswordMatch(string userPasswordHash, string userPassword)
    {
        var hash = HashPassword(userPassword);
        return userPasswordHash == hash;
    }
}