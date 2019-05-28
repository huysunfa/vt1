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
    [Route("api/GoodsReceivedNotesAPI/{action}", Name = "GoodsReceivedNotesAPIApi")]
    public class GoodsReceivedNotesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var goodsreceivednotes = _context.GoodsReceivedNotes.Select(i => new {
                i.GoodsReceivedNoteId,
                i.GRNDate,
                i.GoodsReceivedNoteName,
                i.IsFullReceive,
                i.PurchaseOrderId,
                i.VendorDONumber,
                i.VendorInvoiceNumber,
                i.WarehouseId
            });
            return Request.CreateResponse(DataSourceLoader.Load(goodsreceivednotes, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new GoodsReceivedNote();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.GoodsReceivedNotes.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.GoodsReceivedNoteId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.GoodsReceivedNotes.FirstOrDefault(item => item.GoodsReceivedNoteId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "GoodsReceivedNote not found");

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
            var model = _context.GoodsReceivedNotes.FirstOrDefault(item => item.GoodsReceivedNoteId == key);

            _context.GoodsReceivedNotes.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(GoodsReceivedNote model, IDictionary values) {
            string GOODS_RECEIVED_NOTE_ID = nameof(GoodsReceivedNote.GoodsReceivedNoteId);
            string GRNDATE = nameof(GoodsReceivedNote.GRNDate);
            string GOODS_RECEIVED_NOTE_NAME = nameof(GoodsReceivedNote.GoodsReceivedNoteName);
            string IS_FULL_RECEIVE = nameof(GoodsReceivedNote.IsFullReceive);
            string PURCHASE_ORDER_ID = nameof(GoodsReceivedNote.PurchaseOrderId);
            string VENDOR_DONUMBER = nameof(GoodsReceivedNote.VendorDONumber);
            string VENDOR_INVOICE_NUMBER = nameof(GoodsReceivedNote.VendorInvoiceNumber);
            string WAREHOUSE_ID = nameof(GoodsReceivedNote.WarehouseId);

            if(values.Contains(GOODS_RECEIVED_NOTE_ID)) {
                model.GoodsReceivedNoteId = Convert.ToInt32(values[GOODS_RECEIVED_NOTE_ID]);
            }

            if(values.Contains(GRNDATE)) {
                model.GRNDate = (System.DateTimeOffset)Convert.ChangeType(values[GRNDATE], typeof(System.DateTimeOffset));
            }

            if(values.Contains(GOODS_RECEIVED_NOTE_NAME)) {
                model.GoodsReceivedNoteName = Convert.ToString(values[GOODS_RECEIVED_NOTE_NAME]);
            }

            if(values.Contains(IS_FULL_RECEIVE)) {
                model.IsFullReceive = Convert.ToBoolean(values[IS_FULL_RECEIVE]);
            }

            if(values.Contains(PURCHASE_ORDER_ID)) {
                model.PurchaseOrderId = Convert.ToInt32(values[PURCHASE_ORDER_ID]);
            }

            if(values.Contains(VENDOR_DONUMBER)) {
                model.VendorDONumber = Convert.ToString(values[VENDOR_DONUMBER]);
            }

            if(values.Contains(VENDOR_INVOICE_NUMBER)) {
                model.VendorInvoiceNumber = Convert.ToString(values[VENDOR_INVOICE_NUMBER]);
            }

            if(values.Contains(WAREHOUSE_ID)) {
                model.WarehouseId = Convert.ToInt32(values[WAREHOUSE_ID]);
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