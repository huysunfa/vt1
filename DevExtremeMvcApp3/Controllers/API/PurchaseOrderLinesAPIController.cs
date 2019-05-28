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
    [Route("api/PurchaseOrderLinesAPI/{action}", Name = "PurchaseOrderLinesAPIApi")]
    public class PurchaseOrderLinesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var purchaseorderlines = _context.PurchaseOrderLines.Select(i => new {
                i.PurchaseOrderLineId,
                i.Amount,
                i.Description,
                i.DiscountAmount,
                i.DiscountPercentage,
                i.Price,
                i.ProductId,
                i.PurchaseOrderId,
                i.Quantity,
                i.SubTotal,
                i.TaxAmount,
                i.TaxPercentage,
                i.Total
            });
            return Request.CreateResponse(DataSourceLoader.Load(purchaseorderlines, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new PurchaseOrderLine();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.PurchaseOrderLines.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.PurchaseOrderLineId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.PurchaseOrderLines.FirstOrDefault(item => item.PurchaseOrderLineId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "PurchaseOrderLine not found");

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
            var model = _context.PurchaseOrderLines.FirstOrDefault(item => item.PurchaseOrderLineId == key);

            _context.PurchaseOrderLines.Remove(model);
            _context.SaveChanges();
        }


        [HttpGet]
        public HttpResponseMessage PurchaseOrdersLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.PurchaseOrders
                         orderby i.PurchaseOrderName
                         select new {
                             Value = i.PurchaseOrderId,
                             Text = i.PurchaseOrderName
                         };
            return Request.CreateResponse(DataSourceLoader.Load(lookup, loadOptions));
        }

        private void PopulateModel(PurchaseOrderLine model, IDictionary values) {
            string PURCHASE_ORDER_LINE_ID = nameof(PurchaseOrderLine.PurchaseOrderLineId);
            string AMOUNT = nameof(PurchaseOrderLine.Amount);
            string DESCRIPTION = nameof(PurchaseOrderLine.Description);
            string DISCOUNT_AMOUNT = nameof(PurchaseOrderLine.DiscountAmount);
            string DISCOUNT_PERCENTAGE = nameof(PurchaseOrderLine.DiscountPercentage);
            string PRICE = nameof(PurchaseOrderLine.Price);
            string PRODUCT_ID = nameof(PurchaseOrderLine.ProductId);
            string PURCHASE_ORDER_ID = nameof(PurchaseOrderLine.PurchaseOrderId);
            string QUANTITY = nameof(PurchaseOrderLine.Quantity);
            string SUB_TOTAL = nameof(PurchaseOrderLine.SubTotal);
            string TAX_AMOUNT = nameof(PurchaseOrderLine.TaxAmount);
            string TAX_PERCENTAGE = nameof(PurchaseOrderLine.TaxPercentage);
            string TOTAL = nameof(PurchaseOrderLine.Total);

            if(values.Contains(PURCHASE_ORDER_LINE_ID)) {
                model.PurchaseOrderLineId = Convert.ToInt32(values[PURCHASE_ORDER_LINE_ID]);
            }

            if(values.Contains(AMOUNT)) {
                model.Amount = Convert.ToDouble(values[AMOUNT], CultureInfo.InvariantCulture);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(DISCOUNT_AMOUNT)) {
                model.DiscountAmount = Convert.ToDouble(values[DISCOUNT_AMOUNT], CultureInfo.InvariantCulture);
            }

            if(values.Contains(DISCOUNT_PERCENTAGE)) {
                model.DiscountPercentage = Convert.ToDouble(values[DISCOUNT_PERCENTAGE], CultureInfo.InvariantCulture);
            }

            if(values.Contains(PRICE)) {
                model.Price = Convert.ToDouble(values[PRICE], CultureInfo.InvariantCulture);
            }

            if(values.Contains(PRODUCT_ID)) {
                model.ProductId = Convert.ToInt32(values[PRODUCT_ID]);
            }

            if(values.Contains(PURCHASE_ORDER_ID)) {
                model.PurchaseOrderId = Convert.ToInt32(values[PURCHASE_ORDER_ID]);
            }

            if(values.Contains(QUANTITY)) {
                model.Quantity = Convert.ToDouble(values[QUANTITY], CultureInfo.InvariantCulture);
            }

            if(values.Contains(SUB_TOTAL)) {
                model.SubTotal = Convert.ToDouble(values[SUB_TOTAL], CultureInfo.InvariantCulture);
            }

            if(values.Contains(TAX_AMOUNT)) {
                model.TaxAmount = Convert.ToDouble(values[TAX_AMOUNT], CultureInfo.InvariantCulture);
            }

            if(values.Contains(TAX_PERCENTAGE)) {
                model.TaxPercentage = Convert.ToDouble(values[TAX_PERCENTAGE], CultureInfo.InvariantCulture);
            }

            if(values.Contains(TOTAL)) {
                model.Total = Convert.ToDouble(values[TOTAL], CultureInfo.InvariantCulture);
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