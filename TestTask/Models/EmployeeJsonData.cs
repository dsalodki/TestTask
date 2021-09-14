using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTask.Models
{
    public class EmployeeJsonData
    {
        public int Id { get; set; }
        public int? PersonnelNumber { get; set; }
        public string FullName { get; set; }
        public Sex Sex { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsRegular { get; set; }
    }

    public enum Sex
    {
        Male = 0,
        Female = 1
    }
}
