using DevExtremeMvcApp3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DevExtremeMvcApp3.Controllers.Control
{
    public class SalesOrderLinesController : Controller
    {
        // GET: SalesOrderLines
        public ActionResult Index(int ID = 0)
        {
            ViewBag.ID = ID;
            using (Models.VTEntities db = new Models.VTEntities())
            {
                var data = db.SalesOrders.Where(z => z.SalesOrderId == ID).FirstOrDefault();
                if (data==null)
                {
                    return RedirectToAction("index", "SalesOrder");
                }
                ViewBag.SalesTypeId = db.SalesTypes.Where(z => z.SalesTypeId == data.SalesTypeId).Select(z=>z.SalesTypeName).FirstOrDefault();
                ViewBag.CustomerId = db.Customers.Where(z => z.CustomerId == data.CustomerId).Select(z=>z.CustomerName).FirstOrDefault();
                ViewBag.SalesOrderLines = db.SalesOrderLines.Where(z => z.SalesOrderId == data.SalesOrderId).ToList();

                return View(data);
            }
        }

        public ActionResult Create(int ID=0,int SalesOrderId=0)
        {
            ViewBag.ID = ID;
            using (Models.VTEntities db= new Models.VTEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var data = db.SalesOrderLines.AsNoTracking().Where(z => z.SalesOrderLineId == ID).FirstOrDefault();
                if (data==null)
                {
                    data = new Models.SalesOrderLine();
                    data.SalesOrderId = SalesOrderId;
                }
                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.DateFormatString = "yyyy-MM-dd";
                ViewBag.item = JsonConvert.SerializeObject(data, jsonSettings);
             }
            return PartialView();
        }
        [HttpPost]
        public ActionResult Create(SalesOrderLine item)
        {
            using (Models.VTEntities db= new Models.VTEntities())
            {
                db.SalesOrderLines.Add(item);
                db.SaveChanges();
             }
            return Json("",JsonRequestBehavior.AllowGet);
        }
    }
}