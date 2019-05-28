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
    [Route("api/ProductTypesAPI/{action}", Name = "ProductTypesAPIApi")]
    public class ProductTypesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var producttypes = _context.ProductTypes.Select(i => new {
                i.ProductTypeId,
                i.Description,
                i.ProductTypeName
            });
            return Request.CreateResponse(DataSourceLoader.Load(producttypes, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new ProductType();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.ProductTypes.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.ProductTypeId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.ProductTypes.FirstOrDefault(item => item.ProductTypeId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "ProductType not found");

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
            var model = _context.ProductTypes.FirstOrDefault(item => item.ProductTypeId == key);

            _context.ProductTypes.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(ProductType model, IDictionary values) {
            string PRODUCT_TYPE_ID = nameof(ProductType.ProductTypeId);
            string DESCRIPTION = nameof(ProductType.Description);
            string PRODUCT_TYPE_NAME = nameof(ProductType.ProductTypeName);

            if(values.Contains(PRODUCT_TYPE_ID)) {
                model.ProductTypeId = Convert.ToInt32(values[PRODUCT_TYPE_ID]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(PRODUCT_TYPE_NAME)) {
                model.ProductTypeName = Convert.ToString(values[PRODUCT_TYPE_NAME]);
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