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
    [Route("api/InvoiceTypesAPI/{action}", Name = "InvoiceTypesAPIApi")]
    public class InvoiceTypesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var invoicetypes = _context.InvoiceTypes.Select(i => new {
                i.InvoiceTypeId,
                i.Description,
                i.InvoiceTypeName
            });
            return Request.CreateResponse(DataSourceLoader.Load(invoicetypes, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new InvoiceType();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.InvoiceTypes.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.InvoiceTypeId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.InvoiceTypes.FirstOrDefault(item => item.InvoiceTypeId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "InvoiceType not found");

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
            var model = _context.InvoiceTypes.FirstOrDefault(item => item.InvoiceTypeId == key);

            _context.InvoiceTypes.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(InvoiceType model, IDictionary values) {
            string INVOICE_TYPE_ID = nameof(InvoiceType.InvoiceTypeId);
            string DESCRIPTION = nameof(InvoiceType.Description);
            string INVOICE_TYPE_NAME = nameof(InvoiceType.InvoiceTypeName);

            if(values.Contains(INVOICE_TYPE_ID)) {
                model.InvoiceTypeId = Convert.ToInt32(values[INVOICE_TYPE_ID]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(INVOICE_TYPE_NAME)) {
                model.InvoiceTypeName = Convert.ToString(values[INVOICE_TYPE_NAME]);
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