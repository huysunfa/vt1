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
    [Route("api/UserProfilesAPI/{action}", Name = "UserProfilesAPIApi")]
    public class UserProfilesAPIController : ApiController
    {
        private VTEntities _context = new VTEntities();

        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions) {
            var userprofiles = _context.UserProfiles.Select(i => new {
                i.UserProfileId,
                i.ApplicationUserId,
                i.ConfirmPassword,
                i.Email,
                i.FirstName,
                i.LastName,
                i.OldPassword,
                i.Password,
                i.ProfilePicture
            });
            return Request.CreateResponse(DataSourceLoader.Load(userprofiles, loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form) {
            var model = new UserProfile();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.UserProfiles.Add(model);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, result.UserProfileId);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = _context.UserProfiles.FirstOrDefault(item => item.UserProfileId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "UserProfile not found");

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
            var model = _context.UserProfiles.FirstOrDefault(item => item.UserProfileId == key);

            _context.UserProfiles.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(UserProfile model, IDictionary values) {
            string USER_PROFILE_ID = nameof(UserProfile.UserProfileId);
            string APPLICATION_USER_ID = nameof(UserProfile.ApplicationUserId);
            string CONFIRM_PASSWORD = nameof(UserProfile.ConfirmPassword);
            string EMAIL = nameof(UserProfile.Email);
            string FIRST_NAME = nameof(UserProfile.FirstName);
            string LAST_NAME = nameof(UserProfile.LastName);
            string OLD_PASSWORD = nameof(UserProfile.OldPassword);
            string PASSWORD = nameof(UserProfile.Password);
            string PROFILE_PICTURE = nameof(UserProfile.ProfilePicture);

            if(values.Contains(USER_PROFILE_ID)) {
                model.UserProfileId = Convert.ToInt32(values[USER_PROFILE_ID]);
            }

            if(values.Contains(APPLICATION_USER_ID)) {
                model.ApplicationUserId = Convert.ToString(values[APPLICATION_USER_ID]);
            }

            if(values.Contains(CONFIRM_PASSWORD)) {
                model.ConfirmPassword = Convert.ToString(values[CONFIRM_PASSWORD]);
            }

            if(values.Contains(EMAIL)) {
                model.Email = Convert.ToString(values[EMAIL]);
            }

            if(values.Contains(FIRST_NAME)) {
                model.FirstName = Convert.ToString(values[FIRST_NAME]);
            }

            if(values.Contains(LAST_NAME)) {
                model.LastName = Convert.ToString(values[LAST_NAME]);
            }

            if(values.Contains(OLD_PASSWORD)) {
                model.OldPassword = Convert.ToString(values[OLD_PASSWORD]);
            }

            if(values.Contains(PASSWORD)) {
                model.Password = Convert.ToString(values[PASSWORD]);
            }

            if(values.Contains(PROFILE_PICTURE)) {
                model.ProfilePicture = Convert.ToString(values[PROFILE_PICTURE]);
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