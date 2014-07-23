using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AzureVMSchedule.DAL;
using AzureVMSchedule.DAL.Models;
using Microsoft.AspNet.Identity;

namespace AzureVMSchedule.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {         
        private AzureVMScheduleContext db = new AzureVMScheduleContext();

        private IQueryable<Subscription> Subscriptions
        {
            get
            {
                string userId = User.Identity.GetUserId();    
                return db.Subscriptions.Where(s => s.UserId == userId);
            }
        }

        // GET: Subscription
        public async Task<ActionResult> Index()
        {
            
            return View(await Subscriptions.ToListAsync());
        }

        // GET: Subscription/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = await Subscriptions.Where(s => s.SubscriptionId == id).FirstOrDefaultAsync();
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // GET: Subscription/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Subscription/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SubscriptionId,Name,MangementCertificate")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                subscription.UserId = User.Identity.GetUserId();
                db.Subscriptions.Add(subscription);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(subscription);
        }

        // GET: Subscription/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = await Subscriptions.Where(s => s.SubscriptionId == id).FirstOrDefaultAsync();
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // POST: Subscription/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SubscriptionId,Name,MangementCertificate")] Subscription subscription)
        {
            
            if (ModelState.IsValid)
            {
                subscription.UserId = User.Identity.GetUserId();    
                db.Entry(subscription).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(subscription);
        }

        // GET: Subscription/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = await Subscriptions.Where(s => s.SubscriptionId == id).FirstOrDefaultAsync();
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // POST: Subscription/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Subscription subscription = await Subscriptions.Where(s => s.SubscriptionId == id).FirstOrDefaultAsync();
            db.Subscriptions.Remove(subscription);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
