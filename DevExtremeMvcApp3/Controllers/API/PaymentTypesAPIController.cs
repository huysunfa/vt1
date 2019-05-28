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
    [Route("api/PaymentTypesAPI/{action}", Name = "PaymentTypesAPIApi")]
    public class PaymentTypesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var paymenttypes = _context.PaymentTypes.Select(i => new {
                i.PaymentTypeId,
                i.Description,
                i.PaymentTypeName
            });
            return Request.CreateResponse(DataSourceLoader.Load(paymenttypes, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new PaymentType();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.PaymentTypes.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.PaymentTypeId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.PaymentTypes.FirstOrDefault(item => item.PaymentTypeId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "PaymentType not found");

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
            var model = _context.PaymentTypes.FirstOrDefault(item => item.PaymentTypeId == key);

            _context.PaymentTypes.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(PaymentType model, IDictionary values) {
            string PAYMENT_TYPE_ID = nameof(PaymentType.PaymentTypeId);
            string DESCRIPTION = nameof(PaymentType.Description);
            string PAYMENT_TYPE_NAME = nameof(PaymentType.PaymentTypeName);

            if(values.Contains(PAYMENT_TYPE_ID)) {
                model.PaymentTypeId = Convert.ToInt32(values[PAYMENT_TYPE_ID]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(PAYMENT_TYPE_NAME)) {
                model.PaymentTypeName = Convert.ToString(values[PAYMENT_TYPE_NAME]);
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