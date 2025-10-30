using LinkUp.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _db;
        protected DbSet<T> Set => _db.Set<T>();

        public Repository(ApplicationDbContext db) => _db = db;

        public Task<T?> GetByIdAsync(Guid id) => Set.FindAsync(id).AsTask();
        public async Task AddAsync(T entity) => await Set.AddAsync(entity);
        public Task UpdateAsync(T entity) => Task.CompletedTask;
        public Task DeleteAsync(T entity) { Set.Remove(entity); return Task.CompletedTask; }
        public Task SaveChangesAsync() => _db.SaveChangesAsync();

        public void MarkAdded(object entity) => _db.Entry(entity).State = EntityState.Added;
    }
}
