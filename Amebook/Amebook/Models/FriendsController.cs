using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amebook.Models
{
    public class FriendsController : Controller
    {
        // GET: Friends
        public ActionResult Index()
        {

            if (Request.IsAjaxRequest())
            {
                var zm = 10;

                return Json(zm, JsonRequestBehavior.AllowGet);
            }

            return View();
        }
    }
}