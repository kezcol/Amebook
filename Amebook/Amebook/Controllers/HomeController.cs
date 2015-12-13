using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;

namespace Amebook.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string key)
        {
            string forcopy = key;

            return View((object)forcopy);
        }
    }
}