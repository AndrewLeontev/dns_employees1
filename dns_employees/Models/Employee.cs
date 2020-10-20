using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace dns_employees.Models
{
    public class Employee
    {
        public int id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string Department { get; set; } = "";
        [Required]
        public string Title { get; set; } = "";

        public Employee Manager { get; set; }
        public int? Manager_id { get; set; }
        [Required]
        public DateTime Hire_date { get; set; } = DateTime.Now;
        public string ManagerName { get; set; } = "";

    }
}
