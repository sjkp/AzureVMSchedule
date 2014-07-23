using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AzureVMSchedule.DAL;
using AzureVMSchedule.DAL.Models;

namespace AzureVMSchedule.Controllers
{
    public class SubscriptionsController : ApiController
    {
        private AzureVMScheduleContext db = new AzureVMScheduleContext();

        // GET: api/Subscriptions
        public IQueryable<Subscription> GetSubscriptions()
        {
            return db.Subscriptions;
        }

        // GET: api/Subscriptions/5
        [ResponseType(typeof(Subscription))]
        public async Task<IHttpActionResult> GetSubscription(string id)
        {
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }

            return Ok(subscription);
        }

        // PUT: api/Subscriptions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSubscription(Guid id, Subscription subscription)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != subscription.SubscriptionId)
            {
                return BadRequest();
            }

            db.Entry(subscription).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Subscriptions
        [ResponseType(typeof(Subscription))]
        public async Task<IHttpActionResult> PostSubscription(Subscription subscription)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Subscriptions.Add(subscription);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SubscriptionExists(subscription.SubscriptionId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = subscription.SubscriptionId }, subscription);
        }

        // DELETE: api/Subscriptions/5
        [ResponseType(typeof(Subscription))]
        public async Task<IHttpActionResult> DeleteSubscription(Guid id)
        {
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }

            db.Subscriptions.Remove(subscription);
            await db.SaveChangesAsync();

            return Ok(subscription);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SubscriptionExists(Guid id)
        {
            return db.Subscriptions.Count(e => e.SubscriptionId == id) > 0;
        }
    }
}