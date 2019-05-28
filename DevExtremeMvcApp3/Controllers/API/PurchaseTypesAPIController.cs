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
    [Route("api/PurchaseTypesAPI/{action}", Name = "PurchaseTypesAPIApi")]
    public class PurchaseTypesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var purchasetypes = _context.PurchaseTypes.Select(i => new {
                i.PurchaseTypeId,
                i.Description,
                i.PurchaseTypeName
            });
            return Request.CreateResponse(DataSourceLoader.Load(purchasetypes, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new PurchaseType();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.PurchaseTypes.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.PurchaseTypeId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.PurchaseTypes.FirstOrDefault(item => item.PurchaseTypeId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "PurchaseType not found");

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
            var model = _context.PurchaseTypes.FirstOrDefault(item => item.PurchaseTypeId == key);

            _context.PurchaseTypes.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(PurchaseType model, IDictionary values) {
            string PURCHASE_TYPE_ID = nameof(PurchaseType.PurchaseTypeId);
            string DESCRIPTION = nameof(PurchaseType.Description);
            string PURCHASE_TYPE_NAME = nameof(PurchaseType.PurchaseTypeName);

            if(values.Contains(PURCHASE_TYPE_ID)) {
                model.PurchaseTypeId = Convert.ToInt32(values[PURCHASE_TYPE_ID]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(PURCHASE_TYPE_NAME)) {
                model.PurchaseTypeName = Convert.ToString(values[PURCHASE_TYPE_NAME]);
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