using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployessAPP.Models;
using PagedList;
using System.Data;
using System.Data.Entity;
using System.Net;

namespace EmployessAPP.Controllers
{
    public class EmployeesController : Controller
    {
        // Connects to database
        private EmployeeDbContext db = new EmployeeDbContext();

        // GET: Employees/Index
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
            
            double wage;
            // Checks logged in user's role
            if (Session["Role"].ToString() == "Admin" || Session["Role"].ToString() == "Moderator")
            {
                // Database has Net values of wage, so this loop recalculates it to Gross wage, without changing data in database itself
                foreach (EmployeeData a in db.employeedata)
                {
                    wage = a.Wage / 0.76;
                    wage = System.Math.Round(wage, 2);
                    a.Wage = wage;
                }
                // Sets different data sortOrder
                ViewBag.FirstNameSortParm = String.IsNullOrEmpty(sortOrder) ? "fname" : "fname_desc";
                ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "lname_desc" : "";
                ViewBag.WageSortParm = String.IsNullOrEmpty(sortOrder) ? "wage" : "wage_desc";
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
                var employee = from e in db.employeedata
                    select e;
                if (!String.IsNullOrEmpty(searchString))
                {
                    // Filters by 3 arguments - FirstName, LastName, Role
                    employee = employee.Where(s => s.LastName.ToUpper().Contains(searchString.ToUpper())
                                                   || s.FirstName.ToUpper().Contains(searchString.ToUpper()));
                }
                // Takes sortOrder value and checks how user wants to sort the table data
                switch (sortOrder)
                {
                    case "lname_desc":
                        employee = employee.OrderByDescending(e => e.LastName);
                        break;
                    case "fname":
                        employee = employee.OrderBy(e => e.FirstName);
                        break;
                    case "fname_desc":
                        employee = employee.OrderByDescending(e => e.FirstName);
                        break;
                    case "wage":
                        employee = employee.OrderBy(e => e.Wage);
                        break;
                    case "wage_desc":
                        employee = employee.OrderByDescending(e => e.Wage);
                        break;
                    default:
                        employee = employee.OrderBy(e => e.LastName);
                        break;
                }
                // Sets how many line should be shown in the table
                int pageSize = 5;
                // Calculates how many pages will be shown
                int pageNumber = (page ?? 1);
                // Return paged view
                return View(employee.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                // if user is not privileged he is redirected to index1 action
                return RedirectToAction("Index1");
            }
        }
        public ActionResult Index1()
        {
            // Recalculates wage data to gross
            double wage;
            foreach (EmployeeData a in db.employeedata)
            {
                wage = a.Wage / 0.76;
                wage = System.Math.Round(wage, 2);
                a.Wage = wage;
            }
            return View(db.employeedata.ToList());
        }
        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,FirstName,LastName,Wage")] EmployeeData employee)
        {
            if (ModelState.IsValid)
            {
                db.employeedata.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeData employee = db.employeedata.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,FirstName,LastName,Wage")] EmployeeData employee)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }
        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeData employee = db.employeedata.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }
        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeData employee = db.employeedata.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmployeeData employee = db.employeedata.Find(id);
            db.employeedata.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}