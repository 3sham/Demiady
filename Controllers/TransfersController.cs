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
    public class TransfersController : Controller
    {
        private DemiadyEntities db = new DemiadyEntities();

        // GET: Transfers
        public ActionResult Index()
        {
            try
            {

                var transfers = db.Transfers.Include(t => t.Supplier);
                return View(transfers.ToList());
            }
            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Transfers";
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
                var s = db.Transfers.Where(b => b.Date.Day.Equals(dt.Day) &&
                                     b.Date.Month.Equals(dt.Month) &&
                                     b.Date.Year.Equals(dt.Year));

                return PartialView("_SearchTransfers", s.ToList());
            }
            else if (searchparam == "Sup_ID")
            {
                var s = db.Transfers.Include(b => b.Supplier).Where(b => b.Supplier.Sup_Name.StartsWith(searchTerm));
                return PartialView("_SearchTransfers", s.ToList());
            }
          
            else
            {

                return View();
            }

        }
        // GET: Transfers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transfer transfer = db.Transfers.Find(id);
            if (transfer == null)
            {
                return HttpNotFound();
            }
            return View(transfer);
        }

        // GET: Transfers/Create
        public ActionResult Create()
        {
            ViewBag.Sup_ID = new SelectList(db.Suppliers, "Sup_ID", "Sup_Name");
            return View();
        }

        // POST: Transfers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Date,Value,Sup_ID")] Transfer transfer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var suplier = db.Suppliers.Where(i => i.Sup_ID == transfer.Sup_ID).FirstOrDefault();
                    if (suplier != null)
                    {
                        suplier.Wasel +=transfer.Value;
                        db.Entry(suplier).State = EntityState.Modified;
                    }
                    db.Transfers.Add(transfer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.Sup_ID = new SelectList(db.Suppliers, "Sup_ID", "Sup_Name", transfer.Sup_ID);
                return View(transfer);

            }
            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Transfers";
                return View("Error");
            }

           
        }

        // GET: Transfers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transfer transfer = db.Transfers.Find(id);
            if (transfer == null)
            {
                return HttpNotFound();
            }
            ViewBag.Sup_ID = new SelectList(db.Suppliers, "Sup_ID", "Sup_Name", transfer.Sup_ID);
            return View(transfer);
        }

        // POST: Transfers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Date,Value,Sup_ID")] Transfer transfer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var suplier = db.Suppliers.Where(i => i.Sup_ID == transfer.Sup_ID).FirstOrDefault();
                    if (suplier != null)
                    {
                        var transfer2 = db.Transfers.Find(transfer.ID);

                        if (transfer2.Value > transfer.Value)
                        {
                            suplier.Wasel -= (transfer2.Value - transfer.Value);
                            db.Entry(suplier).State = EntityState.Modified;
                            db.Entry(transfer2).State = EntityState.Detached;
                        }
                        else if (transfer2.Value < transfer.Value)
                        {
                            suplier.Wasel += (transfer.Value - transfer2.Value);
                            db.Entry(suplier).State = EntityState.Modified;
                            db.Entry(transfer2).State = EntityState.Detached;
                        }

                    }
                    db.Entry(transfer).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Sup_ID = new SelectList(db.Suppliers, "Sup_ID", "Sup_Name", transfer.Sup_ID);
                return View(transfer);

            }
            catch (Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Transfers";
                return View("Error");
            }

          
        }

        // GET: Transfers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transfer transfer = db.Transfers.Find(id);
            if (transfer == null)
            {
                return HttpNotFound();
            }
            return View(transfer);
        }

        // POST: Transfers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

                Transfer transfer = db.Transfers.Find(id);
                var suplier = db.Suppliers.Where(i => i.Sup_ID == transfer.Sup_ID).FirstOrDefault();
                if (suplier != null)
                {
                    suplier.Wasel -= transfer.Value;
                    db.Entry(suplier).State = EntityState.Modified;
                }
                db.Transfers.Remove(transfer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception Ex)
            {
                ViewBag.Error = "لا يمكن حذف هذا المنتج لسبب ";
                ViewBag.Error += Ex.Message;
                ViewData["page"] = "Transfers";
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
