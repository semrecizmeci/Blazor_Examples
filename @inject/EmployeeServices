using System.Collections.Generic;

namespace _8hoursBlazorLessonExamples.Models
{
    public class EmployeeServices : IEmployee
    {
        public List<Employees> Employees;
        public EmployeeServices()
        {
            Employees = new List<Employees>()
            {
                new Employees() { name = "sado", age = 12 },
                new Employees() { name = "mado", age = 13 },
            };

        }

        public List<Employees> GetAllEmployee()
        {
         return Employees;
        }
    }
}
