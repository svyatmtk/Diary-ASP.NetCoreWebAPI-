namespace Diary.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> : IChangesSaver
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> CreateAsync(TEntity entity);
        TEntity Update(TEntity entity);
        void Remove(TEntity entity);
    }
}