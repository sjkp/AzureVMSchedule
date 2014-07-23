using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using AzureVMSchedule.DAL;
using AzureVMSchedule.DAL.Models;
using AzureVMSchedule.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureVMSchedule.Controllers
{
    public class HostedServicesController : ApiController
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
        [System.Web.Http.Route("api/HostedServices/{subscriptionId}")]        
        public async Task<IHttpActionResult> GetHostedService(Guid subscriptionId)
        {
            var certificate = await Subscriptions.Where(s => s.SubscriptionId == subscriptionId).FirstOrDefaultAsync();
            if (certificate == null)
            {
                return NotFound();
            }
            var json = await GetData(subscriptionId, certificate, "http://localhost:1337/cloudservice/");

            var result = JObject.Parse(json)["hostedServices"].Select(s => (string) s["serviceName"]);

            return Ok(result);
        }

        [System.Web.Http.Route("api/HostedServices/{subscriptionId}/{serviceName}")]
        public async Task<IHttpActionResult> GetHostedService(Guid subscriptionId, string serviceName)
        {
            var certificate = await Subscriptions.Where(s => s.SubscriptionId == subscriptionId).FirstOrDefaultAsync();
            if (certificate == null || string.IsNullOrEmpty(serviceName))
            {
                return NotFound();
            }
            var json = await GetData(subscriptionId, certificate, "http://localhost:1337/cloudservice/" + serviceName);

            var result = JObject.Parse(json)["deployments"].Select(s => (string)s["name"]);

            return Ok(result);
        }

        [System.Web.Http.Route("api/HostedServices/{subscriptionId}/{serviceName}/{virtualMachineName}")]
        public async Task<IHttpActionResult> GetHostedService(Guid subscriptionId, string serviceName, string virtualMachineName)
        {
            var certificate = await Subscriptions.Where(s => s.SubscriptionId == subscriptionId).FirstOrDefaultAsync();
            if (certificate == null)
            {
                return NotFound();
            }
            var json = await GetData(subscriptionId, certificate, "http://localhost:1337/cloudservice/" + serviceName );

            var result = from deployment in JObject.Parse(json)["deployments"]
                from roleinstance in deployment["roleInstances"]
                select roleinstance["instanceName"].Value<string>();

            return Ok(result);
        }

        private static async Task<string> GetData(Guid subscriptionId, Subscription certificate, string url)
        {
            var client = new HttpClient();
            var body = JsonConvert.SerializeObject(new
            {
                subscriptionId = subscriptionId.ToString(),
                certificate = certificate.MangementCertificate
            });
            var res =
                await
                    client.PostAsync(url,
                        new StringContent(body, Encoding.UTF8, "application/json"));
            var json = await res.Content.ReadAsStringAsync();
            return json;
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
