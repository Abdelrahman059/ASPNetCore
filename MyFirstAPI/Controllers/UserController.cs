using Base;
using blogWebAPI.Authentication;
using DataAccess.Models;
using DataAccess.Repositoires;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace blogWebAPI.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        //const string SessionName = "_Name";
        //const string SessionAge = "_Age";


        private readonly IUserRepository<Author> authorRepository;
        private readonly Authinticate authinticate;
        public UserController(Authinticate authinticate, IUserRepository<Author> authorRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.authinticate = authinticate;
            this.authorRepository = authorRepository;

            this._httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var token = authinticate.Authintication(user.UserEmail, user.Password);


                if (token == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return View();
                }
                //string cookieValueFromContext = _httpContextAccessor.HttpContext.Request.Cookies["key"];
                //string cookieValueFromReq = Request.Cookies["Key"];
                Set("key", token, 20);
                return RedirectToAction("index", "Post");
            }
            ModelState.AddModelError("", "you have to fill all required fields");

            return View();
        }
        // GET: PostController/Create
        public ActionResult register()
        {
            return View();
        }

        // POST: PostController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult register(Author author)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (authorRepository.Find(author.UserEmail) == null)
                    {
                        authorRepository.Add(author);
                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        ModelState.AddModelError("", "this email was used before");
                        return RedirectToAction(nameof(register));
                    }
                }
                catch
                {
                    return View();
                }

            }
            ModelState.AddModelError("", "Fill the required unputs");
            return RedirectToAction(nameof(register));
        }
        public IActionResult Logout()
        {
            //Remove(SessionName);
            Remove("key");

            return RedirectToAction("Login");
        }
        public void Set(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);

            Response.Cookies.Append(key, value, option);
        }
        public void Remove(string key)
        {
            Response.Cookies.Delete(key);
        }
    }
}
