using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DevExtremeMvcApp3.Controllers.Control
{
    public class SalesOrderLinesController : Controller
    {
        // GET: SalesOrderLines
        public ActionResult Index(int ID=0)
        {
            ViewBag.ID = ID;
            return View();
        }
    }
}