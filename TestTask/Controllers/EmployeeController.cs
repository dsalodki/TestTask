using DataLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TestTask.Models;

namespace TestTask.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationContext _db;

        public EmployeeController(ApplicationContext db)
        {
            _db = db;
        }

        public IActionResult GetTotal()
        {
            return new JsonResult(_db.Employees.Count());
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
        public ActionResult Update(Employee model)
        {
            var employee = _db.Employees.FirstOrDefault(x => x.Id == model.Id);
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
                employee.PersonnelNumber = model.Id;
            }
            else
            {
                employee.PersonnelNumber = null;
            }
            _db.SaveChanges();

            return new JsonResult(new { success = true });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var employee = _db.Employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
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
            if (model.IsRegular)
            {
                var employee = _db.Employees.FirstOrDefault(x => x.Id == model.Id);
                employee.PersonnelNumber = model.Id;
                _db.SaveChanges();
            }

            return new JsonResult(new { success = true });
        }

        public IActionResult UploadJson(IFormFile jsonFile)
        {
            EmployeeJsonData[] json = null;
            // read json
            using (var reader = new StreamReader(jsonFile.OpenReadStream()))
            {
                var content = reader.ReadToEnd();
                json = JsonConvert.DeserializeObject<EmployeeJsonData[]>(content);
            }

            // update db
            if (json == null)
            {
                return File(new byte[] { }, "text/plain");
            }

            var result = new StringBuilder();
            foreach (var row in json)
            {
                if (string.IsNullOrEmpty(row.FullName))
                {
                    result.AppendLine($"error: Employee with id {row.Id} has empty Fullname, skipped");
                    continue;
                }

                if (!row.IsRegular && row.PersonnelNumber.HasValue)
                {
                    result.AppendLine(
                        $"error: not regular employee can not have PersonnelNumber {row.PersonnelNumber.HasValue}");
                    row.PersonnelNumber = null;
                }

                if (row.IsRegular && !row.PersonnelNumber.HasValue)
                {
                    result.AppendLine($"error: regular employee with Id {row.Id} has not PersonnelNumber");
                    if (row.Id != 0)
                    {
                        row.PersonnelNumber = row.Id;
                    }
                }

                // add entry to db
                if (row.Id == 0)
                {
                    var employee = new Employee()
                    {
                        IsRegular = row.IsRegular,
                        FullName = row.FullName,
                        Birthday = row.Birthday,
                        Sex = (DataLayer.Entities.Sex)row.Sex
                    };
                    _db.Employees.Add(employee);
                    _db.SaveChanges();
                    if (employee.IsRegular)
                    {
                        employee.PersonnelNumber = employee.Id;
                        _db.SaveChanges();
                    }

                    result.AppendLine($"Entry with Id {employee.Id} was created");
                }
                else
                {
                    var employee = _db.Employees.FirstOrDefault(x => x.Id == row.Id);
                    if (employee == null)
                    {
                        result.AppendLine($"error: Employee with Id {row.Id} was not find");
                    }
                    else
                    {
                        employee.IsRegular = row.IsRegular;
                        employee.PersonnelNumber = row.PersonnelNumber;
                        employee.Birthday = row.Birthday;
                        employee.FullName = row.FullName;
                        employee.Sex = (DataLayer.Entities.Sex)row.Sex;

                        _db.SaveChanges();

                        result.AppendLine($"Entry with Id {row.Id} was changed");
                    }
                }
            }


            return File(Encoding.UTF8.GetBytes(result.ToString()), "text/plain");
        }
    }
}
