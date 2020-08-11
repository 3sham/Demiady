using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Demiady;
using Demiady.Models;

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
            try
            {
                AccountClass s = new AccountClass();
                s.purchase = db.Purchases.Include(p => p.Supplier).Include(x => x.Product).Where(y => y.Date.Month == DateTime.Now.Month).Where(x=>x.Sup_ID == id).ToList();
                ViewBag.sumPurchase = db.Purchases.Where(x => x.Sup_ID == id).Where(y => y.Date.Month.Equals(DateTime.Now.Month)).AsEnumerable().Sum(x => x.Prod_count * x.Prod_Price);
                s.transfer = db.Transfers.Where(y => y.Date.Month == DateTime.Now.Month).Where(x => x.Sup_ID == id).ToList();
                ViewBag.transfer = db.Transfers.Where(x => x.Sup_ID == id).Where(y => y.Date.Month == DateTime.Now.Month).AsEnumerable().Sum(x => x.Value);
                ViewBag.id = id;
                ViewBag.name = db.Suppliers.Find( id).Sup_Name;
                return View(s);
            }
            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Suppliers";
                return View("Error");
            }
        }

        public ActionResult AnotherMonthSup(string Month, int id)
        {
            try
            {
                AccountClass s = new AccountClass();
                s.purchase = db.Purchases.Include(p => p.Supplier).Include(x => x.Product).Where(y => y.Date.Month.ToString() == Month).Where(x => x.Sup_ID == id).ToList();
                ViewBag.sumPurchase = db.Purchases.Where(x => x.Sup_ID == id).Where(y => y.Date.Month.ToString() == Month).AsEnumerable().Sum(x => x.Prod_count * x.Prod_Price);

                s.transfer = db.Transfers.Where(x => x.Sup_ID == id).Where(y => y.Date.Month.ToString() == Month).ToList();
                ViewBag.transfer = db.Transfers.Where(x => x.Sup_ID== id).Where(y => y.Date.Month.ToString() == Month).AsEnumerable().Sum(x => x.Value);

                ViewBag.month = Month;
                return PartialView("_anotherMonthSup", s);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Suppliers";
                return View("Error");
            }
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
                    var s = db.Suppliers.Where(x => x.Sup_Name == supplier.Sup_Name).ToList();
                    if (s.Count() > 0)
                    {
                        ViewBag.Error = "هذا المورد مسجل بالفعل";
                        ViewData["page"] = "Suppliers";
                        return View("Error");
                    }
                    else
                    {
                        supplier.Wasel = 0;
                        db.Suppliers.Add(supplier);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    
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
