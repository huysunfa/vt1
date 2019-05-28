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
    [Route("api/ShipmentsAPI/{action}", Name = "ShipmentsAPIApi")]
    public class ShipmentsAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var shipments = _context.Shipments.Select(i => new {
                i.ShipmentId,
                i.IsFullShipment,
                i.SalesOrderId,
                i.ShipmentDate,
                i.ShipmentName,
                i.ShipmentTypeId,
                i.WarehouseId
            });
            return Request.CreateResponse(DataSourceLoader.Load(shipments, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new Shipment();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.Shipments.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.ShipmentId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.Shipments.FirstOrDefault(item => item.ShipmentId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "Shipment not found");

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
            var model = _context.Shipments.FirstOrDefault(item => item.ShipmentId == key);

            _context.Shipments.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(Shipment model, IDictionary values) {
            string SHIPMENT_ID = nameof(Shipment.ShipmentId);
            string IS_FULL_SHIPMENT = nameof(Shipment.IsFullShipment);
            string SALES_ORDER_ID = nameof(Shipment.SalesOrderId);
            string SHIPMENT_DATE = nameof(Shipment.ShipmentDate);
            string SHIPMENT_NAME = nameof(Shipment.ShipmentName);
            string SHIPMENT_TYPE_ID = nameof(Shipment.ShipmentTypeId);
            string WAREHOUSE_ID = nameof(Shipment.WarehouseId);

            if(values.Contains(SHIPMENT_ID)) {
                model.ShipmentId = Convert.ToInt32(values[SHIPMENT_ID]);
            }

            if(values.Contains(IS_FULL_SHIPMENT)) {
                model.IsFullShipment = Convert.ToBoolean(values[IS_FULL_SHIPMENT]);
            }

            if(values.Contains(SALES_ORDER_ID)) {
                model.SalesOrderId = Convert.ToInt32(values[SALES_ORDER_ID]);
            }

            if(values.Contains(SHIPMENT_DATE)) {
                model.ShipmentDate = (System.DateTimeOffset)Convert.ChangeType(values[SHIPMENT_DATE], typeof(System.DateTimeOffset));
            }

            if(values.Contains(SHIPMENT_NAME)) {
                model.ShipmentName = Convert.ToString(values[SHIPMENT_NAME]);
            }

            if(values.Contains(SHIPMENT_TYPE_ID)) {
                model.ShipmentTypeId = Convert.ToInt32(values[SHIPMENT_TYPE_ID]);
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