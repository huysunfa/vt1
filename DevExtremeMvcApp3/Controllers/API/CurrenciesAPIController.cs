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
    [Route("api/CurrenciesAPI/{action}", Name = "CurrenciesAPIApi")]
    public class CurrenciesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var currencies = _context.Currencies.Select(i => new {
                i.CurrencyId,
                i.CurrencyCode,
                i.CurrencyName,
                i.Description
            });
            return Request.CreateResponse(DataSourceLoader.Load(currencies, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new Currency();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.Currencies.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.CurrencyId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.Currencies.FirstOrDefault(item => item.CurrencyId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "Currency not found");

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
            var model = _context.Currencies.FirstOrDefault(item => item.CurrencyId == key);

            _context.Currencies.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(Currency model, IDictionary values) {
            string CURRENCY_ID = nameof(Currency.CurrencyId);
            string CURRENCY_CODE = nameof(Currency.CurrencyCode);
            string CURRENCY_NAME = nameof(Currency.CurrencyName);
            string DESCRIPTION = nameof(Currency.Description);

            if(values.Contains(CURRENCY_ID)) {
                model.CurrencyId = Convert.ToInt32(values[CURRENCY_ID]);
            }

            if(values.Contains(CURRENCY_CODE)) {
                model.CurrencyCode = Convert.ToString(values[CURRENCY_CODE]);
            }

            if(values.Contains(CURRENCY_NAME)) {
                model.CurrencyName = Convert.ToString(values[CURRENCY_NAME]);
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