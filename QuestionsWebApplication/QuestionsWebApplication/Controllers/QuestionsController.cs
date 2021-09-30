using LoggerUtils;
using QuestionEntities;
using QuestionsController;
using QuestionsWebApplication.Extentions;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QuestionsWebApplication.Controllers
{
    public class QuestionsController : Controller
    {
        private QuestionsHandler QuestionsHandlerObject;
        private const string MessageKey = "Message";
        private const string ResponseKey = "Response";
        private const string DangerKey = "danger";
        private const string InfoKey = "info";
        private const string SuccessKey = "success";
        private const string QuestionsKey = "Questions";
        private const string IndexKey = "Index";

        public QuestionsController(QuestionsHandler pQuestionsHandler)
        {
            try
            {
                QuestionsHandlerObject = pQuestionsHandler;
                
                // Only add the event listener once when the QuestionsHandler singleton gets constructed
                if (QuestionsHandlerObject.IsInitialLoad)
                {
                    QuestionsHandlerObject.UpdateData += NotifyUpdateData;
                }
                
                QuestionsHandlerObject.FillQuestionsData();
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        /// <summary>
        /// GET: \Questions\Index
        /// Questions index page, shows all the questions from the database
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            try
            {
                List<Question> tQuestions = new List<Question>(QuestionsHandlerObject.QuestionsList);
                return View(tQuestions);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                return View("Error");
            }
        }

        /// <summary>
        /// POST: \Questions\GetUpdatedData
        /// API call to get a list of updated questions list and return it as a partial
        /// </summary>
        /// <returns>A partial view</returns>
        [HttpPost]
        public ActionResult GetUpdatedData()
        {
            try
            {
                List<Question> tQuestions = new List<Question>(QuestionsHandlerObject.QuestionsList);
                return PartialView("_QuestionsView", tQuestions);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                return View("Error");
            }
        }

        /// <summary>
        /// GET: \Questions\Create
        /// Returns the create new question view
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                return View("Error");
            }
        }

        /// <summary>
        /// POST: \Questions\Create
        /// Post method to add a new question
        /// </summary>
        /// <param name="pQuestion"></param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Question pQuestion)
        {

            try
            {
                int tResultCode = (int) ResultCodesEnum.SUCCESS;

                // Validate the inputted data
                if (ModelState.IsValid && pQuestion.ValidateQuestionFields())
                {
                    tResultCode = QuestionsHandlerObject.AddQuestion(pQuestion);

                    if (tResultCode == (int)ResultCodesEnum.SUCCESS)
                    {
                        TempData[MessageKey] = Languages.Language.QuestionAddSuccess;
                        TempData[ResponseKey] = SuccessKey;

                        // If successful return to the index page
                        return RedirectToAction(IndexKey, QuestionsKey);
                    }
                    else
                    {
                        TempData[MessageKey] = MessagesUtilities.GetResponseMessage(tResultCode);
                        TempData[ResponseKey] = DangerKey;
                    }
                }
                else
                {
                    TempData[MessageKey] = Languages.Language.QuestionAddValidationFail;
                    TempData[ResponseKey] = DangerKey;
                }

                return View(pQuestion);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                return View("Error");
            }
        }

        /// <summary>
        /// GET: \Questions\Edit\Id
        /// Returns the edit view for the question id with the question data
        /// </summary>
        /// <param name="id">The id of the selected question</param>
        /// <returns>View</returns>
        public ActionResult Edit(int id = -1)
        {
            try
            {
                // Get the question object
                Question tOriginalQuestion = GetQuestionObject(id);

                // If question doesn't exist redirect the user
                if (id == -1 || tOriginalQuestion == null)
                {
                    TempData[MessageKey] = Languages.Language.QuestionNull;
                    TempData[ResponseKey] = InfoKey;
                    return RedirectToAction(IndexKey, QuestionsKey);
                }

                // Create a new instance of the Question class with the right subtype
                Question tCorrectInstance = QuestionsFactory.GetInstance(tOriginalQuestion.Type);
                // Manually assign the Id
                tCorrectInstance.Id = tOriginalQuestion.Id;
                // Get all the subtyped question data from the database
                int tResultCode = QuestionsHandlerObject.GetQuestion(tCorrectInstance);

                if (tResultCode != (int)ResultCodesEnum.SUCCESS)
                {
                    TempData[MessageKey] = MessagesUtilities.GetResponseMessage(tResultCode);
                    TempData[ResponseKey] = DangerKey;
                }

                return View(tCorrectInstance);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                return View("Error");
            }
        }

        /// <summary>
        /// POST: \Question\Edit\Id
        /// POST method to edit an old question
        /// </summary>
        /// <param name="pQuestion"></param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Question pQuestion)
        {
            try
            {
                // Validate the inputted data
                if (ModelState.IsValid && pQuestion.ValidateQuestionFields())
                {
                    int tResultCode = QuestionsHandlerObject.EditQuestion(pQuestion);

                    if (tResultCode == (int)ResultCodesEnum.SUCCESS)
                    {
                        TempData[MessageKey] = Languages.Language.QuestionUpdateSuccess;
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
                    TempData[MessageKey] = Languages.Language.QuestionEditValidationFail;
                    TempData[ResponseKey] = DangerKey;
                }

                return View(pQuestion);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                return View("Error");
            }
        }

        /// <summary>
        /// POST: \Questions\OnDeleteQuestion\Id
        /// POST method to delete questions
        /// </summary>
        /// <param name="pQuestionId">The question id that should be deleted</param>
        /// <returns>JSON object that determines what the value of the POST method</returns>
        [HttpPost]
        public ActionResult OnDeleteQuestion(string pQuestionId)
        {
            try
            {
                int tResultCode = (int)ResultCodesEnum.SUCCESS;
                string tMessageResponse = "";
                string tRequestResponse = "";

                int tCurrentQuestionId = Convert.ToInt32(pQuestionId);
                Question tQuestion = GetQuestionObject(tCurrentQuestionId);

                if (tQuestion != null)
                {
                    tResultCode = QuestionsHandlerObject.RemoveQuestion(tQuestion);

                    if (tResultCode == (int) ResultCodesEnum.SUCCESS)
                    {
                        tMessageResponse = Languages.Language.QuestionDeleteSuccess;
                        tRequestResponse = SuccessKey;
                    }
                    else
                    {
                        tMessageResponse = MessagesUtilities.GetResponseMessage(tResultCode);
                        tRequestResponse = DangerKey;
                    }
                }
                else
                {
                    tMessageResponse = Languages.Language.QuestionDeleteDeleted;
                    tRequestResponse = DangerKey;
                }

                return Json(new
                {
                    message = tMessageResponse,
                    requestResponse = tRequestResponse,
                    didDelete = tResultCode == (int)ResultCodesEnum.SUCCESS
                });
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);

                return Json(new
                {
                    message = Languages.Language.QuestionDeletedFail,
                    requestResponse = DangerKey,
                    didDelete = false
                });
            }
        }

        /// <summary>
        /// Utility function that returns a question object
        /// </summary>
        /// <param name="pId">The question object to return</param>
        /// <returns>a question object if found or null</returns>
        private Question GetQuestionObject(int pId)
        {
            try
            {
                Question tQuestion = QuestionsHandlerObject.QuestionsList.Find((tTempQuestion) => pId == tTempQuestion.Id);
                return tQuestion;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                return null;
            }
        }

        /// <summary>
        /// Utility function that triggers the Hubs to update their data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyUpdateData(object sender, EventArgs e)
        {
            try
            {
                DataUpdateNotifier.Instance.NotifyDataChanged();
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }
    }
}