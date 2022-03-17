using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IRepository<T> where T:BaseEntity
    {
        T GetById(int id);
        void Delete(int id);
        T AddOrUpdate(T entity);
        List<T> List();
    }
}
