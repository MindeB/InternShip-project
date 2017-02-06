using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EmployessAPP.Models
{
    public class EmployeeDbContext : DbContext
    {
        // Creates connection with database using variable name "employeedata"
        public DbSet<EmployeeData> employeedata { get; set; }
    }
}