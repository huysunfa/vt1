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
    [Route("api/ShipmentTypesAPI/{action}", Name = "ShipmentTypesAPIApi")]
    public class ShipmentTypesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var shipmenttypes = _context.ShipmentTypes.Select(i => new {
                i.ShipmentTypeId,
                i.Description,
                i.ShipmentTypeName
            });
            return Request.CreateResponse(DataSourceLoader.Load(shipmenttypes, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new ShipmentType();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.ShipmentTypes.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.ShipmentTypeId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.ShipmentTypes.FirstOrDefault(item => item.ShipmentTypeId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "ShipmentType not found");

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
            var model = _context.ShipmentTypes.FirstOrDefault(item => item.ShipmentTypeId == key);

            _context.ShipmentTypes.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(ShipmentType model, IDictionary values) {
            string SHIPMENT_TYPE_ID = nameof(ShipmentType.ShipmentTypeId);
            string DESCRIPTION = nameof(ShipmentType.Description);
            string SHIPMENT_TYPE_NAME = nameof(ShipmentType.ShipmentTypeName);

            if(values.Contains(SHIPMENT_TYPE_ID)) {
                model.ShipmentTypeId = Convert.ToInt32(values[SHIPMENT_TYPE_ID]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(SHIPMENT_TYPE_NAME)) {
                model.ShipmentTypeName = Convert.ToString(values[SHIPMENT_TYPE_NAME]);
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