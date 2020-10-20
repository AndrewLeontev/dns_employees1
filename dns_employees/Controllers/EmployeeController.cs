using System;
using Microsoft.AspNetCore.Mvc;
using dns_employees.Models;
using dns_employees.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using dns_employees.Helpers;
using Microsoft.Extensions.Logging;

namespace dns_employees.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly MockEmployee db = new MockEmployee();
        private readonly ILogger _logger;

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Fetching employees list");
            var employees = db.GetEmployees();
            return View(employees);
        }

        public IActionResult ManagersList()
        {
            _logger.LogInformation("Fetching managers list");
            var employees = db.GetEmployeesManagers();
            return View("Index", employees);
        }

        public IActionResult Create()
        {
            ViewBag.EmployeesList = new SelectList(db.GetManagerOptions(default(int)), "id", "Name");
            return View(new Employee());
        }

        public IActionResult Edit(int id)
        {
            var employee = db.GetEmployeeById(id);

            if (employee == null)
            {
                _logger.LogError($"Employee with id={id} - {NotFound()}");
                return NotFound();
            }
            ViewBag.EmployeesList = new SelectList(db.GetManagerOptions(default(int)), "id", "Name");

            _logger.LogInformation($"{employee.id} {employee.Name}");
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(int id, [Bind("id,Name,Department,Manager_id,Title,Hire_date")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                //Insert
                if (id == 0)
                {   
                    try
                    {
                        db.InsertNew(employee);
                    }
                    catch (Exception ex)
                    {
                            _logger.LogError($"Error: {ex.Message}");
                            return Json(new { isValid = false, html = EmployeeHelper.RenderRazorViewToString(this, "Create", employee) });
                    }
                }
                //Update
                else
                {
                    try
                    {
                        db.Update(employee);
                    }
                    catch (Exception ex)
                    {
                        if (employee == null)
                        {
                            _logger.LogError($"Employee with id={id} - {NotFound()}");
                            return NotFound();
                        }
                        else
                        {
                            _logger.LogError($"Error: {ex.Message}");
                            return Json(new { isValid = false, html = EmployeeHelper.RenderRazorViewToString(this, "Edit", employee) });
                        }
                    }
                }
                return Json(new { isValid = true, html = EmployeeHelper.RenderRazorViewToString(this, "_ViewAll", db.GetEmployees())});
            }
            return Json(new { isValid = false, html = EmployeeHelper.RenderRazorViewToString(this, id == 0 ? "Create" : "Edit", employee) });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = db.GetEmployeeById(id);
            try
            {
                db.Delete(employee);
                return Json(new { html = EmployeeHelper.RenderRazorViewToString(this, "_ViewAll", db.GetEmployees()) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return Json(new { isValid = false, html = EmployeeHelper.RenderRazorViewToString(this, "index", employee) });

            }

        }
    }
}
