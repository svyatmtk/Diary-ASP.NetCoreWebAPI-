using AutoMapper;
using Diary.Application.Resources;
using Diary.Domain.Dto.RoleDto;
using Diary.Domain.Dto.UserRole;
using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Interfaces.UnitOfWork;
using Diary.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace Diary.Application.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IBaseRepository<UserRole> _userRoleRepository;
    private readonly IMapper _mapper;

    public RoleService(IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository, IMapper mapper,
        IBaseRepository<UserRole> userRoleRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResult<RoleDto>> CreateRoleAsync(RoleDto dto)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);

        if (role != null)
        {
            return new BaseResult<RoleDto>()
            {
                ErrorMessage = ErrorMessage.RoleAlreadyExists,
                ErrorCode = (int)ErrorCodes.RoleAlreadyExists
            };
        }

        role = new Role()
        {
            Name = dto.Name
        };
        await _roleRepository.CreateAsync(role);
        await _roleRepository.SaveChangesAsync();

        return new BaseResult<RoleDto>()
        {
            Data = _mapper.Map<RoleDto>(role)
        };
    }

    public async Task<BaseResult<RoleDto>> UpdateRoleAsync(UpdateRoleDto dto)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (role == null)
        {
            return new BaseResult<RoleDto>()
            {
                ErrorMessage = ErrorMessage.RoleDoesNotExists,
                ErrorCode = (int)ErrorCodes.RoleDoesNotExists
            };
        }

        role.Name = dto.Name;

        var updatedRole = _roleRepository.Update(role);
        await _roleRepository.SaveChangesAsync();

        return new BaseResult<RoleDto>()
        {
            Data = _mapper.Map<RoleDto>(updatedRole)
        };

    }

    public async Task<BaseResult<RoleDto>> RemoveRoleAsync(long id)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

        if (role == null)
        {
            return new BaseResult<RoleDto>()
            {
                ErrorMessage = ErrorMessage.RoleDoesNotExists,
                ErrorCode = (int)ErrorCodes.RoleDoesNotExists
            };
        }

        _roleRepository.Remove(role);
        await _roleRepository.SaveChangesAsync();

        return new BaseResult<RoleDto>()
        {
            Data = _mapper.Map<RoleDto>(role)
        };
    }

    public async Task<BaseResult<UserRoleDto>> AddUserRoleAsync(CreateUserRole dto)
    {
        var user = await _userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login);

        if (user == null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound
            };
        }

        var roles = user.Roles.Select(x => x.Id).ToArray();

        if (roles.All(x => x != dto.RoleId))
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.RoleId);
            if (role == null)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleDoesNotExists,
                    ErrorCode = (int)ErrorCodes.RoleDoesNotExists
                };
            }

            UserRole userRole = new UserRole()
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            await _userRoleRepository.CreateAsync(userRole);
            await _userRoleRepository.SaveChangesAsync();

            return new BaseResult<UserRoleDto>()
            {
                Data = new UserRoleDto(user.Login, role.Name)
            };
        }

        return new BaseResult<UserRoleDto>()
        {
            ErrorMessage = ErrorMessage.UserAlreadyHasThisRole,
            ErrorCode = (int)ErrorCodes.UserAlreadyHasThisRole
        };
    }

    public async Task<BaseResult<UserRoleDto>> RemoveUserRoleAsync(RemoveUserRole dto)
    {
        var user = await _userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login);

        if (user == null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound
            };
        }

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.RoleId);

        if (role == null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorMessage = ErrorMessage.RoleDoesNotExists,
                ErrorCode = (int)ErrorCodes.RoleDoesNotExists
            };
        }

        var userRole = await _userRoleRepository.GetAll()
            .Where(x => x.RoleId == dto.RoleId)
            .FirstOrDefaultAsync(x => x.UserId == user.Id);

        _userRoleRepository.Remove(userRole);
        await _userRoleRepository.SaveChangesAsync();

        return new BaseResult<UserRoleDto>()
        {
            Data = new UserRoleDto(user.Login, role.Name)
        };

    }

    public async Task<BaseResult<UserRoleDto>> UpdateUserRoleAsync(UpdateUserRole dto)
    {
        var user = await _userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login);

        if (user == null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound
            };
        }

        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.OldRoleId);
        if (role == null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorMessage = ErrorMessage.RoleDoesNotExists,
                ErrorCode = (int)ErrorCodes.RoleDoesNotExists
            };
        }

        var newRoleForUser = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.NewRoleId);
        if (newRoleForUser == null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorMessage = ErrorMessage.RoleDoesNotExists,
                ErrorCode = (int)ErrorCodes.RoleDoesNotExists
            };
        }

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var userRole = await _unitOfWork.UserRoles.GetAll().Where(x => x.RoleId == role.Id)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                _unitOfWork.UserRoles.Remove(userRole);
                await _unitOfWork.SaveChangesAsync();

                var newUserRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = newRoleForUser.Id
                };

                await _unitOfWork.UserRoles.CreateAsync(newUserRole);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
            }
        }

        return new BaseResult<UserRoleDto>()
        {
            Data = new UserRoleDto(user.Login, newRoleForUser.Name)
        };
    }
}