using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Demiady;

namespace Demiady.Controllers
{
    public class ExpensController : Controller
    {
        private DemiadyEntities db = new DemiadyEntities();

        // GET: Expens
        public ActionResult Index()
        {
            try
            {
                return View(db.Expenses.ToList());
            }
            catch
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Expens";
                return View("Error");
            }
            
        }

        // GET: Expens/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expens expens = db.Expenses.Find(id);
            if (expens == null)
            {
                return HttpNotFound();
            }
            return View(expens);
        }

        // GET: Expens/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Expens/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Ex_ID,Date,Name,Value")] Expens expens)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Expenses.Add(expens);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(expens);
            }
            catch
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Expens";
                return View("Error");
            }

          
        }

        // GET: Expens/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expens expens = db.Expenses.Find(id);
            if (expens == null)
            {
                return HttpNotFound();
            }
            return View(expens);
        }

        // POST: Expens/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Ex_ID,Date,Name,Value")] Expens expens)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(expens).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(expens);
            }
            catch
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Expens";
                return View("Error");
            }


           
        }

        // GET: Expens/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expens expens = db.Expenses.Find(id);
            if (expens == null)
            {
                return HttpNotFound();
            }
            return View(expens);
        }

        // POST: Expens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            try
            {
                Expens expens = db.Expenses.Find(id);
                db.Expenses.Remove(expens);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Expens";
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
