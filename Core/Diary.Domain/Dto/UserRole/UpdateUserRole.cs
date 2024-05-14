namespace Diary.Domain.Dto.UserRole;

public record UpdateUserRole(string Login, long OldRoleId, long NewRoleId);