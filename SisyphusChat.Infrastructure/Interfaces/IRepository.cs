namespace SisyphusChat.Infrastructure.Interfaces;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task AddAsync(TEntity entity);

    Task<ICollection<TEntity>> GetAllAsync();

    Task<TEntity> GetByIdAsync(string id);

    Task DeleteByIdAsync(string id);

    Task UpdateAsync(TEntity entity);
}