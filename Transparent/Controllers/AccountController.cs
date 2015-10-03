using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Transparent.Filters;
using Transparent.Data.Models;
using System.Data.Linq;
using Transparent.Data;
using Transparent.Business.Interfaces;
using Facebook;
using Transparent.Models;
using System.Data.SqlClient;

namespace Transparent.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserFactory userServiceFactory;

        public AccountController(IUserFactory userServiceFactory)
        {
            this.userServiceFactory = userServiceFactory;
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (model.Action == "ForgottenPassword")
                return ForgottenPassword(model);

            if (!String.IsNullOrEmpty(model.FacebookToken))
                return FacebookLogin(model, returnUrl);

            // If the username is blank, get it from the email address
            if(String.IsNullOrWhiteSpace(model.UserName) && !String.IsNullOrWhiteSpace(model.Email))
                using(var db = new UsersContext())
                {
                    var userProfile = db.UserProfiles.SingleOrDefault(user => user.Email == model.Email);
                    if(userProfile != null)
                        model.UserName = userProfile.UserName;
                };

            // Attempt to login and redirect to the required page
            if (ModelState.IsValid && model.UserName != null && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The details provided are incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterFacebook(RegisterModel model)
        {
            return View("Register", model);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    FacebookUser facebookUser = null;
                    if (!string.IsNullOrEmpty(model.FacebookToken))
                    {
                        facebookUser = GetFacebookUser(model.FacebookToken);
                        model.Password = Guid.NewGuid().ToString();
                    }
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new
                    {
                        Email = model.Email,
                        FacebookId = facebookUser == null ? null : facebookUser.Id
                    });
                    WebSecurity.Login(model.UserName, model.Password);
                    //if (!String.IsNullOrEmpty(model.ReturnUrl))
                    //{
                    //    // TODO: While this is a nice touch, it would be more secure to send an email to
                    //    // complete registration and do the redirection
                    //    return RedirectToLocal(model.ReturnUrl);
                    //}
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
                catch (SqlException e)
                {
                    // TODO: Remove this and use email to complete registration in future.
                    ModelState.AddModelError("", "You are already registered");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/UserProfile

        public ActionResult UserProfile()
        {
            using(var db = new UsersContext())
            {              
                var userProfile = db.FullUserProfiles.Single(profile => profile.UserName == User.Identity.Name);
                return View(userProfile);
            }
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        if (String.IsNullOrWhiteSpace(model.Token) && WebSecurity.IsAuthenticated)
                        {
                            WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        }
                        else
                        {
                            var userId = WebSecurity.GetUserIdFromPasswordResetToken(model.Token);
                            WebSecurity.ResetPassword(model.Token, model.NewPassword);
                            using (var db = new UsersContext())
                            {
                                var username = db.UserProfiles.Single(user => user.UserId == userId).UserName;
                                WebSecurity.Login(username, model.NewPassword);
                            }
                        }
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ForgottenPassword
        [AllowAnonymous]
        public ActionResult ForgottenPassword(string token)
        {
            var userId = WebSecurity.GetUserIdFromPasswordResetToken(token);
            if (userId < 0)
                return View("TokenExpired");
            ViewBag.HasLocalPassword = false;
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View("Manage", new LocalPasswordModel { Token = token });
        }

        private ActionResult ForgottenPassword(LoginModel model)
        {
            try
            {
                userServiceFactory.Create().ForgottenPassword(model.UserName, model.Email);
            }
            catch (ArgumentException)
            {
                // Incorrect username or email, redisplay form
                ModelState.AddModelError("", "The details provided could not be found.");
                return View(model);
            }
            return View("ForgottenPasswordConfirmation");
        }

        private ActionResult FacebookLogin(LoginModel model, string returnUrl)
        {
            var facebookUser = GetFacebookUser(model.FacebookToken);
            using (UsersContext db = new UsersContext())
            {
                var user = db.UserProfiles.SingleOrDefault(u => u.FacebookId == facebookUser.Id);
                if (user == null)
                {
                    returnUrl = returnUrl != null && returnUrl.ToLower().Contains("login")
                        ? null
                        : returnUrl;
                    return RegisterFacebookUser(returnUrl);
                }
                FormsAuthentication.SetAuthCookie(user.UserName, true);
                return RedirectToLocal(returnUrl);
            }
        }

        private FacebookUser GetFacebookUser(string token)
        {
            var client = new FacebookClient(token);
            return new FacebookUser(client.Get("me"));
        }

        private ActionResult RegisterFacebookUser(string returnUrl)
        {
            return RedirectToAction("RegisterFacebook", new RegisterModel { GetFacebookToken = true, ReturnUrl = returnUrl });
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
