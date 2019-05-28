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
    [Route("api/VendorTypesAPI/{action}", Name = "VendorTypesAPIApi")]
    public class VendorTypesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var vendortypes = _context.VendorTypes.Select(i => new {
                i.VendorTypeId,
                i.Description,
                i.VendorTypeName
            });
            return Request.CreateResponse(DataSourceLoader.Load(vendortypes, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new VendorType();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.VendorTypes.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.VendorTypeId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.VendorTypes.FirstOrDefault(item => item.VendorTypeId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "VendorType not found");

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
            var model = _context.VendorTypes.FirstOrDefault(item => item.VendorTypeId == key);

            _context.VendorTypes.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(VendorType model, IDictionary values) {
            string VENDOR_TYPE_ID = nameof(VendorType.VendorTypeId);
            string DESCRIPTION = nameof(VendorType.Description);
            string VENDOR_TYPE_NAME = nameof(VendorType.VendorTypeName);

            if(values.Contains(VENDOR_TYPE_ID)) {
                model.VendorTypeId = Convert.ToInt32(values[VENDOR_TYPE_ID]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(VENDOR_TYPE_NAME)) {
                model.VendorTypeName = Convert.ToString(values[VENDOR_TYPE_NAME]);
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