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
    public class SalesController : Controller
    {
        private DemiadyEntities db = new DemiadyEntities();

        // GET: Sales
        public ActionResult Index()
        {
            try
            {
                var sales = db.Sales.Include(s => s.Product).OrderBy(p=>p.Sal_Date);
                return View(sales.ToList());
            }
            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Sales";
                return View("Error");
            }
           
        }
        public ActionResult Search(string searchparam, string searchTerm)
        {

            if (searchparam == "Sal_Date")
            {
                DateTime dt = Convert.ToDateTime(searchTerm);
                var s = db.Sales.Where(b => b.Sal_Date.Day.Equals(dt.Day) &&
                                     b.Sal_Date.Month.Equals(dt.Month) &&
                                     b.Sal_Date.Year.Equals(dt.Year));

                return PartialView("_SearchSales", s.ToList());
            }
            else if (searchparam == "Client_Name")
            {
                var s = db.Sales.Where(b => b.Client_Name.StartsWith(searchTerm));
                return PartialView("_SearchSales", s.ToList());
            }
            else if (searchparam == "Prod_ID")
            {
                var s = db.Sales.Include(b => b.Product).Where(b => b.Product.Prod_Name.StartsWith(searchTerm));
                return PartialView("_SearchSales", s.ToList());
            }
            else
            {

                return View();
            }

        }
        public ActionResult AddMultiSales()
        {
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name");
            return View();
        }
        [HttpPost]
        public ActionResult AddMultiSales(List<Sale> sales)
        {
            try
            {

               
                if (ModelState.IsValid)
                {
                    //Loop and insert records.
                    foreach (var model in sales)
                    {



                        Store store = new Store();

                        db.Sales.Add(model);
                        var s = db.Stores.Where(i => i.Prod_ID == model.Prod_ID).FirstOrDefault();
                        if (s.Prod_Count > 0 && s.Prod_Count >= model.Prod_Count)
                        {

                            s.Prod_Count -= model.Prod_Count;
                            db.Entry(s).State = EntityState.Modified;

                        }
                        else
                        {
                            ViewBag.Error = "هذا المنتج لا يوجد منه ف المخزن  او الكمية غير متوفرة";
                            ViewData["page"] = "Sales";
                            return View("Error");
                        }



                        db.SaveChanges();
                       

                    }
                    return Json(new { result = "Redirect", url = Url.Action("Index", "Sales") });


                }

                return View();
            }

            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Sales";
                return View("Error");
            }
        }
        // GET: Sales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Sales.Find(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View(sale);
        }

        // GET: Sales/Create
        public ActionResult Create()
        {
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name");
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Sal_ID,Client_Name,Prod_Price,Prod_Count,ProdMain_Price,Prod_gain,Sal_Date,Prod_ID")] Sale sale)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    Store store = new Store();

                    db.Sales.Add(sale);
                    var s = db.Stores.Where(i => i.Prod_ID == sale.Prod_ID).FirstOrDefault();
                    if (s.Prod_Count > 0 && s.Prod_Count >= sale.Prod_Count)
                    {

                        s.Prod_Count -= sale.Prod_Count;
                        db.Entry(s).State = EntityState.Modified;

                    }
                    else
                    {
                        ViewBag.Error = "هذا المنتج لا يوجد منه ف المخزن  او الكمية غير متوفرة";
                        ViewData["page"] = "Sales";
                        return View("Error");
                    }



                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", sale.Prod_ID);
                return View(sale);
            }
            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Sales";
                return View("Error");
            }

            
        }

        // GET: Sales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Sales.Find(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", sale.Prod_ID);
            return View(sale);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Sal_ID,Client_Name,Prod_Price,Prod_Count,ProdMain_Price,Prod_gain,Sal_Date,Prod_ID")] Sale sale)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    var store = db.Stores.Where(i => i.Prod_ID == sale.Prod_ID).FirstOrDefault();

                    if (store.Prod_Count > 0 && store.Prod_Count >= sale.Prod_Count)
                    {
                        var sale2 = db.Sales.Find(sale.Sal_ID);

                        if (sale2.Prod_Count > sale.Prod_Count)
                        {
                            store.Prod_Count += sale2.Prod_Count - sale.Prod_Count;
                            db.Entry(store).State = EntityState.Modified;
                            db.Entry(sale2).State = EntityState.Detached;
                        }
                        else if (sale.Prod_Count > sale2.Prod_Count)
                        {
                            store.Prod_Count -= sale.Prod_Count - sale2.Prod_Count;
                            db.Entry(store).State = EntityState.Modified;
                            db.Entry(sale2).State = EntityState.Detached;
                        }

                    }
                    else
                    {
                        ViewBag.Error = "هذا المنتج لا يوجد منه ف المخزن  او الكمية غير متوفرة";
                        ViewData["page"] = "Sales";
                        return View("Error");
                    }
                    db.Entry(sale).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", sale.Prod_ID);
                return View(sale);
            }
            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Sales";
                return View("Error");
            }

            
        }

        // GET: Sales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Sales.Find(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Sale sale = db.Sales.Find(id);
                var store = db.Stores.Where(i => i.Prod_ID == sale.Prod_ID).FirstOrDefault();

                if (store != null)
                {
                    store.Prod_Count += sale.Prod_Count;
                    db.Entry(store).State = EntityState.Modified;
                }
               
                db.Sales.Remove(sale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Sales";
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
