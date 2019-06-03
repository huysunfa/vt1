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
        public ActionResult Index(int ID = 0)
        {
            ViewBag.ID = ID;

            return View();

        }

        public ActionResult Chart(int ID = 0)
        {
            ViewBag.ID = ID;
            return PartialView();

        }  public ActionResult NhanThanhToan(int ID = 0)
        {
            ViewBag.ID = ID;
            return PartialView();

        }
        [HttpPost]
        public ActionResult ChartData(int ID = 0)
        {
            using (Models.VTEntities db = new VTEntities())
            {
                string SQL = @"select t2.ProductName,ISNULL(SUM(Total),0) as Total from SalesOrderLine
                                AS T1 INNER JOIN Product AS T2 ON T1.ProductId = T2.ProductId
                                 where SalesOrderId = "+ ID + "group by t1.ProductId, t2.ProductName";
                var data = db.Database.SqlQuery<PIE_CHART>(SQL).ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }
        public class PIE_CHART
        {
            public String ProductName { get; set; }
            public Double Total { get; set; }
        }

        public ActionResult Grid(int ID = 0)
        {
            ViewBag.ID = ID;
            using (Models.VTEntities db = new Models.VTEntities())
            {
                updateSoTien(ID);
                var data = db.SalesOrders.Where(z => z.SalesOrderId == ID).FirstOrDefault();
                if (data == null)
                {
                    return RedirectToAction("index", "SalesOrder");
                }
                ViewBag.SalesTypeId = db.SalesTypes.Where(z => z.SalesTypeId == data.SalesTypeId).Select(z => z.SalesTypeName).FirstOrDefault();
                ViewBag.CustomerId = db.Customers.Where(z => z.CustomerId == data.CustomerId).Select(z => z.CustomerName).FirstOrDefault();
                ViewBag.SalesOrderLines = db.SalesOrderLines.Where(z => z.SalesOrderId == data.SalesOrderId).ToList();
           //     ViewBag.CongNo =db.
                return PartialView(data);
            }
        }

        public void updateSoTien(int ID = 0)
        {
            using (Models.VTEntities db = new Models.VTEntities())
            {
                var data = db.SalesOrderLines.AsNoTracking().Where(z => z.SalesOrderId == ID).ToList();
                var item = db.SalesOrders.Where(z => z.SalesOrderId == ID).FirstOrDefault();
                item.Amount = data.Sum(Z => Z.Price);
                item.Discount = data.Where(Z => Z.DiscountAmount.HasValue).Sum(Z => Z.Amount / 100 * Z.DiscountAmount.Value);
                item.SubTotal = data.Where(Z => Z.SubTotal.HasValue).Sum(Z => Z.SubTotal.Value);
                item.Tax = data.Where(Z => Z.TaxAmount.HasValue).Sum(Z => Z.Amount / 100 * Z.TaxAmount.Value);
                item.Total = item.Amount - item.Discount + item.SubTotal + item.Tax;
                db.SaveChanges();
            }
        }
        public ActionResult Create(int ID = 0, int SalesOrderId = 0)
        {
            ViewBag.ID = ID;
            using (Models.VTEntities db = new Models.VTEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var data = db.SalesOrderLines.AsNoTracking().Where(z => z.SalesOrderLineId == ID).FirstOrDefault();
                if (data == null)
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
            using (Models.VTEntities db = new Models.VTEntities())
            {
                if (item.SalesOrderLineId == 0)
                {

                    item.DateUpdate = DateTime.Now;
                    db.SalesOrderLines.Add(item);

                }
                else
                {
                    var data = db.SalesOrderLines.Where(z => z.SalesOrderLineId == item.SalesOrderLineId).FirstOrDefault();
                    data.Quantity = item.Quantity;
                    data.Price = item.Price;
                    data.TaxAmount = item.TaxAmount;
                    data.DiscountAmount = item.DiscountAmount;
                    data.Total = item.Total;
                    data.SubTotal = item.SubTotal;
                }
                db.SaveChanges();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        } [HttpPost]
        public ActionResult NhanThanhToan(PaymentReceive item)
        {
            using (Models.VTEntities db = new Models.VTEntities())
            {
                item.PaymentDate = DateTime.Now;
                db.PaymentReceives.Add(item);
                db.SaveChanges();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Delete(int ID)
        {
            using (Models.VTEntities db = new Models.VTEntities())
            {
                var item = db.SalesOrderLines.Where(z => z.SalesOrderLineId == ID).FirstOrDefault();
                ID = item.SalesOrderId;
                db.SalesOrderLines.Remove(item);
                db.SaveChanges();
                updateSoTien(ID);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}