using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Swap.Chat_Database
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext _context = null;

        public Repository(DbContext context) => _context = context;

        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
        }

        public TEntity Get(int id) => _context.Set<TEntity>().Find(id);

        public IEnumerable<TEntity> GetAll() => _context.Set<TEntity>();

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            _context.SaveChanges();
        }
    }
}