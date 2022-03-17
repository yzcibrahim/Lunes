using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class IncomeExpenseRepository : BaseRepository<IncomeExpense>, IRepository<IncomeExpense>
    {
        public IncomeExpenseRepository(MonthlyIncomeExpenseDbContext context):base(context)
        {

        }
    }
}
