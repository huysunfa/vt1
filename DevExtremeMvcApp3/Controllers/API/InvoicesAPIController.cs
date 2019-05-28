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
    [Route("api/InvoicesAPI/{action}", Name = "InvoicesAPIApi")]
    public class InvoicesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var invoices = _context.Invoices.Select(i => new {
                i.InvoiceId,
                i.InvoiceDate,
                i.InvoiceDueDate,
                i.InvoiceName,
                i.InvoiceTypeId,
                i.ShipmentId
            });
            return Request.CreateResponse(DataSourceLoader.Load(invoices, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new Invoice();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.Invoices.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.InvoiceId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.Invoices.FirstOrDefault(item => item.InvoiceId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "Invoice not found");

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
            var model = _context.Invoices.FirstOrDefault(item => item.InvoiceId == key);

            _context.Invoices.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(Invoice model, IDictionary values) {
            string INVOICE_ID = nameof(Invoice.InvoiceId);
            string INVOICE_DATE = nameof(Invoice.InvoiceDate);
            string INVOICE_DUE_DATE = nameof(Invoice.InvoiceDueDate);
            string INVOICE_NAME = nameof(Invoice.InvoiceName);
            string INVOICE_TYPE_ID = nameof(Invoice.InvoiceTypeId);
            string SHIPMENT_ID = nameof(Invoice.ShipmentId);

            if(values.Contains(INVOICE_ID)) {
                model.InvoiceId = Convert.ToInt32(values[INVOICE_ID]);
            }

            if(values.Contains(INVOICE_DATE)) {
                model.InvoiceDate = (System.DateTimeOffset)Convert.ChangeType(values[INVOICE_DATE], typeof(System.DateTimeOffset));
            }

            if(values.Contains(INVOICE_DUE_DATE)) {
                model.InvoiceDueDate = (System.DateTimeOffset)Convert.ChangeType(values[INVOICE_DUE_DATE], typeof(System.DateTimeOffset));
            }

            if(values.Contains(INVOICE_NAME)) {
                model.InvoiceName = Convert.ToString(values[INVOICE_NAME]);
            }

            if(values.Contains(INVOICE_TYPE_ID)) {
                model.InvoiceTypeId = Convert.ToInt32(values[INVOICE_TYPE_ID]);
            }

            if(values.Contains(SHIPMENT_ID)) {
                model.ShipmentId = Convert.ToInt32(values[SHIPMENT_ID]);
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