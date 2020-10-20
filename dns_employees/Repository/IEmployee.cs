using dns_employees.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dns_employees.Repository
{
    interface IEmployee
    {
        IList<Employee> GetEmployees();
        Employee GetEmployeeById(int? id);
        void InsertNew(Employee employee);
        void Update(Employee employee);
        void Delete(Employee employee);
    }
}
