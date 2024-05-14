using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Diary.Domain.Interfaces.UnitOfWork;

public interface IUnitOfWork : IChangesSaver
{
    Task<IDbContextTransaction> BeginTransactionAsync();
    IBaseRepository<User> Users { get; set; }
    IBaseRepository<Role> Roles { get; set; }
    IBaseRepository<UserRole> UserRoles { get; set; }
}