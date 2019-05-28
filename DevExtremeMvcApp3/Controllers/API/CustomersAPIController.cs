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
    [Route("api/CustomersAPI/{action}", Name = "CustomersAPIApi")]
    public class CustomersAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var customers = _context.Customers.Select(i => new {
                i.CustomerId,
                i.Address,
                i.City,
                i.ContactPerson,
                i.CustomerName,
                i.CustomerTypeId,
                i.Email,
                i.Phone,
                i.State,
                i.ZipCode
            });
            return Request.CreateResponse(DataSourceLoader.Load(customers, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new Customer();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.Customers.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.CustomerId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.Customers.FirstOrDefault(item => item.CustomerId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "Customer not found");

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
            var model = _context.Customers.FirstOrDefault(item => item.CustomerId == key);

            _context.Customers.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(Customer model, IDictionary values) {
            string CUSTOMER_ID = nameof(Customer.CustomerId);
            string ADDRESS = nameof(Customer.Address);
            string CITY = nameof(Customer.City);
            string CONTACT_PERSON = nameof(Customer.ContactPerson);
            string CUSTOMER_NAME = nameof(Customer.CustomerName);
            string CUSTOMER_TYPE_ID = nameof(Customer.CustomerTypeId);
            string EMAIL = nameof(Customer.Email);
            string PHONE = nameof(Customer.Phone);
            string STATE = nameof(Customer.State);
            string ZIP_CODE = nameof(Customer.ZipCode);

            if(values.Contains(CUSTOMER_ID)) {
                model.CustomerId = Convert.ToInt32(values[CUSTOMER_ID]);
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

            if(values.Contains(CUSTOMER_NAME)) {
                model.CustomerName = Convert.ToString(values[CUSTOMER_NAME]);
            }

            if(values.Contains(CUSTOMER_TYPE_ID)) {
                model.CustomerTypeId = Convert.ToInt32(values[CUSTOMER_TYPE_ID]);
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