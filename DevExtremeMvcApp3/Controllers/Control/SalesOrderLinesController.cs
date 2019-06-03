using DevExtremeMvcApp3.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
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

        }
        public class C_BaoGiaThongThuong
        {
            public string ProductName { get; set; }
            public string UnitOfMeasureName { get; set; }
            public double Quantity { get; set; }
            public double DefaultSellingPrice { get; set; }

        }

        public ActionResult BaoGiaThongThuong(int ID = 0)
        {
             string path = System.Web.HttpContext.Current.Server.MapPath("~") +  "BAOGIATHONGTHUONG.xlsx";

             using (System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                using (ExcelPackage excel = new ExcelPackage(stream))
                {
                     // Get Sheet Name
                    var workbook = excel.Workbook;
                    var ws = workbook.Worksheets.FirstOrDefault();
                    int recordRow = 12;
                    using (Models.VTEntities db = new VTEntities())
                    {
                        var data = db.Database.SqlQuery<C_BaoGiaThongThuong>(@"
                                SELECT T2.ProductName,T3.UnitOfMeasureName,T1.Quantity,T2.DefaultSellingPrice 
                                FROM SalesOrderLine as t1
                                inner join Product as t2 on t1.ProductId = t2.ProductId

                                left join UnitOfMeasure as t3 on t2.UnitOfMeasureId = t3.UnitOfMeasureId

                                where t1.SalesOrderId = " + ID + "").ToList();

                        foreach (var item in data)
                        {
                         ws = CoppyRow(ws, 12, recordRow);
                            ws.Cells[recordRow, 2].Value = item.ProductName;
                            ws.Cells[recordRow, 3].Value = item.UnitOfMeasureName;
                            ws.Cells[recordRow, 4].Value = item.Quantity;
                            ws.Cells[recordRow, 5].Value = item.DefaultSellingPrice;
                            ws.InsertRow(recordRow,1);

                            recordRow++;
                        }
                        var byteexcel = excel.GetAsByteArray();
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        var TENKH = db.Database.SqlQuery<String>("SELECT t2.CustomerName FROM SalesOrder as t1 inner join Customer as t2 on t1.CustomerId = t2.CustomerId WHERE SalesOrderId = " + ID).FirstOrDefault();
                        return File(byteexcel, "application/vnd.ms-excel", "Báo giá thông thường " + TENKH + ".xlsx");
                    }
                }
            }
        }
        public ExcelWorksheet CoppyRow(ExcelWorksheet ws, int form, int to)
        {
            //   ws.Cells[String.Format("{0}:{0}", form)].Copy(ws.Cells[String.Format("{0}:{0}", to)]);
            ws.Cells[form, 1, form, ws.Dimension.End.Column].Copy(ws.Cells[to, 1, to, ws.Dimension.End.Column]);
            ws.Row(to).Height = ws.Row(form).Height;
            ws.Row(to).StyleID = ws.Row(form).StyleID;
            return ws;
        }

        public ActionResult XuatBaoGia(int ID = 0)
        {
            ViewBag.ID = ID;
            return PartialView();

        }
        public ActionResult NhanThanhToan(int ID = 0)
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
                                 where SalesOrderId = " + ID + "group by t1.ProductId, t2.ProductName";
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
                var LDaThanhToan = db.PaymentReceives.Where(z => z.SalesOrderId == ID).ToList();
                Double DaThanhToan = 0;
                if (LDaThanhToan.Count != 0)
                {
                    DaThanhToan = LDaThanhToan.Sum(z => z.PaymentAmount);

                }
                ViewBag.ConLai = (data.Total - DaThanhToan).ToString("#,###");
                ViewBag.DaThanhToan = DaThanhToan.ToString("#,###");
                ViewBag.CongNo = db.Database.SqlQuery<Double>("select isnull(Total-PaymentAmount,0) AS CongNo from SalesOrder as t1 left join (select SalesOrderId,SUM(PaymentAmount) AS PaymentAmount from PaymentReceive  WHERE PaymentAmount>0  group by SalesOrderId ) as t2 on t1.SalesOrderId = t2.SalesOrderId where CustomerId = " + data.CustomerId + " and T1.SalesOrderId != " + data.SalesOrderId + " ").FirstOrDefault().ToString("#,###");
                ViewBag.ConLai = (ViewBag.ConLai == "" ? "0" : ViewBag.ConLai);
                ViewBag.DaThanhToan = (ViewBag.DaThanhToan == "" ? "0" : ViewBag.DaThanhToan);
                ViewBag.CongNo = (ViewBag.CongNo == "" ? "0" : ViewBag.CongNo);
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
        }
        [HttpPost]
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