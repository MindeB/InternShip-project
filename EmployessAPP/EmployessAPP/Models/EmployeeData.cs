using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace EmployessAPP.Models
{
    // Generates table by given data
    public class EmployeeData
    {
        // Used to identify employee
        [Key]
        public int EmployeeID { get; set; }
        
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Wage is required")]
        // Sets variable Wage to DataType Currency
        [DataType(DataType.Currency)]
        // Displays at table with euro symbol
        [DisplayFormat(DataFormatString = "{0:n} €")]
        public double Wage { get; set; }
    }
}