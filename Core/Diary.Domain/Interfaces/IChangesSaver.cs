namespace Diary.Domain.Interfaces;

public interface IChangesSaver
{
    Task<int> SaveChangesAsync();
}