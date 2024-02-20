using Microsoft.AspNetCore.Mvc;
using NHlytics.Utility;
using NHLytics.Models;
using System.Diagnostics;

namespace NHlytics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FacebookUtility _fb;
        private readonly DbUtility _db;

        public HomeController(ILogger<HomeController> logger, FacebookUtility fb, DbUtility db)
        {
            _logger = logger;
            this._fb = fb;
            this._db = db;
        }

        public IActionResult Index()
        {
            string token = string.Empty;
            string email = Request.Cookies["fb_email"];

            if (!string.IsNullOrEmpty(email))
            {
                token = _db.GetTokenByUserId(email);
            }
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Url = _fb.GetloginUrl();
            }
            else
            {
                try
                {
                    var returModel = _fb.GetdataWithToken(token);
                    return View("loginDetail", returModel);
                }
                catch (Exception)
                {
                    _db.DeleteToken(email);
                    ViewBag.Url = _fb.GetloginUrl();
                }
            }

            return View();
        }

        public IActionResult loginDetail(string code)
        {
            string token = string.Empty;
            string email = Request.Cookies["fb_email"];

            if (!string.IsNullOrEmpty(email))
            {
                token = _db.GetTokenByUserId(email);
            }
            FbModel userDetail = null;
            if (string.IsNullOrEmpty(token))
            {
                userDetail = _fb.FacebookRedirect(code);
                Response.Cookies.Append("fb_email", userDetail.Email);
                _db.SaveToken(userDetail);
                ViewBag.Message = "Login Page.";
            }
            else
            {
                try
                {
                    userDetail = _fb.GetdataWithToken(token);
                }
                catch (Exception)
                {
                    _db.DeleteToken(email);
                    ViewBag.Url = _fb.GetloginUrl();
                    return View("index");
                }
            }
            return View(userDetail);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
