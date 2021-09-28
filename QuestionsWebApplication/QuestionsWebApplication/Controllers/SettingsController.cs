using LoggerUtils;
using QuestionEntities;
using QuestionsController;
using QuestionsWebApplication.Extentions;
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
                tSettingsModel.Language = (tLanguageCookie != null && !string.IsNullOrEmpty(tLanguageCookie.Value)) ? (LanguagesEnum)Enum.Parse(typeof(LanguagesEnum), tLanguageCookie.Value) : LanguagesEnum.English;
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
                // Validate the input and check if the saving of the connection string was successful or not
                if (ModelState.IsValid)
                {
                    int tResultCode = QuestionsHandlerObject.ChangeConnectionString(pSettingsModel.ConnectionStringObject);

                    if (tResultCode == (int)ResultCodesEnum.SUCCESS)
                    {

                        HttpCookie tLanguageCookie = Request.Cookies[LanguageKey];
                        if (tLanguageCookie == null)
                        {
                            // no cookie found, create it
                            tLanguageCookie = new HttpCookie(LanguageKey);
                        }

                        tLanguageCookie.Value = pSettingsModel.Language.ToString();
                        tLanguageCookie.Expires = DateTime.UtcNow.AddDays(3000);
                        Response.Cookies.Add(tLanguageCookie);

                        // Safety check the cookie value
                        if (!string.IsNullOrEmpty(tLanguageCookie.Value))
                        {
                            // Assign the correct language in the current thread
                            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(tLanguageCookie.Value.Substring(0, 2));
                            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(tLanguageCookie.Value.Substring(0, 2));
                        }

                        TempData[MessageKey] = Languages.Language.SaveSuccess;
                        TempData[ResponseKey] = SuccessKey;
                    }
                    else
                    {
                        TempData[MessageKey] = MessagesUtilities.GetResponseMessage(tResultCode);
                        TempData[ResponseKey] = DangerKey;
                    }
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

        /// <summary>
        /// POST: /Settings/TestConnection
        /// POST method to save all the settings that were set by the user
        /// </summary>
        /// <param name="pSettingsModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TestConnection(SettingsModel pSettingsModel)
        {
            try
            {
                int tResultCode = QuestionsHandlerObject.TestConnection(pSettingsModel.ConnectionStringObject);

                if (tResultCode == (int)ResultCodesEnum.SUCCESS)
                {
                    TempData[MessageKey] = Languages.Language.TestSuccess;
                    TempData[ResponseKey] = SuccessKey;
                }
                else
                {
                    TempData[MessageKey] = MessagesUtilities.GetResponseMessage(tResultCode);
                    TempData[ResponseKey] = DangerKey;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = Languages.Language.TestFail;
                TempData[ResponseKey] = DangerKey;
            }

            return View(pSettingsModel);
        }
    }
}