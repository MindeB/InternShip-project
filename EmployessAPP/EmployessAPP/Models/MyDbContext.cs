using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace EmployessAPP.Models
{
    public class MyDbContext : DbContext
    {
        // Creates connection with database using variable name "userAccount"
        public DbSet<UserAccount> userAccount { get; set; }
        
    }
}