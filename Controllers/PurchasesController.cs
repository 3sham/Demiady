using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Demiady;
using OfficeOpenXml;
using OfficeOpenXml.Style;

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

            searchparam = searchparam.Trim();
            searchTerm = searchTerm.Trim();
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
            else if (searchparam == "Sal_Month")
            {
                 
                var s = db.Purchases.Where(b => b.Date.Month.ToString() == searchTerm);
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
                        else
                        {
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
        public ActionResult Export(DateTime date, string name)
        {
            
            try
            {
                name = name.Trim();
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
                if (name == "" && date != null)
                {
                    var pur = db.Purchases.Include(s => s.Product).Include(s => s.Supplier).Where(e => e.Date.Equals(date)).ToList();
                    if (pur.Count > 0)
                    {

                        ws.Cells["A1"].Value = "المجموع";
                        ws.Cells["B1"].Value = "العدد";
                        ws.Cells["C1"].Value = "سعر الواحدة";
                        ws.Cells["D1"].Value = "  اسم المنتج   ";
                        ws.Cells["E1"].Value = "اسم المورد";
                        ws.Cells["F1"].Value = "التاريخ";

                        ws.Cells["A1:G1"].Style.Font.Bold = true;
                        ws.Cells["A1:G1"].Style.Font.Color.SetColor(Color.DeepSkyBlue);


                        int rowstart = 2;
                        foreach (var p in pur)
                        {

                            ws.Cells[string.Format("A{0}", rowstart)].Value = p.Prod_count * p.Prod_Price;
                            ws.Cells[string.Format("B{0}", rowstart)].Value = p.Prod_count;
                            ws.Cells[string.Format("C{0}", rowstart)].Value = p.Prod_Price;
                            ws.Cells[string.Format("D{0}", rowstart)].Value = p.Product.Prod_Name;
                            ws.Cells[string.Format("E{0}", rowstart)].Value = p.Supplier.Sup_Name;
                            ws.Cells[string.Format("F{0}", rowstart)].Value = string.Format("{0:dd/MM/yyyy}", p.Date);
                            rowstart++;
                        }
                        string filename = "مشتريات يوم " + date.ToString("MM-dd-yyy") + ".xlsx";
                        ws.Cells[string.Format("B{0}", rowstart)].Value = "المجموع الكلي";
                        ws.Cells[string.Format("A{0}", rowstart)].Value = pur.Sum(s => s.Prod_count * s.Prod_Price);
                        ws.Cells[string.Format("B{0}", rowstart)].Style.Font.Bold = true;
                        ws.Cells[string.Format("B{0}", rowstart)].Style.Font.Color.SetColor(Color.SandyBrown);
                        ws.Cells[string.Format("A{0}", rowstart)].Style.Font.Bold = true;
                        ws.Cells[string.Format("A{0}", rowstart)].Style.Font.Color.SetColor(Color.SandyBrown);

                        ws.Cells["A:AZ"].AutoFitColumns();
                        ws.Cells["A:AZ"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", @"attachment; filename = " + filename);
                        Response.BinaryWrite(pck.GetAsByteArray());
                        Response.End();
                        return Json(new { status = "Success" });
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (date != null && name != null)
                {
                    var pur = db.Purchases.Include(s => s.Product).Include(s => s.Supplier).Where(s => s.Supplier.Sup_Name == name).Where(e => e.Date.Equals(date)).ToList();
                    if (pur.Count > 0)
                    {

                        ws.Cells["A1"].Value = "المجموع";
                        ws.Cells["B1"].Value = "العدد";
                        ws.Cells["C1"].Value = "سعر الواحدة";
                        ws.Cells["D1"].Value = "  اسم المنتج   ";
                        ws.Cells["E1"].Value = "اسم المورد";
                        ws.Cells["F1"].Value = "التاريخ";

                        ws.Cells["A1:G1"].Style.Font.Bold = true;
                        ws.Cells["A1:G1"].Style.Font.Color.SetColor(Color.DeepSkyBlue);


                        int rowstart = 2;
                        foreach (var p in pur)
                        {

                            ws.Cells[string.Format("A{0}", rowstart)].Value = p.Prod_count * p.Prod_Price;
                            ws.Cells[string.Format("B{0}", rowstart)].Value = p.Prod_count;
                            ws.Cells[string.Format("C{0}", rowstart)].Value = p.Prod_Price;
                            ws.Cells[string.Format("D{0}", rowstart)].Value = p.Product.Prod_Name;
                            ws.Cells[string.Format("E{0}", rowstart)].Value = p.Supplier.Sup_Name;
                            ws.Cells[string.Format("F{0}", rowstart)].Value = string.Format("{0:dd/MM/yyyy}", p.Date);
                            rowstart++;
                        }
                        string filename = "مشتريات من  " + name + " يوم " + date.ToString("MM-dd-yyy") + ".xlsx";
                        ws.Cells[string.Format("B{0}", rowstart)].Value = "المجموع الكلي";
                        ws.Cells[string.Format("A{0}", rowstart)].Value = pur.Sum(s => s.Prod_count * s.Prod_Price);
                        ws.Cells[string.Format("B{0}", rowstart)].Style.Font.Bold = true;
                        ws.Cells[string.Format("B{0}", rowstart)].Style.Font.Color.SetColor(Color.SandyBrown);
                        ws.Cells[string.Format("A{0}", rowstart)].Style.Font.Bold = true;
                        ws.Cells[string.Format("A{0}", rowstart)].Style.Font.Color.SetColor(Color.SandyBrown);

                        ws.Cells["A:AZ"].AutoFitColumns();
                        ws.Cells["A:AZ"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", @"attachment; filename = " + filename);
                        Response.BinaryWrite(pck.GetAsByteArray());
                        Response.End();
                        return Json(new { status = "Success" });
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }

                else
                {
                    return RedirectToAction("Index");
                }

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
