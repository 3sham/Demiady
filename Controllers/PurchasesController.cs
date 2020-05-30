using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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
        public ActionResult Export(DateTime date, string name)
        {
            
            try
            {
                name = name.Trim();
                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Add(System.Reflection.Missing.Value);
                Microsoft.Office.Interop.Excel.Worksheet worksheet = workbook.ActiveSheet;
                if (name == "" && date != null)
                {
                    var pur = db.Purchases.Include(s => s.Product).Include(s => s.Supplier).Where(e => e.Date.Equals(date)).ToList();
                    if (pur.Count > 0)
                    {
                        worksheet.Cells[1, 1] = "المجموع";
                        worksheet.Cells[1, 2] = "العدد";
                        worksheet.Cells[1, 3] = "سعر الواحدة";
                        worksheet.Cells[1, 4] = "  اسم المنتج   ";
                        worksheet.Cells[1, 5] = "اسم المورد";
                        worksheet.Cells[1, 6] = "التاريخ";
                        int row = 2;
                        foreach (var p in pur)
                        {
                            worksheet.Cells[row, 1] = p.Prod_count * p.Prod_Price;
                            worksheet.Cells[row, 2] = p.Prod_count;
                            worksheet.Cells[row, 3] = p.Prod_Price;
                            worksheet.Cells[row, 4] = p.Product.Prod_Name;
                            worksheet.Cells[row, 5] = p.Supplier.Sup_Name;
                            worksheet.Cells[row, 6] = p.Date;
                            row++;
                        }
                        worksheet.Cells[row, 2] = "المجموع الكلي";
                        worksheet.Cells[row, 1] = pur.Sum(s => s.Prod_count * s.Prod_Price);
                        var row1 = worksheet.Rows[row];
                        row1.Font.Size = 15;
                        row1.Font.Bold = true;
                        row1.Font.Color = System.Drawing.Color.RoyalBlue;


                        worksheet.get_Range("A1", "F1").EntireColumn.AutoFit();
                        worksheet.Cells.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        worksheet.get_Range("A1", "F1").Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        // format heading

                        var range_heading = worksheet.get_Range("A1", "F1");
                        range_heading.Font.Bold = true;
                        range_heading.Font.Color = System.Drawing.Color.DeepSkyBlue;

                        range_heading.Font.Size = 13;
                        //string filename = "E:\\مبيعات يوم " + date.ToString("MM-dd-yyy") + ".csv";
                        string filename = "E:\\مشتريات  " + name + " يوم " + date.ToString("MM-dd-yyy") + ".csv";
                        workbook.SaveAs(filename);
                        workbook.Close();
                        Marshal.ReleaseComObject(workbook);
                        app.Quit();
                        Marshal.FinalReleaseComObject(app);
                        ViewBag.saaved = "تم التنزيل";
                        return Json(new { status = "Success" });
                    }
                    else
                    {
                        return Json(new { status = "Faild" });
                    }
                }
                else if (date != null && name != null)
                {
                    var pur = db.Purchases.Include(s => s.Product).Include(s => s.Supplier).Where(s => s.Supplier.Sup_Name == name).Where(e => e.Date.Equals(date)).ToList();
                    if (pur.Count > 0)
                    {
                        worksheet.Cells[1, 1] = "المجموع";
                        worksheet.Cells[1, 2] = "العدد";
                        worksheet.Cells[1, 3] = "سعر الواحدة";
                        worksheet.Cells[1, 4] = "  اسم المنتج   ";
                        worksheet.Cells[1, 5] = "اسم المورد";
                        worksheet.Cells[1, 6] = "التاريخ";
                        int row = 2;
                        foreach (var p in pur)
                        {
                            worksheet.Cells[row, 1] = p.Prod_count * p.Prod_Price;
                            worksheet.Cells[row, 2] = p.Prod_count;
                            worksheet.Cells[row, 3] = p.Prod_Price;
                            worksheet.Cells[row, 4] = p.Product.Prod_Name;
                            worksheet.Cells[row, 5] = p.Supplier.Sup_Name;
                            worksheet.Cells[row, 6] = p.Date;
                            row++;
                        }
                        worksheet.Cells[row, 2] = "المجموع الكلي";
                        worksheet.Cells[row, 1] = pur.Sum(s => s.Prod_count * s.Prod_Price);
                        var row1 = worksheet.Rows[row];
                        row1.Font.Size = 15;
                        row1.Font.Bold = true;
                        row1.Font.Color = System.Drawing.Color.RoyalBlue;
                        worksheet.get_Range("A1", "F1").EntireColumn.AutoFit();
                        worksheet.Cells.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        worksheet.get_Range("A1", "F1").Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        // format heading
                        var range_heading = worksheet.get_Range("A1", "F1");
                        range_heading.Font.Bold = true;
                        range_heading.Font.Color = System.Drawing.Color.DeepSkyBlue;

                        range_heading.Font.Size = 13;
                        //string filename = "E:\\مبيعات يوم " + date.ToString("MM-dd-yyy") + ".csv";
                        string filename = "E:\\مشتريات من  " + name + " يوم " + date.ToString("MM-dd-yyy") + ".csv";
                        workbook.SaveAs(filename);
                        workbook.Close();
                        Marshal.ReleaseComObject(workbook);
                        app.Quit();
                        Marshal.FinalReleaseComObject(app);
                        ViewBag.saaved = "تم التنزيل";
                        return Json(new { status = "Success" });
                    }
                    else
                    {
                        return Json(new { status = "Faild" });
                    }
                }

                else
                {
                    return Json(new { status = "Faild" });
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
