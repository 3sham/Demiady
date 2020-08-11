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
    public class ThomasController : Controller
    {
        private DemiadyEntities db = new DemiadyEntities();

        // GET: Thomas
        public ActionResult Index()
        {
            var thomas = db.Thomas.Include(t => t.Product);
            return View(thomas.ToList());
        }

        // GET: Thomas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thoma thoma = db.Thomas.Find(id);
            if (thoma == null)
            {
                return HttpNotFound();
            }
            return View(thoma);
        }

        // GET: Thomas/Create
        public ActionResult Create()
        {
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name");
            return View();
        }

        // POST: Thomas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Prod_Price,Prod_Count,Date,Prod_ID")] Thoma thoma)
        {
            if (ModelState.IsValid)
            {
                db.Thomas.Add(thoma);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", thoma.Prod_ID);
            return View(thoma);
        }

        // GET: Thomas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thoma thoma = db.Thomas.Find(id);
            if (thoma == null)
            {
                return HttpNotFound();
            }
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", thoma.Prod_ID);
            return View(thoma);
        }

        // POST: Thomas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Prod_Price,Prod_Count,Date,Prod_ID")] Thoma thoma)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thoma).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", thoma.Prod_ID);
            return View(thoma);
        }

        // GET: Thomas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thoma thoma = db.Thomas.Find(id);
            if (thoma == null)
            {
                return HttpNotFound();
            }
            return View(thoma);
        }

        // POST: Thomas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Thoma thoma = db.Thomas.Find(id);
            db.Thomas.Remove(thoma);
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
