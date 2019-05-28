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
    [Route("api/SalesTypesAPI/{action}", Name = "SalesTypesAPIApi")]
    public class SalesTypesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var salestypes = _context.SalesTypes.Select(i => new {
                i.SalesTypeId,
                i.Description,
                i.SalesTypeName
            });
            return Request.CreateResponse(DataSourceLoader.Load(salestypes, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new SalesType();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.SalesTypes.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.SalesTypeId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.SalesTypes.FirstOrDefault(item => item.SalesTypeId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "SalesType not found");

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
            var model = _context.SalesTypes.FirstOrDefault(item => item.SalesTypeId == key);

            _context.SalesTypes.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(SalesType model, IDictionary values) {
            string SALES_TYPE_ID = nameof(SalesType.SalesTypeId);
            string DESCRIPTION = nameof(SalesType.Description);
            string SALES_TYPE_NAME = nameof(SalesType.SalesTypeName);

            if(values.Contains(SALES_TYPE_ID)) {
                model.SalesTypeId = Convert.ToInt32(values[SALES_TYPE_ID]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(SALES_TYPE_NAME)) {
                model.SalesTypeName = Convert.ToString(values[SALES_TYPE_NAME]);
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