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
    public class AccountsController : Controller
    {
        private DemiadyEntities db = new DemiadyEntities();

        // GET: Accounts
        public ActionResult Index()
        {
            try
            {
                AccountClass s = new AccountClass();
                s.purchase = db.Purchases.Include(p => p.Supplier).Include(x => x.Product).Where(y => y.Date.Month == DateTime.Now.Month).ToList();
                ViewBag.sumPurchase = db.Purchases.Where(y => y.Date.Month == DateTime.Now.Month).Sum(x => x.Prod_count * x.Prod_Price);
                s.account = db.Accounts.ToList();
                s.sale = db.Sales.Where(y => y.Sal_Date.Month == DateTime.Now.Month).ToList();
                ViewBag.sale = db.Sales.Where(y => y.Sal_Date.Month == DateTime.Now.Month).Sum(x => x.Prod_gain);
                s.transfer = db.Transfers.Where(y => y.Date.Month == DateTime.Now.Month).ToList();
                ViewBag.transfer = db.Transfers.Where(y => y.Date.Month == DateTime.Now.Month).Sum(x => x.Value);
                s.expens = db.Expenses.Where(y => y.Date.Month == DateTime.Now.Month).ToList();
                ViewBag.expens = db.Expenses.Where(y => y.Date.Month == DateTime.Now.Month).Sum(x => x.Value);
                return View(s);
            }
            catch(Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Accounts";
                return View("Error");
            }
           
        }

        [HttpPost]
        public ActionResult Add(string Month)
        {
           try
            {
                Account ac = new Account();
                var purchase = db.Purchases.Where(b => b.Date.Month.ToString() == Month);
                var sale = db.Sales.Where(b => b.Sal_Date.Month.ToString() == Month);
                var transfer = db.Transfers.Where(b => b.Date.Month.ToString() == Month);
                ac.Month = Month;
                ac.Gained = sale.Sum(s => s.Prod_gain);
                ac.In_Account = sale.Sum(s => s.Prod_Count * s.ProdMain_Price);
                ac.Out_Account = transfer.Sum(s => s.Value);
                ac.Wallet = ac.In_Account - ac.Out_Account;
                db.Accounts.Add(ac);
                db.SaveChanges();
                return PartialView("_AddAccount", db.Accounts.ToList());
            }
            catch( Exception)
            {
                ViewBag.notFount = "Error";
                return PartialView("_AddAccount", db.Accounts.ToList());
            }
            
              
            

        }
        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Acc_ID,Month,Gained,In_Account,Out_Account,Wallet")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }

            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Acc_ID,Month,Gained,In_Account,Out_Account,Wallet")] Account account)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    db.Entry(account).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(account);
            }
            catch(Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Accounts";
                return View("Error");
            }
           
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Account account = db.Accounts.Find(id);
                db.Accounts.Remove(account);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch(Exception)
            {
                ViewBag.Error = "حدث خطأ ";
                ViewData["page"] = "Accounts";
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
