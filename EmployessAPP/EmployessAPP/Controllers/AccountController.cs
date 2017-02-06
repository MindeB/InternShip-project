using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using EmployessAPP.Models;
using EmployessAPP.Controllers;
using System.Web.Security;
using PagedList;

namespace EmployessAPP.Controllers
{
    public class AccountController : Controller
    {
        // Connects to database
        private MyDbContext db = new MyDbContext();
        // GET: Account/Index
        /// <summary>
        /// Calculates and shows changed data to table
        /// </summary>
        /// <param name="sortOrder">Paramater to identify how data should be sorted</param>
        /// <param name="currentFilter">Filters data as default</param>
        /// <param name="searchString">Filters data with users input</param>
        /// <param name="page">Shows current page's data</param>
        /// <returns></returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            //Checks if a person is logged
            if (Session["UserID"] == null)
            {
                // Returns to router default page which is Account/Login
                return RedirectToAction("");
            }
            //Checks if user's role is Admin
            if (Session["Role"].ToString() == "Admin")
            {
                // Sets different data sortOrder
                ViewBag.FirstNameSortParm = String.IsNullOrEmpty(sortOrder) ? "fname" : "fname_desc";
                ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "lname_desc" : "";
                ViewBag.UserNameSortParm = String.IsNullOrEmpty(sortOrder) ? "uname" : "uname_desc";
                ViewBag.RoleNameSortParm = String.IsNullOrEmpty(sortOrder) ? "rname" : "rname_desc";
                ViewBag.CurrentFilter = searchString;
                //Checks if there is any input in the searchbar
                if (searchString != null)
                {
                    // Sets table page to 1
                    page = 1;
                }
                else
                {
                    // Gets default or previous value
                    searchString = currentFilter;
                }
                // Reads users from database and checks gets user by searchstring 
                var user = from e in db.userAccount
                    select e;
                if (!String.IsNullOrEmpty(searchString))
                {
                    // Filters by 3 arguments - FirstName, LastName, Role
                    user = user.Where(s => s.LastName.ToUpper().Contains(searchString.ToUpper())
                                           || s.FirstName.ToUpper().Contains(searchString.ToUpper()) ||
                                           s.Username.ToUpper().Contains(searchString.ToUpper()) ||
                                           s.Role.ToUpper().Contains(searchString.ToUpper()));
                }
                // Takes sortOrder value and checks how user wants to sort the table data
                switch (sortOrder)
                {
                    case "lname_desc":
                        user = user.OrderByDescending(e => e.LastName);
                        break;
                    case "fname":
                        user = user.OrderBy(e => e.FirstName);
                        break;
                    case "fname_desc":
                        user = user.OrderByDescending(e => e.FirstName);
                        break;
                    case "uname":
                        user = user.OrderBy(e => e.Username);
                        break;
                    case "uname_desc":
                        user = user.OrderByDescending(e => e.Username);
                        break;
                    case "rname":
                        user = user.OrderBy(e => e.Role);
                        break;
                    case "rname_desc":
                        user = user.OrderByDescending(e => e.Role);
                        break;
                    default:
                        user = user.OrderBy(e => e.LastName);
                        break;
                }
                // Sets how many line should be shown in the table
                int pageSize = 5;
                // Calculates how many pages will be shown
                int pageNumber = (page ?? 1);
                // Return paged view
                return View(user.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                //Redirects to login page
                return RedirectToAction("");
            }

        }
        // GET: Users/Create
        public ActionResult Create()
        {
            //Checks if user has access
            if (Session["Role"].ToString() == "Admin")
            {
                return View();
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,FirstName,LastName,Username,Password,ConfirmPassword,Role")] UserAccount account)
        {
            if (ModelState.IsValid )
            {
                
                    db.userAccount.Add(account);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                
            }


            return View(account);
        }
        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
                UserAccount user = db.userAccount.Find(id);
                if (user == null || Session["Role"].ToString() != "Admin")
                {
                    return HttpNotFound();
                }
                 
            return View(user);
        }
        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAccount user = db.userAccount.Find(id);
            if (user == null || Session["Role"].ToString() != "Admin")
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID, FirstName, LastName, Username, Password, ConfirmPassword, Role")] UserAccount user)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }
        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAccount user = db.userAccount.Find(id);
            if (user == null || Session["Role"].ToString() != "Admin")
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserAccount user = db.userAccount.Find(id);
            db.userAccount.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Register()
        {
            // Return registration form
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserAccount account)
        {
            // Finds user by username if it's not found then it's null
            var usr = db.userAccount.FirstOrDefault(u => u.Username == account.Username);
            // Checks if username is available
            if (usr == null)
            {
                if (ModelState.IsValid)
                {
                    // Sets user's role "User", this is for the register form that can be accessed by anyone
                    account.Role = "User";
                    db.userAccount.Add(account);
                    db.SaveChanges();
                    ModelState.Clear();
                    ViewBag.Message = account.FirstName + " " + account.LastName + " succcessfully registered.";
                }
            }
            else
            {
                // Warns user that the username is already taken
                ViewBag.Message = "'" + account.Username + "'" + " this username is taken.";
            }
            return View();
        }
        
        //Login
        public ActionResult Login()
        {
            // Checks if user is not logged in
            if (Session["UserID"] == null)
            {
                return View();
            }
            else
            {
                // Automatically log out user if he is logged in
                return RedirectToAction("LogOut");
            }
        }
        public ActionResult LogOut()
        {
            // Clears session and logged in user's data
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult Login(UserAccount user)
        {
            // Checks if user exists
                var usr = db.userAccount.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
            if (usr != null)
            {
                // Reads logged in user data to Session
                Session["UserID"] = usr.UserID.ToString();
                Session["Username"] = usr.Username.ToString();
                Session["FirstName"] = usr.FirstName.ToString();
                Session["LastName"] = usr.LastName.ToString();
                // This is if somehow user role becomes null, it will make an empty string for not throwing an error 
                if (usr.Role != null)
                {
                    Session["Role"] = usr.Role.ToString();
                }
                else
                {
                    Session["Role"] = "";
                }
                    // Redirects to main page
                    return RedirectToAction("LoggedIn");
                }
                else
                {
                    // If user's typed in info does not meet requirements, he is warned
                    ModelState.AddModelError("", "Username or Password is incorrect");
                }
                return View();
        }

        public ActionResult LoggedIn()
        {
            // Only moderator and admin can acccess this view
            if (Session["UserID"] != null && Session["Role"].ToString() != "User")
            {
                return View();
            }
            // User without priviledges can only view his data
            else if (Session["Role"].ToString() == "User")
            {
                return RedirectToAction("Index", "Employees");
            }
            
                return RedirectToAction("Login");
            
        }
    }
}