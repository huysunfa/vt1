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
    [Route("api/UnitOfMeasuresAPI/{action}", Name = "UnitOfMeasuresAPIApi")]
    public class UnitOfMeasuresAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var unitofmeasures = _context.UnitOfMeasures.Select(i => new {
                i.UnitOfMeasureId,
                i.Description,
                i.UnitOfMeasureName
            });
            return Request.CreateResponse(DataSourceLoader.Load(unitofmeasures, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new UnitOfMeasure();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.UnitOfMeasures.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.UnitOfMeasureId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.UnitOfMeasures.FirstOrDefault(item => item.UnitOfMeasureId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "UnitOfMeasure not found");

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
            var model = _context.UnitOfMeasures.FirstOrDefault(item => item.UnitOfMeasureId == key);

            _context.UnitOfMeasures.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(UnitOfMeasure model, IDictionary values) {
            string UNIT_OF_MEASURE_ID = nameof(UnitOfMeasure.UnitOfMeasureId);
            string DESCRIPTION = nameof(UnitOfMeasure.Description);
            string UNIT_OF_MEASURE_NAME = nameof(UnitOfMeasure.UnitOfMeasureName);

            if(values.Contains(UNIT_OF_MEASURE_ID)) {
                model.UnitOfMeasureId = Convert.ToInt32(values[UNIT_OF_MEASURE_ID]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(UNIT_OF_MEASURE_NAME)) {
                model.UnitOfMeasureName = Convert.ToString(values[UNIT_OF_MEASURE_NAME]);
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