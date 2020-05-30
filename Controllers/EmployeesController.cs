using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Demiady.Models;

namespace Demiady.Controllers
{
    public class EmployeesController : Controller
    {
        private DemiadyEntities db = new DemiadyEntities();

        // GET: Employees
        public ActionResult Index()
        {
            try
            {
                TransferEmployee tr = new TransferEmployee();
                tr.employees = db.Employees.ToList();
                tr.transferEmps = db.TransferEmps.Where(y => y.Date.Month == DateTime.Now.Month).ToList();
                return View(tr);
            }
            catch(Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Employees";
                return View("Error");
            }
           
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Emp_ID,Name,Salary,Wasel,Ba2y,Date")] Employee employee)
        {
            try
            {
                employee.Ba2y = employee.Salary - employee.Wasel;
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Employees";
                return View("Error");
            }
               
        }
        public ActionResult CreateEmp()
        {
            ViewBag.Emp_ID = new SelectList(db.Employees, "Emp_ID", "Name");
            return View();
        }

        // POST: TransferEmps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEmp([Bind(Include = "ID,Date,Value,Emp_ID")] TransferEmp transferEmp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var emp = db.Employees.Find(transferEmp.Emp_ID);
                    if (emp != null)
                    {
                        emp.Wasel += transferEmp.Value;
                        emp.Ba2y = emp.Salary - emp.Wasel;
                        db.Entry(emp).State = EntityState.Modified;
                    }
                    db.TransferEmps.Add(transferEmp);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Employees");
                }

                ViewBag.Emp_ID = new SelectList(db.Employees, "Emp_ID", "Name", transferEmp.Emp_ID);
                return View(transferEmp);
            }
            catch
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Employees";
                return View("Error");
            }
          
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Emp_ID,Name,Salary,Wasel,Ba2y,Date")] Employee employee)
        {
           
           try
            {
                employee.Ba2y = employee.Salary - employee.Wasel;
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Employees";
                return View("Error");
            }
                
        }
        public ActionResult EditEmp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransferEmp transferEmp = db.TransferEmps.Find(id);
            if (transferEmp == null)
            {
                return HttpNotFound();
            }
            ViewBag.Emp_ID = new SelectList(db.Employees, "Emp_ID", "Name", transferEmp.Emp_ID);
            return View(transferEmp);
        }

        // POST: TransferEmps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmp([Bind(Include = "ID,Date,Value,Emp_ID")] TransferEmp transferEmp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var emp = db.Employees.Find(transferEmp.Emp_ID);
                    if (emp != null)
                    {
                        var transfer2 = db.TransferEmps.Find(transferEmp.ID);

                        if (transfer2.Value > transferEmp.Value)
                        {
                            emp.Wasel -= (transfer2.Value - transferEmp.Value);
                            db.Entry(emp).State = EntityState.Modified;
                            db.Entry(transfer2).State = EntityState.Detached;
                        }
                        else if (transfer2.Value < transferEmp.Value)
                        {
                            emp.Wasel += (transferEmp.Value - transfer2.Value);
                            db.Entry(emp).State = EntityState.Modified;
                            db.Entry(transfer2).State = EntityState.Detached;
                        }
                    }
                    emp.Ba2y = emp.Salary - emp.Wasel;
                    db.Entry(transferEmp).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Emp_ID = new SelectList(db.Employees, "Emp_ID", "Name", transferEmp.Emp_ID);
                return View(transferEmp);
            }
            catch(Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Employees";
                return View("Error");
            }
            
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
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
            try
            {
                Employee employee = db.Employees.Find(id);
                db.Employees.Remove(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Employees";
                return View("Error");
            }
           
        }
        public ActionResult DeleteEmp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransferEmp transferEmp = db.TransferEmps.Find(id);
            if (transferEmp == null)
            {
                return HttpNotFound();
            }
            return View(transferEmp);
        }

        // POST: TransferEmps/Delete/5
        [HttpPost, ActionName("DeleteEmp")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEmp(int id)
        {
            try
            {
                TransferEmp transferEmp = db.TransferEmps.Find(id);
                var emp = db.Employees.Find(transferEmp.Emp_ID);
                if (emp != null)
                {
                    emp.Wasel -= transferEmp.Value;
                    emp.Ba2y = emp.Salary - emp.Wasel;
                    db.Entry(emp).State = EntityState.Modified;
                }
                db.TransferEmps.Remove(transferEmp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Employees";
                return View("Error");
            }
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
