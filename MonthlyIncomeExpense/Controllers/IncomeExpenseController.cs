using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MonthlyIncomeExpense.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonthlyIncomeExpense.Controllers
{
    public class IncomeExpenseController : Controller
    {
        IRepository<IncomeExpense> _repository;
        public IncomeExpenseController(IRepository<IncomeExpense> repository)
        {
            _repository = repository;
        }
       
        public ActionResult Index()
        {
            List<IncomeExpense> result = _repository.List();
            var str = JsonSerializer.Serialize(result);
            List<IncomeExpenseViewModel> model = JsonSerializer.Deserialize<List<IncomeExpenseViewModel>>(str);
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new IncomeExpenseViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IncomeExpense model)
        {
            try
            {
                _repository.AddOrUpdate(model);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            IncomeExpense entity = _repository.GetById(id);
           
            var str = JsonSerializer.Serialize(entity);
            IncomeExpenseViewModel model=JsonSerializer.Deserialize<IncomeExpenseViewModel>(str);
            return View(nameof(Create), model);
        }

        public ActionResult Delete(int id)
        {
            _repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
