using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositoires
{
    public interface IUserRepository<TEntity>
    {
        TEntity Find(string userEmail);
        void Add(TEntity entity);
    }
}
