using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace DevExtremeMvcApp3.Models.Controllers
{
    [Route("api/PurchaseOrdersAPI/{action}", Name = "PurchaseOrdersAPIApi")]
    public class PurchaseOrdersAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var purchaseorders = _context.PurchaseOrders.Select(i => new {
                i.PurchaseOrderId,
                i.Amount,
                i.BranchId,
                i.CurrencyId,
                i.DeliveryDate,
                i.Discount,
                i.Freight,
                i.OrderDate,
                i.PurchaseOrderName,
                i.PurchaseTypeId,
                i.Remarks,
                i.SubTotal,
                i.Tax,
                i.Total,
                i.VendorId
            });
            return Request.CreateResponse(DataSourceLoader.Load(purchaseorders, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new PurchaseOrder();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.PurchaseOrders.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.PurchaseOrderId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.PurchaseOrders.FirstOrDefault(item => item.PurchaseOrderId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "PurchaseOrder not found");

            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        public void Delete(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.PurchaseOrders.FirstOrDefault(item => item.PurchaseOrderId == key);

            _context.PurchaseOrders.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(PurchaseOrder model, IDictionary values) {
            string PURCHASE_ORDER_ID = nameof(PurchaseOrder.PurchaseOrderId);
            string AMOUNT = nameof(PurchaseOrder.Amount);
            string BRANCH_ID = nameof(PurchaseOrder.BranchId);
            string CURRENCY_ID = nameof(PurchaseOrder.CurrencyId);
            string DELIVERY_DATE = nameof(PurchaseOrder.DeliveryDate);
            string DISCOUNT = nameof(PurchaseOrder.Discount);
            string FREIGHT = nameof(PurchaseOrder.Freight);
            string ORDER_DATE = nameof(PurchaseOrder.OrderDate);
            string PURCHASE_ORDER_NAME = nameof(PurchaseOrder.PurchaseOrderName);
            string PURCHASE_TYPE_ID = nameof(PurchaseOrder.PurchaseTypeId);
            string REMARKS = nameof(PurchaseOrder.Remarks);
            string SUB_TOTAL = nameof(PurchaseOrder.SubTotal);
            string TAX = nameof(PurchaseOrder.Tax);
            string TOTAL = nameof(PurchaseOrder.Total);
            string VENDOR_ID = nameof(PurchaseOrder.VendorId);

            if(values.Contains(PURCHASE_ORDER_ID)) {
                model.PurchaseOrderId = Convert.ToInt32(values[PURCHASE_ORDER_ID]);
            }

            if(values.Contains(AMOUNT)) {
                model.Amount = Convert.ToDouble(values[AMOUNT], CultureInfo.InvariantCulture);
            }

            if(values.Contains(BRANCH_ID)) {
                model.BranchId = Convert.ToInt32(values[BRANCH_ID]);
            }

            if(values.Contains(CURRENCY_ID)) {
                model.CurrencyId = Convert.ToInt32(values[CURRENCY_ID]);
            }

            if(values.Contains(DELIVERY_DATE)) {
                model.DeliveryDate = (System.DateTimeOffset)Convert.ChangeType(values[DELIVERY_DATE], typeof(System.DateTimeOffset));
            }

            if(values.Contains(DISCOUNT)) {
                model.Discount = Convert.ToDouble(values[DISCOUNT], CultureInfo.InvariantCulture);
            }

            if(values.Contains(FREIGHT)) {
                model.Freight = Convert.ToDouble(values[FREIGHT], CultureInfo.InvariantCulture);
            }

            if(values.Contains(ORDER_DATE)) {
                model.OrderDate = (System.DateTimeOffset)Convert.ChangeType(values[ORDER_DATE], typeof(System.DateTimeOffset));
            }

            if(values.Contains(PURCHASE_ORDER_NAME)) {
                model.PurchaseOrderName = Convert.ToString(values[PURCHASE_ORDER_NAME]);
            }

            if(values.Contains(PURCHASE_TYPE_ID)) {
                model.PurchaseTypeId = Convert.ToInt32(values[PURCHASE_TYPE_ID]);
            }

            if(values.Contains(REMARKS)) {
                model.Remarks = Convert.ToString(values[REMARKS]);
            }

            if(values.Contains(SUB_TOTAL)) {
                model.SubTotal = Convert.ToDouble(values[SUB_TOTAL], CultureInfo.InvariantCulture);
            }

            if(values.Contains(TAX)) {
                model.Tax = Convert.ToDouble(values[TAX], CultureInfo.InvariantCulture);
            }

            if(values.Contains(TOTAL)) {
                model.Total = Convert.ToDouble(values[TOTAL], CultureInfo.InvariantCulture);
            }

            if(values.Contains(VENDOR_ID)) {
                model.VendorId = Convert.ToInt32(values[VENDOR_ID]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}