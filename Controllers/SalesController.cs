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

            searchparam = searchparam.Trim();
            searchTerm = searchTerm.Trim();
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
                    var sale = db.Sales.Include(s=>s.Product).Where(e => e.Sal_Date.Equals(date)).ToList();
                    if (sale.Count > 0)
                    {
                        worksheet.Cells[1, 1] = "المجموع";
                        worksheet.Cells[1, 2] = "المكسب";
                        worksheet.Cells[1, 3] = "السعر الاصلي للمنتج";
                        worksheet.Cells[1, 4] = "العدد";
                        worksheet.Cells[1, 5] = "سعر بيع المنتج";
                        worksheet.Cells[1, 6] = "  اسم المنتج   ";
                        worksheet.Cells[1, 7] = "اسم العميل";
                        worksheet.Cells[1, 8] = "التاريخ";
                        int row = 2;
                        foreach (var p in sale)
                        {
                            worksheet.Cells[row, 1] = p.Prod_Count * p.Prod_Price;
                            worksheet.Cells[row, 2] = p.Prod_gain;
                            worksheet.Cells[row, 3] = p.ProdMain_Price;
                            worksheet.Cells[row, 4] = p.Prod_Count;
                            worksheet.Cells[row, 5] = p.Prod_Price;
                            worksheet.Cells[row, 6] = p.Product.Prod_Name;
                            worksheet.Cells[row, 7] = p.Client_Name;
                            worksheet.Cells[row, 8] = p.Sal_Date;
                            row++;
                        }
                        worksheet.Cells[row, 2] = "المجموع الكلي";
                        worksheet.Cells[row, 1] = sale.Sum(s => s.Prod_Count * s.Prod_Price);
                        var row1 = worksheet.Rows[row];
                        row1.Font.Size = 15;
                        row1.Font.Bold = true;
                        row1.Font.Color = System.Drawing.Color.RoyalBlue;
                        worksheet.get_Range("A1", "H1").EntireColumn.AutoFit();
                        worksheet.Cells.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        worksheet.get_Range("A1", "F1").Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        // format heading
                        var range_heading = worksheet.get_Range("A1", "H1");
                        range_heading.Font.Bold = true;
                        range_heading.Font.Color = System.Drawing.Color.DeepSkyBlue;

                        range_heading.Font.Size = 13;
                        //string filename = "E:\\مبيعات يوم " + date.ToString("MM-dd-yyy") + ".csv";
                        string filename = "E:\\مبيعات يوم " + date.ToString("MM-dd-yyy") + ".csv";
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
                    var sale = db.Sales.Include(s => s.Product).Where(e => e.Sal_Date.Equals(date)).Where(s=>s.Client_Name == name).ToList();
                    if (sale.Count > 0)
                    {
                        worksheet.Cells[1, 1] = "المجموع";
                        worksheet.Cells[1, 2] = "المكسب";
                        worksheet.Cells[1, 3] = "السعر الاصلي للمنتج";
                        worksheet.Cells[1, 4] = "العدد";
                        worksheet.Cells[1, 5] = "سعر بيع المنتج";
                        worksheet.Cells[1, 6] = "  اسم المنتج   ";
                        worksheet.Cells[1, 7] = "اسم العميل";
                        worksheet.Cells[1, 8] = "التاريخ";
                        int row = 2;
                        foreach (var p in sale)
                        {
                            worksheet.Cells[row, 1] = p.Prod_Count * p.Prod_Price;
                            worksheet.Cells[row, 2] = p.Prod_gain;
                            worksheet.Cells[row, 3] = p.ProdMain_Price;
                            worksheet.Cells[row, 4] = p.Prod_Count;
                            worksheet.Cells[row, 5] = p.Prod_Price;
                            worksheet.Cells[row, 6] = p.Product.Prod_Name;
                            worksheet.Cells[row, 7] = p.Client_Name;
                            worksheet.Cells[row, 8] = p.Sal_Date;
                            row++;
                        }
                        worksheet.Cells[row, 2] = "المجموع الكلي";
                        worksheet.Cells[row, 1] = sale.Sum(s => s.Prod_Count * s.Prod_Price);
                        var row1 = worksheet.Rows[row];
                        row1.Font.Size = 15;
                        row1.Font.Bold = true;
                        row1.Font.Color = System.Drawing.Color.RoyalBlue;
                        worksheet.get_Range("A1", "H1").EntireColumn.AutoFit();
                        worksheet.Cells.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        worksheet.get_Range("A1", "F1").Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        // format heading
                        var range_heading = worksheet.get_Range("A1", "H1");
                        range_heading.Font.Bold = true;
                        range_heading.Font.Color = System.Drawing.Color.DeepSkyBlue;

                        range_heading.Font.Size = 13;
                        //string filename = "E:\\مبيعات يوم " + date.ToString("MM-dd-yyy") + ".csv";
                        string filename = "E:\\مبيعات  " + name + " يوم " + date.ToString("MM-dd-yyy") + ".csv";
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
            catch (Exception)
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
