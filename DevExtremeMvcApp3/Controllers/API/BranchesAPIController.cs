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
    [Route("api/BranchesAPI/{action}", Name = "BranchesAPIApi")]
    public class BranchesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var branches = _context.Branches.Select(i => new {
                i.BranchId,
                i.Address,
                i.BranchName,
                i.City,
                i.ContactPerson,
                i.CurrencyId,
                i.Description,
                i.Email,
                i.Phone,
                i.State,
                i.ZipCode
            });
            return Request.CreateResponse(DataSourceLoader.Load(branches, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new Branch();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.Branches.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.BranchId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.Branches.FirstOrDefault(item => item.BranchId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "Branch not found");

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
            var model = _context.Branches.FirstOrDefault(item => item.BranchId == key);

            _context.Branches.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(Branch model, IDictionary values) {
            string BRANCH_ID = nameof(Branch.BranchId);
            string ADDRESS = nameof(Branch.Address);
            string BRANCH_NAME = nameof(Branch.BranchName);
            string CITY = nameof(Branch.City);
            string CONTACT_PERSON = nameof(Branch.ContactPerson);
            string CURRENCY_ID = nameof(Branch.CurrencyId);
            string DESCRIPTION = nameof(Branch.Description);
            string EMAIL = nameof(Branch.Email);
            string PHONE = nameof(Branch.Phone);
            string STATE = nameof(Branch.State);
            string ZIP_CODE = nameof(Branch.ZipCode);

            if(values.Contains(BRANCH_ID)) {
                model.BranchId = Convert.ToInt32(values[BRANCH_ID]);
            }

            if(values.Contains(ADDRESS)) {
                model.Address = Convert.ToString(values[ADDRESS]);
            }

            if(values.Contains(BRANCH_NAME)) {
                model.BranchName = Convert.ToString(values[BRANCH_NAME]);
            }

            if(values.Contains(CITY)) {
                model.City = Convert.ToString(values[CITY]);
            }

            if(values.Contains(CONTACT_PERSON)) {
                model.ContactPerson = Convert.ToString(values[CONTACT_PERSON]);
            }

            if(values.Contains(CURRENCY_ID)) {
                model.CurrencyId = Convert.ToInt32(values[CURRENCY_ID]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
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