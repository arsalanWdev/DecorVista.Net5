using DecorVista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecorVista.DataAccess.Repository.IRepository
{
    public interface IPortfolioRepository:IRepository<Portfolio>
    {
        void Update(Portfolio obj);
        IEnumerable<Portfolio> GetPortfolioByDesignerId(string userId);


    }
}
