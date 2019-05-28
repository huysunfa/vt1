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
    [Route("api/SalesOrdersAPI/{action}", Name = "SalesOrdersAPIApi")]
    public class SalesOrdersAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var salesorders = _context.SalesOrders.Select(i => new {
                i.SalesOrderId,
                i.Amount,
                i.BranchId,
                i.CurrencyId,
                i.CustomerId,
                i.CustomerRefNumber,
                i.DeliveryDate,
                i.Discount,
                i.Freight,
                i.OrderDate,
                i.Remarks,
                i.SalesOrderName,
                i.SalesTypeId,
                i.SubTotal,
                i.Tax,
                i.Total
            });
            return Request.CreateResponse(DataSourceLoader.Load(salesorders, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new SalesOrder();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.SalesOrders.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.SalesOrderId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.SalesOrders.FirstOrDefault(item => item.SalesOrderId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "SalesOrder not found");

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
            var model = _context.SalesOrders.FirstOrDefault(item => item.SalesOrderId == key);

            _context.SalesOrders.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(SalesOrder model, IDictionary values) {
            string SALES_ORDER_ID = nameof(SalesOrder.SalesOrderId);
            string AMOUNT = nameof(SalesOrder.Amount);
            string BRANCH_ID = nameof(SalesOrder.BranchId);
            string CURRENCY_ID = nameof(SalesOrder.CurrencyId);
            string CUSTOMER_ID = nameof(SalesOrder.CustomerId);
            string CUSTOMER_REF_NUMBER = nameof(SalesOrder.CustomerRefNumber);
            string DELIVERY_DATE = nameof(SalesOrder.DeliveryDate);
            string DISCOUNT = nameof(SalesOrder.Discount);
            string FREIGHT = nameof(SalesOrder.Freight);
            string ORDER_DATE = nameof(SalesOrder.OrderDate);
            string REMARKS = nameof(SalesOrder.Remarks);
            string SALES_ORDER_NAME = nameof(SalesOrder.SalesOrderName);
            string SALES_TYPE_ID = nameof(SalesOrder.SalesTypeId);
            string SUB_TOTAL = nameof(SalesOrder.SubTotal);
            string TAX = nameof(SalesOrder.Tax);
            string TOTAL = nameof(SalesOrder.Total);

            if(values.Contains(SALES_ORDER_ID)) {
                model.SalesOrderId = Convert.ToInt32(values[SALES_ORDER_ID]);
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

            if(values.Contains(CUSTOMER_ID)) {
                model.CustomerId = Convert.ToInt32(values[CUSTOMER_ID]);
            }

            if(values.Contains(CUSTOMER_REF_NUMBER)) {
                model.CustomerRefNumber = Convert.ToString(values[CUSTOMER_REF_NUMBER]);
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

            if(values.Contains(REMARKS)) {
                model.Remarks = Convert.ToString(values[REMARKS]);
            }

            if(values.Contains(SALES_ORDER_NAME)) {
                model.SalesOrderName = Convert.ToString(values[SALES_ORDER_NAME]);
            }

            if(values.Contains(SALES_TYPE_ID)) {
                model.SalesTypeId = Convert.ToInt32(values[SALES_TYPE_ID]);
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