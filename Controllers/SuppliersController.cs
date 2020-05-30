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
    public class SuppliersController : Controller
    {
        private DemiadyEntities db = new DemiadyEntities();




        // GET: Suppliers
        public ActionResult Index()
        {
            try
            {
                return View(db.Suppliers.ToList());
            }
            catch (Exception )
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Suppliers";
                return View("Error");
            }

        }
      

        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Sup_ID,Sup_Name,Sup_Address,Sup_Phone,Bank_Account,Account,Wasel")] Supplier supplier)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    db.Suppliers.Add(supplier);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(supplier);
            }
            catch (Exception )
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Suppliers";
                return View("Error");
            }

        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Sup_ID,Sup_Name,Sup_Address,Sup_Phone,Bank_Account,Account,Wasel")] Supplier supplier)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    db.Entry(supplier).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(supplier);
            }
            catch (Exception )
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Suppliers";
                return View("Error");
            }

        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Supplier supplier = db.Suppliers.Find(id);
                db.Suppliers.Remove(supplier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception )
            {
                ViewBag.Error = " حدث خطأ لا يمكن ازالة هذا المورد ";
                ViewData["page"] = "Suppliers";
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
