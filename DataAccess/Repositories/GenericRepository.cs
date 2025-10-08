using StockManager.Context;
using StockManager.DataAccess.Interfaces;
using StockManager.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StockManager.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public T GetById(int id) => _dbSet.Find(id);
        public IEnumerable<T> GetAll() => _dbSet.Where(x => x.IsDeleted == false).ToList();

        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate).ToList();

        public T FirstOrDefault(Expression<Func<T, bool>> predicate) => _dbSet.FirstOrDefault(predicate);

        public T SingleOrDefault(Expression<Func<T, bool>> predicate) => _dbSet.SingleOrDefault(predicate);

        public void Add(T entity) => _dbSet.Add(entity);

        public void AddRange(IEnumerable<T> entities) => _dbSet.AddRange(entities);

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            var prop = typeof(T).GetProperty("IsDeleted");
            if (prop != null)
            {
                prop.SetValue(entity, true);
                if(_context.Entry(entity).State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }
                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                if (_context.Entry(entity).State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }
                _dbSet.Remove(entity);
            }
        }

        public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

        public IQueryable<T> Queryable() => _dbSet.AsQueryable();

        public void Save() => _context.SaveChanges();
    }
}
