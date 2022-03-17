using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class IncomeExpense: BaseEntity
    {
        public string Text { get; set; }
        public decimal Price { get; set; }
        public string ImageBase64 { get; set; }
    }
}
