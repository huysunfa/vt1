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
    [Route("api/CustomerTypesAPI/{action}", Name = "CustomerTypesAPIApi")]
    public class CustomerTypesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var customertypes = _context.CustomerTypes.Select(i => new {
                i.CustomerTypeId,
                i.CustomerTypeName,
                i.Description
            });
            return Request.CreateResponse(DataSourceLoader.Load(customertypes, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new CustomerType();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.CustomerTypes.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.CustomerTypeId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.CustomerTypes.FirstOrDefault(item => item.CustomerTypeId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "CustomerType not found");

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
            var model = _context.CustomerTypes.FirstOrDefault(item => item.CustomerTypeId == key);

            _context.CustomerTypes.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(CustomerType model, IDictionary values) {
            string CUSTOMER_TYPE_ID = nameof(CustomerType.CustomerTypeId);
            string CUSTOMER_TYPE_NAME = nameof(CustomerType.CustomerTypeName);
            string DESCRIPTION = nameof(CustomerType.Description);

            if(values.Contains(CUSTOMER_TYPE_ID)) {
                model.CustomerTypeId = Convert.ToInt32(values[CUSTOMER_TYPE_ID]);
            }

            if(values.Contains(CUSTOMER_TYPE_NAME)) {
                model.CustomerTypeName = Convert.ToString(values[CUSTOMER_TYPE_NAME]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
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