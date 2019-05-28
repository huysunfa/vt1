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
    [Route("api/ProductsAPI/{action}", Name = "ProductsAPIApi")]
    public class ProductsAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var products = _context.Products.Select(i => new {
                i.ProductId,
                i.Barcode,
                i.BranchId,
                i.CurrencyId,
                i.DefaultBuyingPrice,
                i.DefaultSellingPrice,
                i.Description,
                i.ProductCode,
                i.ProductImageUrl,
                i.ProductName,
                i.UnitOfMeasureId
            });
            return Request.CreateResponse(DataSourceLoader.Load(products, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new Product();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.Products.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.ProductId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.Products.FirstOrDefault(item => item.ProductId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "Product not found");

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
            var model = _context.Products.FirstOrDefault(item => item.ProductId == key);

            _context.Products.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(Product model, IDictionary values) {
            string PRODUCT_ID = nameof(Product.ProductId);
            string BARCODE = nameof(Product.Barcode);
            string BRANCH_ID = nameof(Product.BranchId);
            string CURRENCY_ID = nameof(Product.CurrencyId);
            string DEFAULT_BUYING_PRICE = nameof(Product.DefaultBuyingPrice);
            string DEFAULT_SELLING_PRICE = nameof(Product.DefaultSellingPrice);
            string DESCRIPTION = nameof(Product.Description);
            string PRODUCT_CODE = nameof(Product.ProductCode);
            string PRODUCT_IMAGE_URL = nameof(Product.ProductImageUrl);
            string PRODUCT_NAME = nameof(Product.ProductName);
            string UNIT_OF_MEASURE_ID = nameof(Product.UnitOfMeasureId);

            if(values.Contains(PRODUCT_ID)) {
                model.ProductId = Convert.ToInt32(values[PRODUCT_ID]);
            }

            if(values.Contains(BARCODE)) {
                model.Barcode = Convert.ToString(values[BARCODE]);
            }

            if(values.Contains(BRANCH_ID)) {
                model.BranchId = Convert.ToInt32(values[BRANCH_ID]);
            }

            if(values.Contains(CURRENCY_ID)) {
                model.CurrencyId = Convert.ToInt32(values[CURRENCY_ID]);
            }

            if(values.Contains(DEFAULT_BUYING_PRICE)) {
                model.DefaultBuyingPrice = Convert.ToDouble(values[DEFAULT_BUYING_PRICE], CultureInfo.InvariantCulture);
            }

            if(values.Contains(DEFAULT_SELLING_PRICE)) {
                model.DefaultSellingPrice = Convert.ToDouble(values[DEFAULT_SELLING_PRICE], CultureInfo.InvariantCulture);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(PRODUCT_CODE)) {
                model.ProductCode = Convert.ToString(values[PRODUCT_CODE]);
            }

            if(values.Contains(PRODUCT_IMAGE_URL)) {
                model.ProductImageUrl = Convert.ToString(values[PRODUCT_IMAGE_URL]);
            }

            if(values.Contains(PRODUCT_NAME)) {
                model.ProductName = Convert.ToString(values[PRODUCT_NAME]);
            }

            if(values.Contains(UNIT_OF_MEASURE_ID)) {
                model.UnitOfMeasureId = Convert.ToInt32(values[UNIT_OF_MEASURE_ID]);
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