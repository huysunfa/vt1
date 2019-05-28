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
    [Route("api/PaymentReceivesAPI/{action}", Name = "PaymentReceivesAPIApi")]
    public class PaymentReceivesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var paymentreceives = _context.PaymentReceives.Select(i => new {
                i.PaymentReceiveId,
                i.InvoiceId,
                i.IsFullPayment,
                i.PaymentAmount,
                i.PaymentDate,
                i.PaymentReceiveName,
                i.PaymentTypeId
            });
            return Request.CreateResponse(DataSourceLoader.Load(paymentreceives, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new PaymentReceive();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.PaymentReceives.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.PaymentReceiveId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.PaymentReceives.FirstOrDefault(item => item.PaymentReceiveId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "PaymentReceive not found");

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
            var model = _context.PaymentReceives.FirstOrDefault(item => item.PaymentReceiveId == key);

            _context.PaymentReceives.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(PaymentReceive model, IDictionary values) {
            string PAYMENT_RECEIVE_ID = nameof(PaymentReceive.PaymentReceiveId);
            string INVOICE_ID = nameof(PaymentReceive.InvoiceId);
            string IS_FULL_PAYMENT = nameof(PaymentReceive.IsFullPayment);
            string PAYMENT_AMOUNT = nameof(PaymentReceive.PaymentAmount);
            string PAYMENT_DATE = nameof(PaymentReceive.PaymentDate);
            string PAYMENT_RECEIVE_NAME = nameof(PaymentReceive.PaymentReceiveName);
            string PAYMENT_TYPE_ID = nameof(PaymentReceive.PaymentTypeId);

            if(values.Contains(PAYMENT_RECEIVE_ID)) {
                model.PaymentReceiveId = Convert.ToInt32(values[PAYMENT_RECEIVE_ID]);
            }

            if(values.Contains(INVOICE_ID)) {
                model.InvoiceId = Convert.ToInt32(values[INVOICE_ID]);
            }

            if(values.Contains(IS_FULL_PAYMENT)) {
                model.IsFullPayment = Convert.ToBoolean(values[IS_FULL_PAYMENT]);
            }

            if(values.Contains(PAYMENT_AMOUNT)) {
                model.PaymentAmount = Convert.ToDouble(values[PAYMENT_AMOUNT], CultureInfo.InvariantCulture);
            }

            if(values.Contains(PAYMENT_DATE)) {
                model.PaymentDate = (System.DateTimeOffset)Convert.ChangeType(values[PAYMENT_DATE], typeof(System.DateTimeOffset));
            }

            if(values.Contains(PAYMENT_RECEIVE_NAME)) {
                model.PaymentReceiveName = Convert.ToString(values[PAYMENT_RECEIVE_NAME]);
            }

            if(values.Contains(PAYMENT_TYPE_ID)) {
                model.PaymentTypeId = Convert.ToInt32(values[PAYMENT_TYPE_ID]);
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