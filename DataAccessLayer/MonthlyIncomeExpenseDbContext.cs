using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class MonthlyIncomeExpenseDbContext:DbContext
    {
        public MonthlyIncomeExpenseDbContext(DbContextOptions<MonthlyIncomeExpenseDbContext> options) : base(options)
        {
            
        }
        public DbSet<IncomeExpense> IncomeExpenses { get; set; }
    }
}
