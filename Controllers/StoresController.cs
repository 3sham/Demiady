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
    public class StoresController : Controller
    {
        private DemiadyEntities db = new DemiadyEntities();

        // GET: Stores
        public ActionResult Index()
        {
            var stores = db.Stores.Include(s => s.Product);
            return View(stores.ToList());
        }
        public ActionResult Search(string ProductName)
        {
            ProductName = ProductName.Trim();
            if (ProductName != null)
            {
                var s = db.Stores.Include(x=>x.Product).Where(b => b.Product.Prod_Name.StartsWith(ProductName));

                return PartialView("_SearchStore", s.ToList());
            }

            else
            {

                return View();
            }

        }

        // GET: Stores/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Store store = db.Stores.Find(id);
        //    if (store == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(store);
        //}

        // GET: Stores/Create
        //public ActionResult Create()
        //{
        //    ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name");
        //    return View();
        //}

        // POST: Stores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,Prod_Count,Prod_ID")] Store store)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Stores.Add(store);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", store.Prod_ID);
        //    return View(store);
        //}

        // GET: Stores/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Store store = db.Stores.Find(id);
        //    if (store == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", store.Prod_ID);
        //    return View(store);
        //}

        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ID,Prod_Count,Prod_ID")] Store store)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(store).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", store.Prod_ID);
        //    return View(store);
        //}

        // GET: Stores/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Store store = db.Stores.Find(id);
        //    if (store == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(store);
        //}

        // POST: Stores/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Store store = db.Stores.Find(id);
        //    db.Stores.Remove(store);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
