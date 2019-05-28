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
    [Route("api/CashBanksAPI/{action}", Name = "CashBanksAPIApi")]
    public class CashBanksAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var cashbanks = _context.CashBanks.Select(i => new {
                i.CashBankId,
                i.CashBankName,
                i.Description
            });
            return Request.CreateResponse(DataSourceLoader.Load(cashbanks, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new CashBank();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.CashBanks.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.CashBankId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.CashBanks.FirstOrDefault(item => item.CashBankId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "CashBank not found");

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
            var model = _context.CashBanks.FirstOrDefault(item => item.CashBankId == key);

            _context.CashBanks.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(CashBank model, IDictionary values) {
            string CASH_BANK_ID = nameof(CashBank.CashBankId);
            string CASH_BANK_NAME = nameof(CashBank.CashBankName);
            string DESCRIPTION = nameof(CashBank.Description);

            if(values.Contains(CASH_BANK_ID)) {
                model.CashBankId = Convert.ToInt32(values[CASH_BANK_ID]);
            }

            if(values.Contains(CASH_BANK_NAME)) {
                model.CashBankName = Convert.ToString(values[CASH_BANK_NAME]);
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