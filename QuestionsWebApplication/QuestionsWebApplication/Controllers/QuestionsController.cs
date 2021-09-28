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
            List<Question> tQuestions = new List<Question>();

            try
            {
                tQuestions.AddRange(QuestionsHandlerObject.QuestionsList);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = Languages.Language.ListFail;
                TempData[ResponseKey] = DangerKey;
            }

            return View(tQuestions);
        }

        /// <summary>
        /// POST: \Questions\GetUpdatedData
        /// API call to get a list of updated questions list and return it as a partial
        /// </summary>
        /// <returns>A partial view</returns>
        [HttpPost]
        public ActionResult GetUpdatedData()
        {
            List<Question> tQuestions = new List<Question>();

            try
            {
                tQuestions.AddRange(QuestionsHandlerObject.QuestionsList);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = Languages.Language.ListFail;
                TempData[ResponseKey] = DangerKey;
            }

            return PartialView("_QuestionsView", tQuestions);
        }

        /// <summary>
        /// GET: \Questions\Create
        /// Returns the create new question view
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Create()
        {
            return View();
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
            int tResultCode = (int) ResultCodesEnum.SUCCESS;

            try
            {
                // Validate the inputted data
                if (ModelState.IsValid && pQuestion.ValidateQuestionFields())
                {
                    tResultCode = QuestionsHandlerObject.AddQuestion(pQuestion);

                    if (tResultCode == (int)ResultCodesEnum.SUCCESS)
                    {
                        TempData[MessageKey] = Languages.Language.QuestionAddSuccess;
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
                    TempData[MessageKey] = Languages.Language.QuestionAddValidationFail;
                    TempData[ResponseKey] = DangerKey;
                    tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = Languages.Language.QuestionAddFail;
                TempData[ResponseKey] = DangerKey;
                tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
            }

            // If successful redirect to the index page
            if (tResultCode == (int) ResultCodesEnum.SUCCESS)
            {
                return RedirectToAction(IndexKey, QuestionsKey);
            }
            // If not return to the same page with the question data
            else
            {
                return View(pQuestion);
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
            Question tCorrectInstance = null;

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
                tCorrectInstance = QuestionsFactory.GetInstance(tOriginalQuestion.Type);
                // Manually assign the Id
                tCorrectInstance.Id = tOriginalQuestion.Id;
                // Get all the subtyped question data from the database
                int tResultCode = QuestionsHandlerObject.GetQuestion(tCorrectInstance);

                if (tResultCode != (int)ResultCodesEnum.SUCCESS)
                {
                    TempData[MessageKey] = MessagesUtilities.GetResponseMessage(tResultCode);
                    TempData[ResponseKey] = DangerKey;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = Languages.Language.QuestionAddFail;
                TempData[ResponseKey] = DangerKey;
            }

            return View(tCorrectInstance);
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
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = Languages.Language.QuestionUpdateFail;
                TempData[ResponseKey] = DangerKey;
            }

            return View(pQuestion);
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
            int tResultCode = (int)ResultCodesEnum.SUCCESS;
            string tMessageResponse = "";
            string tRequestResponse = "";

            try
            {
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
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tMessageResponse = Languages.Language.QuestionDeletedFail;
                tRequestResponse = DangerKey;
            }

            return Json(new
            {
                message = tMessageResponse,
                requestResponse = tRequestResponse,
                didDelete = tResultCode == (int)ResultCodesEnum.SUCCESS
            });
        }

        /// <summary>
        /// GET: \Questions\Details\Id
        /// Returns the details view for the question id with the question data
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View</returns>
        public ActionResult Details(int id = -1)
        {
            Question tCorrectInstance = null;
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
                tCorrectInstance = QuestionsFactory.GetInstance(tOriginalQuestion.Type);
                // Manually assign the Id
                tCorrectInstance.Id = tOriginalQuestion.Id;
                // Get all the subtyped question data from the database
                int tResultCode = QuestionsHandlerObject.GetQuestion(tCorrectInstance);

                if (tResultCode != (int) ResultCodesEnum.SUCCESS)
                {
                    TempData[MessageKey] = MessagesUtilities.GetResponseMessage(tResultCode);
                    TempData[ResponseKey] = DangerKey;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = Languages.Language.QuestionFetchFail;
                TempData[ResponseKey] = DangerKey;
            }

            return View(tCorrectInstance);
        }

        /// <summary>
        /// Utility function that returns a question object
        /// </summary>
        /// <param name="pId">The question object to return</param>
        /// <returns>a question object if found or null</returns>
        private Question GetQuestionObject(int pId)
        {
            Question tQuestion = null;

            try
            {
                tQuestion = QuestionsHandlerObject.QuestionsList.Find((tTempQuestion) => pId == tTempQuestion.Id);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tQuestion;
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