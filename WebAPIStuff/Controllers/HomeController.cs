﻿using System.Web.Mvc;

namespace WebAPIStuff.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}