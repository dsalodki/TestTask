using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Entities;

namespace TestTask.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationContext _db;

        public EmployeeController(ApplicationContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var employees = _db.Employees.ToList();

            return View(employees);
        }
    }
}
