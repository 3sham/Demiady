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
    public class ProductsController : Controller
    {
        private DemiadyEntities db = new DemiadyEntities();

        // GET: Products
        public ActionResult Index()
        { 
            try
            {
                return View(db.Products.ToList());
            }
            catch (Exception )
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Products";
                return View("Error");
            }
          
        }
        public ActionResult Search(string ProductName)
        {
            if (ProductName != null)
            {
                ProductName = ProductName.Trim();
                var s = db.Products.Where(b => b.Prod_Name.StartsWith(ProductName));

                return PartialView("_SearchProducts", s.ToList());
            }

            else
            {

                return View();
            }

        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Prod_ID,Prod_Name,Prod_Price")] Product product)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var s = db.Products.Where(x => x.Prod_Name == product.Prod_Name).ToList();
                    if (s.Count() > 0)
                    {
                        ViewBag.Error = "هذا المنتج مسجل بالفعل";
                        ViewData["page"] = "Products";
                        return View("Error");
                    }
                    else
                    {
                        db.Products.Add(product);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    
                }

                return View(product);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "حدث خطأ ";

                ViewData["page"] = "Products";
                return View("Error");
            }

          
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Prod_ID,Prod_Name,Prod_Price")] Product product)
        {

            try
            {
                if (ModelState.IsValid)
                {
                   
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    
                    
                }
                return View(product);
            }
            catch (Exception )
            {
                ViewBag.Error = "حدث خطأ ";

                ViewData["page"] = "Products";
                return View("Error");
            }


         
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Product product = db.Products.Find(id);
                db.Products.Remove(product);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch(Exception )
            {
                ViewBag.Error = "حدث خطأ : لا يمكن ازالة هذا المنتج ";
                
                ViewData["page"] = "Products";
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
