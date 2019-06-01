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
    [Route("api/SalesOrderLinesAPI/{action}", Name = "SalesOrderLinesAPIApi")]
    public class SalesOrderLinesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions,int ID=0) {

            var salesorderlines = _context.SalesOrderLines.Where(z=>z.SalesOrderId==ID).OrderByDescending(z=>z.DateUpdate).Select(i => new {
                i.SalesOrderLineId,
                i.Amount,
                i.Description,
                i.DiscountAmount,
                i.DiscountPercentage,
                i.Price,
                i.ProductId,
                i.Quantity,
                i.SalesOrderId,
                i.SubTotal,
                i.TaxAmount,
                i.TaxPercentage,
                i.Total
            });
            return Request.CreateResponse(DataSourceLoader.Load(salesorderlines, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form,int ID=0) {
            var model = new SalesOrderLine();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            model.SalesOrderId = ID;
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.SalesOrderLines.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.SalesOrderLineId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.SalesOrderLines.FirstOrDefault(item => item.SalesOrderLineId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "SalesOrderLine not found");

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
            var model = _context.SalesOrderLines.FirstOrDefault(item => item.SalesOrderLineId == key);

            _context.SalesOrderLines.Remove(model);
            _context.SaveChanges();
        }


        [HttpGet]
        public HttpResponseMessage SalesOrdersLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.SalesOrders
                         orderby i.CustomerRefNumber
                         select new {
                             Value = i.SalesOrderId,
                             Text = i.CustomerRefNumber
                         };
            return Request.CreateResponse(DataSourceLoader.Load(lookup, loadOptions));
        }

        private void PopulateModel(SalesOrderLine model, IDictionary values) {
            string SALES_ORDER_LINE_ID = nameof(SalesOrderLine.SalesOrderLineId);
            string AMOUNT = nameof(SalesOrderLine.Amount);
            string DESCRIPTION = nameof(SalesOrderLine.Description);
            string DISCOUNT_AMOUNT = nameof(SalesOrderLine.DiscountAmount);
            string DISCOUNT_PERCENTAGE = nameof(SalesOrderLine.DiscountPercentage);
            string PRICE = nameof(SalesOrderLine.Price);
            string PRODUCT_ID = nameof(SalesOrderLine.ProductId);
            string QUANTITY = nameof(SalesOrderLine.Quantity);
            string SALES_ORDER_ID = nameof(SalesOrderLine.SalesOrderId);
            string SUB_TOTAL = nameof(SalesOrderLine.SubTotal);
            string TAX_AMOUNT = nameof(SalesOrderLine.TaxAmount);
            string TAX_PERCENTAGE = nameof(SalesOrderLine.TaxPercentage);
            string TOTAL = nameof(SalesOrderLine.Total);

            if(values.Contains(SALES_ORDER_LINE_ID)) {
                model.SalesOrderLineId = Convert.ToInt32(values[SALES_ORDER_LINE_ID]);
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

            if(values.Contains(QUANTITY)) {
                model.Quantity = Convert.ToDouble(values[QUANTITY], CultureInfo.InvariantCulture);
            }

            if(values.Contains(SALES_ORDER_ID)) {
                model.SalesOrderId = Convert.ToInt32(values[SALES_ORDER_ID]);
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