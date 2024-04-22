using Microsoft.AspNetCore.Mvc;
using MCS_project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCS_project.Controllers
{
    public class EmployeeController : Controller
    {
        private List<Employee> LoadEmployees()
        {
            var employees = new List<Employee>();
            string? filePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location?.ToString()), @"Employees.txt");
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (reader.ReadLine() is string line)
                {
                    if (line.Split('\t').Length < 5) { Console.WriteLine($"Warning: Line skipped due to insufficient data elements: {line}"); continue; }
                    var employee = new Employee
                    {
                        Name = line.Split('\t')[0].Trim(),
                        Address = line.Split('\t')[1].Trim(),
                        BirthDate = string.IsNullOrWhiteSpace(line.Split('\t')[2]) ? DateOnly.MinValue : DateOnly.Parse(line.Split('\t')[2]),
                        Graduation = line.Split('\t')[3].Trim(),
                        EmploymentType = line.Split('\t')[4].Trim(),
                    };
                    if (employee.EmploymentType != "Free lancer") { employee.Assurance = line.Split('\t')[5].Trim(); }
                    if (!string.IsNullOrEmpty(line.Split('\t').ElementAtOrDefault(6)?.Trim())) { employee.Salary = line.Split('\t')[6].Trim(); }
                    employees.Add(employee);
                }
            }
            return employees;
        }
        public IActionResult Index()
        {
            var employees = LoadEmployees();
            List<Employee> hourlyEmployees = employees.Where(e => e.EmploymentType == "Hourly Payroll").ToList();
            List<Employee> monthlyEmployees = employees.Where(e => e.EmploymentType == "Monthly payroll").ToList();
            List<Employee> freelancerEmployees = employees.Where(e => e.EmploymentType == "Free lancer").ToList();
            ViewBag.HourlyEmployees = new SelectList(hourlyEmployees, "Name", "Name");
            ViewBag.MonthlyEmployees = new SelectList(monthlyEmployees, "Name", "Name");
            ViewBag.FreelancerEmployees = new SelectList(freelancerEmployees, "Name", "Name");
            if (employees == null || !employees.Any()) { return View("Error"); }
            return View(employees);
        }
        public IActionResult GetEmployeeData(string name)
        {
            var employees = LoadEmployees();
            if (employees == null || !employees.Any()) { return NotFound("Employee list is not populated."); }
            var selectedEmployee = employees.FirstOrDefault(e => e.Name == name);
            if (selectedEmployee == null) { return NotFound(); }
            return PartialView("EmployeeData", selectedEmployee);
        }
        [HttpPost]
        public IActionResult AddEmployee(Employee employee)
        {
            string? filePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location?.ToString()), @"Employees.txt");
            string employeeData = $"{employee.Name}\t\"{employee.Address}\"\t{employee.BirthDate}\t{employee.Graduation}\t{employee.EmploymentType}\t{employee.Assurance}\t{employee.Salary}\n";
            System.IO.File.AppendAllText(filePath, employeeData);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditEmployee(Employee updatedEmployee)
        {

            var employees = LoadEmployees();
            var employeeToEdit = employees.FirstOrDefault(e => e.Name == updatedEmployee.Name);
            if (employeeToEdit != null)
            {
                employeeToEdit.Address = updatedEmployee.Address;
                employeeToEdit.BirthDate = updatedEmployee.BirthDate;
                employeeToEdit.Graduation = updatedEmployee.Graduation;
                employeeToEdit.EmploymentType = updatedEmployee.EmploymentType;
                employeeToEdit.Assurance = updatedEmployee.Assurance;
                employeeToEdit.Salary = updatedEmployee.Salary;

                string? filePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location?.ToString()), @"Employees.txt");
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var employee in employees)
                    {
                        string employeeData = $"{employee.Name}\t\"{employee.Address}\"\t{employee.BirthDate}\t{employee.Graduation}\t{employee.EmploymentType}\t{employee.Assurance}\t{employee.Salary}\n";
                        writer.Write(employeeData);
                    }
                }
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult DeleteEmployee(string name)
        {

            var employees = LoadEmployees();
            var employeeToDelete = employees.FirstOrDefault(e => e.Name == name);
            if (employeeToDelete != null)
            {
                employees.Remove(employeeToDelete);
                string? filePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location?.ToString()), @"Employees.txt");
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var employee in employees)
                    {
                        string employeeData = $"{employee.Name}\t\"{employee.Address}\"\t{employee.BirthDate}\t{employee.Graduation}\t{employee.EmploymentType}\t{employee.Assurance}\t{employee.Salary}\n";
                        writer.Write(employeeData);
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
