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
    [Route("api/VendorsAPI/{action}", Name = "VendorsAPIApi")]
    public class VendorsAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var vendors = _context.Vendors.Select(i => new {
                i.VendorId,
                i.Address,
                i.City,
                i.ContactPerson,
                i.Email,
                i.Phone,
                i.State,
                i.VendorName,
                i.VendorTypeId,
                i.ZipCode
            });
            return Request.CreateResponse(DataSourceLoader.Load(vendors, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new Vendor();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.Vendors.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.VendorId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.Vendors.FirstOrDefault(item => item.VendorId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "Vendor not found");

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
            var model = _context.Vendors.FirstOrDefault(item => item.VendorId == key);

            _context.Vendors.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(Vendor model, IDictionary values) {
            string VENDOR_ID = nameof(Vendor.VendorId);
            string ADDRESS = nameof(Vendor.Address);
            string CITY = nameof(Vendor.City);
            string CONTACT_PERSON = nameof(Vendor.ContactPerson);
            string EMAIL = nameof(Vendor.Email);
            string PHONE = nameof(Vendor.Phone);
            string STATE = nameof(Vendor.State);
            string VENDOR_NAME = nameof(Vendor.VendorName);
            string VENDOR_TYPE_ID = nameof(Vendor.VendorTypeId);
            string ZIP_CODE = nameof(Vendor.ZipCode);

            if(values.Contains(VENDOR_ID)) {
                model.VendorId = Convert.ToInt32(values[VENDOR_ID]);
            }

            if(values.Contains(ADDRESS)) {
                model.Address = Convert.ToString(values[ADDRESS]);
            }

            if(values.Contains(CITY)) {
                model.City = Convert.ToString(values[CITY]);
            }

            if(values.Contains(CONTACT_PERSON)) {
                model.ContactPerson = Convert.ToString(values[CONTACT_PERSON]);
            }

            if(values.Contains(EMAIL)) {
                model.Email = Convert.ToString(values[EMAIL]);
            }

            if(values.Contains(PHONE)) {
                model.Phone = Convert.ToString(values[PHONE]);
            }

            if(values.Contains(STATE)) {
                model.State = Convert.ToString(values[STATE]);
            }

            if(values.Contains(VENDOR_NAME)) {
                model.VendorName = Convert.ToString(values[VENDOR_NAME]);
            }

            if(values.Contains(VENDOR_TYPE_ID)) {
                model.VendorTypeId = Convert.ToInt32(values[VENDOR_TYPE_ID]);
            }

            if(values.Contains(ZIP_CODE)) {
                model.ZipCode = Convert.ToString(values[ZIP_CODE]);
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