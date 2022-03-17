using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public abstract class BaseRepository<T> where T : BaseEntity
    {
        protected MonthlyIncomeExpenseDbContext _ctx;
        public BaseRepository(MonthlyIncomeExpenseDbContext ctx)
        {
            _ctx = ctx;
        }

        public virtual T GetById(int id)
        {
            return _ctx.Set<T>().FirstOrDefault(c => c.Id == id);
        }

        public void Delete(int id)
        {
            T silinecek = GetById(id);
            _ctx.Set<T>().Remove(silinecek);
            _ctx.SaveChanges();
        }

        public T AddOrUpdate(T entity)
        {
            if (entity.Id > 0)
            {
                _ctx.Attach(entity);
                _ctx.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                _ctx.Set<T>().Add(entity);
            }
            _ctx.SaveChanges();

            return entity;
        }

        public List<T> List()
        {
            return _ctx.Set<T>().ToList();
        }
    }
}
