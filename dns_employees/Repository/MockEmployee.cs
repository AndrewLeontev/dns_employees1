using dns_employees.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace dns_employees.Repository
{
    public class MockEmployee : IEmployee
    {
        private readonly string CS = GetConfiguration().GetSection("Data").GetSection("ConnectionString").Value;

        public static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        public IList<Employee> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spGetEmployees", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var employee = new Employee()
                    {
                        id = Convert.ToInt32(rdr["id"]),
                        Name = rdr["Name"].ToString(),
                        Department = rdr["Department"].ToString(),
                        Title = rdr["Title"].ToString(),
                        Manager_id = rdr["Manager_id"].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(rdr["Manager_id"]),
                        Hire_date = rdr["Hire_date"].Equals(DBNull.Value) ? default(DateTime) : Convert.ToDateTime(rdr["Hire_date"]).Date,
                        ManagerName = rdr["Manager_name"].ToString(),
                        Manager = this.GetEmployeeById(rdr["Manager_id"].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(rdr["Manager_id"]))
                    };
                    employees.Add(employee);
                }
                return (employees);
            }
        }

        public IList<Employee> GetManagerOptions(int current_id) 
        {
            if (current_id == 0)
            {
                return this.GetEmployees();
            }
            else
            {
                List<Employee> employees = new List<Employee>();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("spEmployeesPossibleManagersList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@current_id", current_id);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var employee = new Employee()
                        {
                            id = Convert.ToInt32(rdr["id"]),
                            Name = rdr["Name"].ToString(),
                            Department = rdr["Department"].ToString(),
                            Title = rdr["Title"].ToString(),
                            Manager_id = rdr["Manager_id"].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(rdr["Manager_id"]),
                            Hire_date = rdr["Hire_date"].Equals(DBNull.Value) ? default(DateTime) : Convert.ToDateTime(rdr["Hire_date"]).Date,
                            Manager = this.GetEmployeeById(rdr["Manager_id"].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(rdr["Manager_id"]))
                        };
                        employees.Add(employee);
                    }
                    return (employees);
                }
            }
            
        }
        public IList<Employee> GetEmployeesManagers()
        {
            List<Employee> employees = new List<Employee>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spEmployeesAllManagers", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var employee = new Employee()
                    {
                        id = Convert.ToInt32(rdr["id"]),
                        Name = rdr["Name"].ToString(),
                        Department = rdr["Department"].ToString(),
                        Title = rdr["Title"].ToString(),
                        Manager_id = rdr["Manager_id"].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(rdr["Manager_id"]),
                        Hire_date = rdr["Hire_date"].Equals(DBNull.Value) ? default(DateTime) : Convert.ToDateTime(rdr["Hire_date"]).Date,
                        Manager = this.GetEmployeeById(rdr["Manager_id"].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(rdr["Manager_id"]))
                    };
                    employees.Add(employee);
                }
                return (employees);
            }
        }

        public Employee GetEmployeeById(int? id)
        {
            Employee employee = new Employee();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spFindEmployeeById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    employee.id = Convert.ToInt32(rdr["id"]);
                    employee.Name = rdr["Name"].ToString();
                    employee.Department = rdr["Department"].ToString();
                    employee.Title = rdr["Title"].ToString();
                    employee.Manager_id = rdr["Manager_id"].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(rdr["Manager_id"]);
                    employee.Hire_date = rdr["Hire_date"].Equals(DBNull.Value) ? default(DateTime) : Convert.ToDateTime(rdr["Hire_date"]).Date;
                    employee.Manager = this.GetEmployeeById(rdr["Manager_id"].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(rdr["Manager_id"]));
                }
                return employee;
            }
        }

        public void InsertNew(Employee employee)
        {
            using (SqlConnection con = new SqlConnection(CS))
            {
                var cmd = new SqlCommand("spAddNew", con);
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", employee.Name);
                cmd.Parameters.AddWithValue("@Department", employee.Department);
                cmd.Parameters.AddWithValue("@Title", employee.Title);
                if (employee.Manager_id != 0 && employee.Manager_id != null)
                {
                    cmd.Parameters.AddWithValue("@Manager_id", employee.Manager_id);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Manager_id", DBNull.Value);
                }
                cmd.Parameters.AddWithValue("@Hire_date", employee.Hire_date);
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Employee employee)
        {
            using (SqlConnection con = new SqlConnection(CS))
            {
                var cmd = new SqlCommand("spUpdateEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@id", employee.id);
                cmd.Parameters.AddWithValue("@Name", employee.Name);
                cmd.Parameters.AddWithValue("@Department", employee.Department);
                cmd.Parameters.AddWithValue("@Title", employee.Title);
                if (employee.Manager_id != 0 && employee.Manager_id != null)
                {
                    cmd.Parameters.AddWithValue("@Manager_id", employee.Manager_id);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Manager_id", DBNull.Value);
                }
                cmd.Parameters.AddWithValue("@Hire_date", employee.Hire_date);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(Employee employee)
        {
            using (SqlConnection con = new SqlConnection(CS))
            {
                var cmd = new SqlCommand("spDeleteEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@id", employee.id);
                cmd.ExecuteNonQuery();
            }
        }

    }
}
