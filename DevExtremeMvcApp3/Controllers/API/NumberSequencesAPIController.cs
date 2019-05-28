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
    [Route("api/NumberSequencesAPI/{action}", Name = "NumberSequencesAPIApi")]
    public class NumberSequencesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var numbersequences = _context.NumberSequences.Select(i => new {
                i.NumberSequenceId,
                i.LastNumber,
                i.Module,
                i.NumberSequenceName,
                i.Prefix
            });
            return Request.CreateResponse(DataSourceLoader.Load(numbersequences, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new NumberSequence();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.NumberSequences.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.NumberSequenceId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.NumberSequences.FirstOrDefault(item => item.NumberSequenceId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "NumberSequence not found");

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
            var model = _context.NumberSequences.FirstOrDefault(item => item.NumberSequenceId == key);

            _context.NumberSequences.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(NumberSequence model, IDictionary values) {
            string NUMBER_SEQUENCE_ID = nameof(NumberSequence.NumberSequenceId);
            string LAST_NUMBER = nameof(NumberSequence.LastNumber);
            string MODULE = nameof(NumberSequence.Module);
            string NUMBER_SEQUENCE_NAME = nameof(NumberSequence.NumberSequenceName);
            string PREFIX = nameof(NumberSequence.Prefix);

            if(values.Contains(NUMBER_SEQUENCE_ID)) {
                model.NumberSequenceId = Convert.ToInt32(values[NUMBER_SEQUENCE_ID]);
            }

            if(values.Contains(LAST_NUMBER)) {
                model.LastNumber = Convert.ToInt32(values[LAST_NUMBER]);
            }

            if(values.Contains(MODULE)) {
                model.Module = Convert.ToString(values[MODULE]);
            }

            if(values.Contains(NUMBER_SEQUENCE_NAME)) {
                model.NumberSequenceName = Convert.ToString(values[NUMBER_SEQUENCE_NAME]);
            }

            if(values.Contains(PREFIX)) {
                model.Prefix = Convert.ToString(values[PREFIX]);
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