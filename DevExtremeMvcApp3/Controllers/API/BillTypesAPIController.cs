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
    [Route("api/BillTypesAPI/{action}", Name = "BillTypesAPIApi")]
    public class BillTypesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var billtypes = _context.BillTypes.Select(i => new {
                i.BillTypeId,
                i.BillTypeName,
                i.Description
            });
            return Request.CreateResponse(DataSourceLoader.Load(billtypes, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new BillType();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.BillTypes.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.BillTypeId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.BillTypes.FirstOrDefault(item => item.BillTypeId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "BillType not found");

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
            var model = _context.BillTypes.FirstOrDefault(item => item.BillTypeId == key);

            _context.BillTypes.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(BillType model, IDictionary values) {
            string BILL_TYPE_ID = nameof(BillType.BillTypeId);
            string BILL_TYPE_NAME = nameof(BillType.BillTypeName);
            string DESCRIPTION = nameof(BillType.Description);

            if(values.Contains(BILL_TYPE_ID)) {
                model.BillTypeId = Convert.ToInt32(values[BILL_TYPE_ID]);
            }

            if(values.Contains(BILL_TYPE_NAME)) {
                model.BillTypeName = Convert.ToString(values[BILL_TYPE_NAME]);
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