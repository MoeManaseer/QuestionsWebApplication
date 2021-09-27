using LoggerUtils;
using QuestionsController;
using QuestionsWebApplication.Models;
using System;
using System.Web;
using System.Web.Mvc;
using static QuestionsWebApplication.Models.SettingsModel;

namespace QuestionsWebApplication.Controllers
{
    public class SettingsController : Controller
    {
        private QuestionsHandler QuestionsHandlerObject;

        public SettingsController(QuestionsHandler pQuestionsHandler)
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

        // GET: Settings
        public ActionResult Edit()
        {
            SettingsModel tSettingsModel = new SettingsModel();
            tSettingsModel.ConnectionStringObject = QuestionsHandlerObject.GetConnectionString();

            HttpCookie tLanguageCookie = Request.Cookies["Language"];
            tSettingsModel.Language = (tLanguageCookie != null && string.IsNullOrEmpty(tLanguageCookie.Value)) ? (LanguagesEnum) Enum.Parse(typeof(LanguagesEnum), tLanguageCookie.Value) : LanguagesEnum.English;

            return View(tSettingsModel);
        }

        [HttpPost]
        public ActionResult Edit(SettingsModel pSettingsModel)
        {
            HttpCookie tLanguageCookie = new HttpCookie("Language");
            tLanguageCookie.Value = pSettingsModel.Language.ToString().Substring(0,2);
            Response.Cookies.Add(tLanguageCookie);
            return View();
        }
    }
}