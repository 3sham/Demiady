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
            else if (searchparam == "Sal_Month")
            {
                var s = db.Sales.Where(b => b.Sal_Date.Month.ToString() == searchTerm);
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
        public ActionResult Create([Bind(Include = "Sal_ID,Client_Name,Client_Phone,Prod_Price,Prod_Count,ProdMain_Price,Prod_gain,Sal_Date,Prod_ID")] Sale sale)
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
        public ActionResult Edit([Bind(Include = "Sal_ID,Client_Name,Client_Phone,Prod_Price,Prod_Count,ProdMain_Price,Prod_gain,Sal_Date,Prod_ID")] Sale sale)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    var store = db.Stores.Where(i => i.Prod_ID == sale.Prod_ID).FirstOrDefault();

                    if (store.Prod_Count >= 0 )
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
                        else if (sale2.Prod_Count == sale.Prod_Count)
                        {
                            
                            db.Entry(store).State = EntityState.Modified;
                            db.Entry(sale2).State = EntityState.Detached;
                        }

                        //db.Entry(sale2).State = EntityState.Detached;
                    }
                    else
                    {
                        ViewBag.Error = "هذا المنتج لا يوجد منه ف المخزن  او الكمية غير متوفرة";
                        ViewData["page"] = "Sales";
                        return View("Error");
                    }
                    //db.Entry(store).State = EntityState.Detached;
                    
                    db.Entry(sale).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Prod_ID = new SelectList(db.Products, "Prod_ID", "Prod_Name", sale.Prod_ID);
                return View(sale);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "هذا المنتج لا يوجد منه ف المخزن  او الكمية غير متوفرة";
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
               
                if (name == "" && date != null)
                {
                    ExcelPackage pck = new ExcelPackage();
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
                    var sales = db.Sales.Include(s => s.Product).Where(e => e.Sal_Date.Equals(date)).ToList();
                    if (sales.Count > 0)
                    {
                        ws.Cells["A1"].Value = "المجموع";
                        ws.Cells["B1"].Value = "المكسب";
                        ws.Cells["C1"].Value = "السعر الاصلي للمنتج";
                        ws.Cells["D1"].Value = "العدد";
                        ws.Cells["E1"].Value = "سعر بيع المنتج";
                        ws.Cells["F1"].Value = "  اسم المنتج   ";
                        ws.Cells["G1"].Value = "رقم العميل";
                        ws.Cells["H1"].Value = "اسم العميل";
                        ws.Cells["I1"].Value = "التاريخ";
                      
                        ws.Cells["A1:I1"].Style.Font.Bold = true;
                        ws.Cells["A1:I1"].Style.Font.Color.SetColor(Color.DeepSkyBlue);

                        int rowstart = 2;
                        foreach (var item in sales)
                        {
                          
                            ws.Cells[string.Format("A{0}", rowstart)].Value = item.Prod_Count * item.Prod_Price;
                            ws.Cells[string.Format("B{0}", rowstart)].Value = item.Prod_gain;
                            ws.Cells[string.Format("C{0}", rowstart)].Value = item.ProdMain_Price;
                            ws.Cells[string.Format("D{0}", rowstart)].Value = item.Prod_Count;
                            ws.Cells[string.Format("E{0}", rowstart)].Value = item.Prod_Price;
                            ws.Cells[string.Format("F{0}", rowstart)].Value = item.Product.Prod_Name;
                            ws.Cells[string.Format("G{0}", rowstart)].Value = item.Client_Phone;
                            ws.Cells[string.Format("H{0}", rowstart)].Value = item.Client_Name;
                            ws.Cells[string.Format("I{0}", rowstart)].Value = string.Format("{0:dd/MM/yyyy}",item.Sal_Date);
                            rowstart++;
                        }
                        string filename = "مبيعات يوم " + date.ToString("MM-dd-yyy") + ".xlsx";
                        ws.Cells[string.Format("B{0}",rowstart)].Value = "المجموع الكلي";
                        ws.Cells[string.Format("A{0}", rowstart)].Value = sales.Sum(s => s.Prod_Count * s.Prod_Price);
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
                    ExcelPackage pck = new ExcelPackage();
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
                    var sales = db.Sales.Include(s => s.Product).Where(e => e.Sal_Date.Equals(date)).Where(s => s.Client_Name == name).ToList();
                    if (sales.Count > 0)
                    {
                        ws.Cells["A1"].Value = "المجموع";
                        ws.Cells["B1"].Value = "المكسب";
                        ws.Cells["C1"].Value = "السعر الاصلي للمنتج";
                        ws.Cells["D1"].Value = "العدد";
                        ws.Cells["E1"].Value = "سعر بيع المنتج";
                        ws.Cells["F1"].Value = "  اسم المنتج   ";
                        ws.Cells["G1"].Value = "رقم العميل";
                        ws.Cells["H1"].Value = "اسم العميل";
                        ws.Cells["I1"].Value = "التاريخ";
                        ws.Cells["A1:I1"].Style.Font.Bold = true;
                        ws.Cells["A1:I1"].Style.Font.Color.SetColor(Color.DeepSkyBlue);
                        int rowstart = 2;
                        foreach (var item in sales)
                        {
                            
                            ws.Cells[string.Format("A{0}", rowstart)].Value = item.Prod_Count * item.Prod_Price;
                            ws.Cells[string.Format("B{0}", rowstart)].Value = item.Prod_gain;
                            ws.Cells[string.Format("C{0}", rowstart)].Value = item.ProdMain_Price;
                            ws.Cells[string.Format("D{0}", rowstart)].Value = item.Prod_Count;
                            ws.Cells[string.Format("E{0}", rowstart)].Value = item.Prod_Price;
                            ws.Cells[string.Format("F{0}", rowstart)].Value = item.Product.Prod_Name;
                            ws.Cells[string.Format("G{0}", rowstart)].Value = item.Client_Phone;
                            ws.Cells[string.Format("H{0}", rowstart)].Value = item.Client_Name;
                            ws.Cells[string.Format("I{0}", rowstart)].Value = string.Format("{0:dd/MM/yyyy}", item.Sal_Date);
                            rowstart++;
                        }
                        
                        string filename = "مبيعات  " + name + " يوم " + date.ToString("MM-dd-yyy") + ".xlsx";
                        ws.Cells[string.Format("B{0}", rowstart)].Value = "المجموع الكلي";
                        ws.Cells[string.Format("A{0}", rowstart)].Value = sales.Sum(s => s.Prod_Count * s.Prod_Price);
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
                        ViewBag.saaved = "تم التنزيل";
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
