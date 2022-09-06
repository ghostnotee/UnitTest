using Microsoft.EntityFrameworkCore;
using UnitTest.Web.Models;

namespace UnitTest.Web.Repository;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private unittestdbContext Context;
    private DbSet<TEntity> DbSet;

    public Repository(unittestdbContext context)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();
    }

    public async Task CreateAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
        Context.SaveChanges();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task<TEntity> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
        Context.SaveChanges();
    }
}
