using DataLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTask.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationContext _db;

        public EmployeeController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult GetEmployees(int page, int rows)
        {
            if (page == 0)
                page = 1;
            if (rows == 0)
            {
                rows = 10;
            }
            var employees = _db.Employees.Skip((page - 1) * rows).Take(rows).ToList();

            return new JsonResult(employees);
        }

        [HttpPost]
        public ActionResult Update(int id, Employee model)
        {
            var employee = _db.Employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
            {
                return new JsonResult(new { success = false, errorMsg = "can not find such Employee" });
            }

            employee.FullName = model.FullName;
            employee.Birthday = model.Birthday;
            employee.Sex = model.Sex;
            employee.IsRegular = model.IsRegular;
            if (employee.IsRegular)
            {
                employee.PersonnelNumber = id;
            }
            _db.SaveChanges();

            return new JsonResult(new { success = true });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var employee = _db.Employees.FirstOrDefault(x => x.Id == id);
            if(employee == null)
            {
                return new JsonResult(new { success = false, errorMsg = "can not find such Employee" });
            }

            _db.Employees.Remove(employee);
            _db.SaveChanges();

            return new JsonResult(new { success = true });
        }

        [HttpPost]
        public ActionResult Save(Employee model)
        {
            _db.Employees.Add(model);
            _db.SaveChanges();

            return new JsonResult(new { success = true });
        }

    }
}
