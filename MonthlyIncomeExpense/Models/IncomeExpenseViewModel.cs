using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyIncomeExpense.Models
{
    public class IncomeExpenseViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Desc")]
        [Required(ErrorMessage = "{0} is required")]
        public string Text { get; set; }
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        public string ImageBase64 { get; set; }
    }
}
