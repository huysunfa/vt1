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
    [Route("api/WarehousesAPI/{action}", Name = "WarehousesAPIApi")]
    public class WarehousesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var warehouses = _context.Warehouses.Select(i => new {
                i.WarehouseId,
                i.BranchId,
                i.Description,
                i.WarehouseName
            });
            return Request.CreateResponse(DataSourceLoader.Load(warehouses, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new Warehouse();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.Warehouses.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.WarehouseId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.Warehouses.FirstOrDefault(item => item.WarehouseId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "Warehouse not found");

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
            var model = _context.Warehouses.FirstOrDefault(item => item.WarehouseId == key);

            _context.Warehouses.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(Warehouse model, IDictionary values) {
            string WAREHOUSE_ID = nameof(Warehouse.WarehouseId);
            string BRANCH_ID = nameof(Warehouse.BranchId);
            string DESCRIPTION = nameof(Warehouse.Description);
            string WAREHOUSE_NAME = nameof(Warehouse.WarehouseName);

            if(values.Contains(WAREHOUSE_ID)) {
                model.WarehouseId = Convert.ToInt32(values[WAREHOUSE_ID]);
            }

            if(values.Contains(BRANCH_ID)) {
                model.BranchId = Convert.ToInt32(values[BRANCH_ID]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(WAREHOUSE_NAME)) {
                model.WarehouseName = Convert.ToString(values[WAREHOUSE_NAME]);
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