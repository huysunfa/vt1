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
    [Route("api/BillsAPI/{action}", Name = "BillsAPIApi")]
    public class BillsAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var bills = _context.Bills.Select(i => new {
                i.BillId,
                i.BillDate,
                i.BillDueDate,
                i.BillName,
                i.BillTypeId,
                i.GoodsReceivedNoteId,
                i.VendorDONumber,
                i.VendorInvoiceNumber
            });
            return Request.CreateResponse(DataSourceLoader.Load(bills, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new Bill();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.Bills.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.BillId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.Bills.FirstOrDefault(item => item.BillId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "Bill not found");

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
            var model = _context.Bills.FirstOrDefault(item => item.BillId == key);

            _context.Bills.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(Bill model, IDictionary values) {
            string BILL_ID = nameof(Bill.BillId);
            string BILL_DATE = nameof(Bill.BillDate);
            string BILL_DUE_DATE = nameof(Bill.BillDueDate);
            string BILL_NAME = nameof(Bill.BillName);
            string BILL_TYPE_ID = nameof(Bill.BillTypeId);
            string GOODS_RECEIVED_NOTE_ID = nameof(Bill.GoodsReceivedNoteId);
            string VENDOR_DONUMBER = nameof(Bill.VendorDONumber);
            string VENDOR_INVOICE_NUMBER = nameof(Bill.VendorInvoiceNumber);

            if(values.Contains(BILL_ID)) {
                model.BillId = Convert.ToInt32(values[BILL_ID]);
            }

            if(values.Contains(BILL_DATE)) {
                model.BillDate = (System.DateTimeOffset)Convert.ChangeType(values[BILL_DATE], typeof(System.DateTimeOffset));
            }

            if(values.Contains(BILL_DUE_DATE)) {
                model.BillDueDate = (System.DateTimeOffset)Convert.ChangeType(values[BILL_DUE_DATE], typeof(System.DateTimeOffset));
            }

            if(values.Contains(BILL_NAME)) {
                model.BillName = Convert.ToString(values[BILL_NAME]);
            }

            if(values.Contains(BILL_TYPE_ID)) {
                model.BillTypeId = Convert.ToInt32(values[BILL_TYPE_ID]);
            }

            if(values.Contains(GOODS_RECEIVED_NOTE_ID)) {
                model.GoodsReceivedNoteId = Convert.ToInt32(values[GOODS_RECEIVED_NOTE_ID]);
            }

            if(values.Contains(VENDOR_DONUMBER)) {
                model.VendorDONumber = Convert.ToString(values[VENDOR_DONUMBER]);
            }

            if(values.Contains(VENDOR_INVOICE_NUMBER)) {
                model.VendorInvoiceNumber = Convert.ToString(values[VENDOR_INVOICE_NUMBER]);
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