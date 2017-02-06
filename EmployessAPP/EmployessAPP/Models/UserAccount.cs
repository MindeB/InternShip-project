using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EmployessAPP.Models
{
    //Generates table by given data
    public class UserAccount
    {
        // Identifies user by id
        [Key] 
        public int UserID { get; set; }
        // User's first name with request not null
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        // User's last name with request not null
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        // User's Username with request not null
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        // User's password with request not null and minimum length of 6
        [Required(ErrorMessage = "Password is required"), MinLength(6)]
        // Hides symbols when you type and sets datatype to password
        [DataType(DataType.Password)]
        public string Password { get; set; }
        // Used to compare two inputs and identify if password is correct
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Please confirm your passowrd.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        // User's role which gives different access levels
        public string Role { get; set; }
    }
}