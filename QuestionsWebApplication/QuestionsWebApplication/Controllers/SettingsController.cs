using LoggerUtils;
using QuestionEntities;
using QuestionsController;
using QuestionsWebApplication.Models;
using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static QuestionsWebApplication.Models.SettingsModel;

namespace QuestionsWebApplication.Controllers
{
    public class SettingsController : Controller
    {
        private QuestionsHandler QuestionsHandlerObject;
        private const string LanguageKey = "Language";
        private const string MessageKey = "Message";
        private const string ResponseKey = "Response";
        private const string DangerKey = "danger";
        private const string SuccessKey = "success";

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

        /// <summary>
        /// GET: /Settings/Edit
        /// Returns a view containing all the settings optio
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Edit()
        {
            SettingsModel tSettingsModel = null; 
            try
            {
                tSettingsModel = new SettingsModel();
                // Get the current connection string object
                tSettingsModel.ConnectionStringObject = QuestionsHandlerObject.GetConnectionString();

                // Get the language cookie
                HttpCookie tLanguageCookie = Request.Cookies[LanguageKey];
                // If the language cookie is not set, fallback to the defualt language.
                tSettingsModel.Language = (tLanguageCookie != null && string.IsNullOrEmpty(tLanguageCookie.Value)) ? (LanguagesEnum)Enum.Parse(typeof(LanguagesEnum), tLanguageCookie.Value) : LanguagesEnum.English;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = Languages.Language.ListFail;
                TempData[ResponseKey] = DangerKey;
            }

            return View(tSettingsModel);
        }

        /// <summary>
        /// POST: /Settings/Edit
        /// POST method to save all the settings that were set by the user
        /// </summary>
        /// <param name="pSettingsModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(SettingsModel pSettingsModel)
        {
            try
            {
                int tResultCode = QuestionsHandlerObject.ChangeConnectionString(pSettingsModel.ConnectionStringObject);
                
                // Validate the input and check if the saving of the connection string was successful or not
                if (ModelState.IsValid && tResultCode == (int) ResultCodesEnum.SUCCESS)
                {
                    // Check if the language cookie is set
                    bool tIsLanguageCookieSet = Request.Cookies[LanguageKey] != null;
                    // If the language cookie is set, get it, else create a new cookie
                    HttpCookie tLanguageCookie = tIsLanguageCookieSet ? Request.Cookies[LanguageKey] : new HttpCookie(LanguageKey);
                    // Assign the cookie value
                    tLanguageCookie.Value = pSettingsModel.Language.ToString();

                    // If the cookie is not set, add it to the response cookies 
                    if (!tIsLanguageCookieSet)
                    {
                        Response.Cookies.Add(tLanguageCookie);
                    }

                    // Safety check the cookie value
                    if (!string.IsNullOrEmpty(tLanguageCookie.Value))
                    {
                        // Assign the correct language in the current thread
                        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(tLanguageCookie.Value.Substring(0,2));
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(tLanguageCookie.Value.Substring(0, 2));
                    }

                    TempData[MessageKey] = Languages.Language.SaveSuccess;
                    TempData[ResponseKey] = SuccessKey;
                }
                else
                {
                    TempData[MessageKey] = Languages.Language.SaveFail;
                    TempData[ResponseKey] = DangerKey;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = Languages.Language.SaveFail;
                TempData[ResponseKey] = DangerKey;
            }

            return View(pSettingsModel);
        }
    }
}