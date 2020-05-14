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
    public class PurchasesController : Controller
    {
        private DemiadyEntities db = new DemiadyEntities();

        // GET: Purchases
        public ActionResult Index()
        {
            try
            {
                var purchases = db.Purchases.Include(p => p.Supplier).Include(s => s.Product).OrderBy(p=>p.Date);
                return View(purchases.ToList());
            }
            catch (Exception )
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Purchases";
                return View("Error");
            }
          
        }
        [HttpPost]
        public ActionResult Search(string searchparam, string searchTerm)
        {
           
            if (searchparam == "Date")
            {
                DateTime dt = Convert.ToDateTime(searchTerm);
                var s = db.Purchases.Where(b => b.Date.Day.Equals(dt.Day) &&
                                     b.Date.Month.Equals(dt.Month) &&
                                     b.Date.Year.Equals(dt.Year));

                return PartialView("_SearchPurchases", s.ToList());
            }
            else if (searchparam == "Sup_ID")
            {
                var s = db.Purchases.Include(b => b.Supplier).Where(b => b.Supplier.Sup_Name.StartsWith(searchTerm));
                return PartialView("_SearchPurchases", s.ToList());
            }
            else if (searchparam == "Prod_ID")
            {
                var s = db.Purchases.Include(b => b.Product).Where(b => b.Product.Prod_Name.StartsWith(searchTerm));
                return PartialView("_SearchPurchases", s.ToList());
            }
            else
            {

                return View();
            }

        }
        public ActionResult AddMultiPurchase()
        {

            ViewBag.Sup_ID = new SelectList(db.Suppliers, "Sup_ID", "Sup_Name");
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name");
            return View();
        }
        [HttpPost]
        public ActionResult AddMultiPurchase(List<Purchase> purchase)
        {
            try
            {

                //Check for NULL.
                //if (purchase == null)
                //{
                //    purchase = new List<Purchase>();
                //}
                if (ModelState.IsValid)
                {
                    //Loop and insert records.
                    foreach (var model in purchase)
                    {

                    
                       
                            Store store = new Store();

                            db.Purchases.Add(model);
                            var s = db.Stores.Where(i => i.Prod_ID == model.Prod_ID).FirstOrDefault();
                            var suplier = db.Suppliers.Where(i => i.Sup_ID == model.Sup_ID).FirstOrDefault();
                            if (suplier != null)
                            {
                                suplier.Account += model.Prod_count * model.Prod_Price;
                                db.Entry(suplier).State = EntityState.Modified;
                            }
                            if (s != null)
                            {

                                s.Prod_Count += model.Prod_count;
                                db.Entry(s).State = EntityState.Modified;

                            }
                            else
                            {
                                store.Prod_ID = model.Prod_ID;
                                store.Prod_Count = model.Prod_count;
                                db.Stores.Add(store);
                            }


                            db.SaveChanges();

                     }
                    return Json(new { result = "Redirect", url = Url.Action("Index", "Purchases") });


                }

                return View();
            }

            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Purchases";
                return View("Error");
            }
        }

        // GET: Purchases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        // GET: Purchases/Create
        public ActionResult Create()
        {
            ViewBag.Sup_ID = new SelectList(db.Suppliers, "Sup_ID", "Sup_Name");
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name");
            return View();
        }

        // POST: Purchases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Pur_ID,Date,Prod_Price,Prod_count,Sup_ID,Prod_ID")] Purchase purchase)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    Store store = new Store();

                    db.Purchases.Add(purchase);
                    var s = db.Stores.Where(i => i.Prod_ID == purchase.Prod_ID).FirstOrDefault();
                    var suplier = db.Suppliers.Where(i => i.Sup_ID == purchase.Sup_ID).FirstOrDefault();
                    if (suplier != null)
                    {
                        suplier.Account += purchase.Prod_count * purchase.Prod_Price;
                        db.Entry(suplier).State = EntityState.Modified;
                    }
                    if (s != null)
                    {

                        s.Prod_Count += purchase.Prod_count;
                        db.Entry(s).State = EntityState.Modified;

                    }
                    else
                    {
                        store.Prod_ID = purchase.Prod_ID;
                        store.Prod_Count = purchase.Prod_count;
                        db.Stores.Add(store);
                    }


                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.Sup_ID = new SelectList(db.Suppliers, "Sup_ID", "Sup_Name", purchase.Sup_ID);
                ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", purchase.Prod_ID);
                return View(purchase);
            }
            catch (Exception )
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Purchases";
                return View("Error");
            }

           
        }

        // GET: Purchases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            ViewBag.Sup_ID = new SelectList(db.Suppliers, "Sup_ID", "Sup_Name", purchase.Sup_ID);
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", purchase.Prod_ID);
            return View(purchase);
        }

        // POST: Purchases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Pur_ID,Date,Prod_Price,Prod_count,Sup_ID,Prod_ID")] Purchase purchase)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    Store store = new Store();


                    var s = db.Stores.Where(i => i.Prod_ID == purchase.Prod_ID).FirstOrDefault();
                    var suplier = db.Suppliers.Where(i => i.Sup_ID == purchase.Sup_ID).FirstOrDefault();
                    if (suplier != null)
                    {
                        var purchase2 = db.Purchases.Find(purchase.Pur_ID);

                        if (purchase2.Prod_Price > purchase.Prod_Price || purchase2.Prod_count > purchase.Prod_count)
                        {
                            suplier.Account -= (purchase2.Prod_count*purchase2.Prod_Price)- (purchase.Prod_count * purchase.Prod_Price);
                            db.Entry(suplier).State = EntityState.Modified;
                            db.Entry(purchase2).State = EntityState.Detached;
                        }
                        else if (purchase2.Prod_Price < purchase.Prod_Price || purchase2.Prod_count < purchase.Prod_count)
                        {
                            suplier.Account += (purchase.Prod_count * purchase.Prod_Price) - (purchase2.Prod_count * purchase2.Prod_Price);
                            db.Entry(suplier).State = EntityState.Modified;
                            db.Entry(purchase2).State = EntityState.Detached;
                        }
                       
                    }
                    if (s != null)
                    {
                        s.Prod_ID = purchase.Prod_ID;
                        s.Prod_Count = purchase.Prod_count;
                        db.Entry(s).State = EntityState.Modified;

                    }
                    else
                    {
                        store.Prod_ID = purchase.Prod_ID;
                        store.Prod_Count = purchase.Prod_count;
                        db.Stores.Add(store);
                    }
                    db.Entry(purchase).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Sup_ID = new SelectList(db.Suppliers, "Sup_ID", "Sup_Name", purchase.Sup_ID);
                ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", purchase.Prod_ID);
                return View(purchase);
            }
            catch (Exception )
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Purchases";
                return View("Error");
            }

           
        }

        // GET: Purchases/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name");
            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            try
            {
                Purchase purchase = db.Purchases.Find(id);
                var store = db.Stores.Where(i => i.Prod_ID == purchase.Prod_ID).FirstOrDefault();
                var suplier = db.Suppliers.Where(i => i.Sup_ID == purchase.Sup_ID).FirstOrDefault();
                if (suplier != null)
                {
                    suplier.Account -= purchase.Prod_count * purchase.Prod_Price;
                    db.Entry(suplier).State = EntityState.Modified;
                }
                if (store != null)
                {
                    store.Prod_Count -= purchase.Prod_count;
                    db.Entry(store).State = EntityState.Modified;
                }
                db.Purchases.Remove(purchase);
                db.SaveChanges();
                ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name");
                return RedirectToAction("Index");
            }
            catch (Exception )
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Purchases";
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
