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
    [Route("api/PaymentVouchersAPI/{action}", Name = "PaymentVouchersAPIApi")]
    public class PaymentVouchersAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var paymentvouchers = _context.PaymentVouchers.Select(i => new {
                i.PaymentvoucherId,
                i.BillId,
                i.CashBankId,
                i.IsFullPayment,
                i.PaymentAmount,
                i.PaymentDate,
                i.PaymentTypeId,
                i.PaymentVoucherName
            });
            return Request.CreateResponse(DataSourceLoader.Load(paymentvouchers, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new PaymentVoucher();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.PaymentVouchers.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.PaymentvoucherId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.PaymentVouchers.FirstOrDefault(item => item.PaymentvoucherId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "PaymentVoucher not found");

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
            var model = _context.PaymentVouchers.FirstOrDefault(item => item.PaymentvoucherId == key);

            _context.PaymentVouchers.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(PaymentVoucher model, IDictionary values) {
            string PAYMENTVOUCHER_ID = nameof(PaymentVoucher.PaymentvoucherId);
            string BILL_ID = nameof(PaymentVoucher.BillId);
            string CASH_BANK_ID = nameof(PaymentVoucher.CashBankId);
            string IS_FULL_PAYMENT = nameof(PaymentVoucher.IsFullPayment);
            string PAYMENT_AMOUNT = nameof(PaymentVoucher.PaymentAmount);
            string PAYMENT_DATE = nameof(PaymentVoucher.PaymentDate);
            string PAYMENT_TYPE_ID = nameof(PaymentVoucher.PaymentTypeId);
            string PAYMENT_VOUCHER_NAME = nameof(PaymentVoucher.PaymentVoucherName);

            if(values.Contains(PAYMENTVOUCHER_ID)) {
                model.PaymentvoucherId = Convert.ToInt32(values[PAYMENTVOUCHER_ID]);
            }

            if(values.Contains(BILL_ID)) {
                model.BillId = Convert.ToInt32(values[BILL_ID]);
            }

            if(values.Contains(CASH_BANK_ID)) {
                model.CashBankId = Convert.ToInt32(values[CASH_BANK_ID]);
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

            if(values.Contains(PAYMENT_TYPE_ID)) {
                model.PaymentTypeId = Convert.ToInt32(values[PAYMENT_TYPE_ID]);
            }

            if(values.Contains(PAYMENT_VOUCHER_NAME)) {
                model.PaymentVoucherName = Convert.ToString(values[PAYMENT_VOUCHER_NAME]);
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