﻿using LoggerUtils;
using QuestionsController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuestionsWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private QuestionsHandler QuestionsHandlerObject;
        
        public HomeController(QuestionsHandler pQuestionsHandler)
        {
            try
            {
                QuestionsHandlerObject = pQuestionsHandler;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}