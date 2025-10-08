using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManager.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Entities.Product> Products { get; }
        IGenericRepository<Entities.Category> Categories { get; }

        int SaveChanges();
    }
}
