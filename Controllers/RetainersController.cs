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
    public class RetainersController : Controller
    {
        private DemiadyEntities db = new DemiadyEntities();

        // GET: Retainers
        public ActionResult Index()
        {
            var retainers = db.Retainers.Include(r => r.Product);
            return View(retainers.ToList());
        }

        // GET: Retainers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retainer retainer = db.Retainers.Find(id);
            if (retainer == null)
            {
                return HttpNotFound();
            }
            return View(retainer);
        }

        // GET: Retainers/Create
        public ActionResult Create()
        {
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name");
            return View();
        }

        // POST: Retainers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Ret_ID,Ret_Date,Client_Name,Client_Phone,Prod_Price,Prod_Count,Ret_Value,Due_date,Prod_ID")] Retainer retainer)
        {
            if (ModelState.IsValid)
            {
                db.Retainers.Add(retainer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", retainer.Prod_ID);
            return View(retainer);
        }

        // GET: Retainers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retainer retainer = db.Retainers.Find(id);
            if (retainer == null)
            {
                return HttpNotFound();
            }
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", retainer.Prod_ID);
            return View(retainer);
        }

        // POST: Retainers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Ret_ID,Ret_Date,Client_Name,Client_Phone,Prod_Price,Prod_Count,Ret_Value,Due_date,Prod_ID")] Retainer retainer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(retainer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", retainer.Prod_ID);
            return View(retainer);
        }

        // GET: Retainers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retainer retainer = db.Retainers.Find(id);
            if (retainer == null)
            {
                return HttpNotFound();
            }
            return View(retainer);
        }

        // POST: Retainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Retainer retainer = db.Retainers.Find(id);
            db.Retainers.Remove(retainer);
            db.SaveChanges();
            return RedirectToAction("Index");
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
