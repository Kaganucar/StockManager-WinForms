using StockManager.DataAccess.Interfaces;
using StockManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManager.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        private IGenericRepository<Entities.Product> _products;
        private IGenericRepository<Entities.Category> _categories;

        public UnitOfWork(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IGenericRepository<Entities.Product> Products
            => _products ?? (_products = new GenericRepository<Entities.Product>(_context));

        public IGenericRepository<Entities.Category> Categories
            => _categories ?? (_categories = new GenericRepository<Entities.Category>(_context));

        public int SaveChanges() => _context.SaveChanges(); 

        public void Dispose() => _context.Dispose();
    }
}
