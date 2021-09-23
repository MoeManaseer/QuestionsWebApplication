using LoggerUtils;
using QuestionEntities;
using QuestionsController;
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
        private const string CreateKey = "Create";

        public QuestionsController(QuestionsHandler pQuestionsHandler)
        {
            try
            {
                QuestionsHandlerObject = pQuestionsHandler;
                QuestionsHandlerObject.FillQuestionsData();
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            List<Question> tQuestions = new List<Question>();

            try
            {
                tQuestions.AddRange(QuestionsHandlerObject.QuestionsList);
                QuestionsHandlerObject.UpdateData += NotifyUpdateData;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = "Something wrong happend while getting the questions list..";
                TempData[ResponseKey] = DangerKey;
            }

            return View(tQuestions);
        }

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
                TempData[MessageKey] = "Something wrong happend while getting the data list..";
                TempData[ResponseKey] = DangerKey;
            }

            return PartialView("_QuestionsView", tQuestions);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Question pQuestion)
        {
            int tResultCode = (int) ResultCodesEnum.SUCCESS;

            try
            {
                tResultCode = QuestionsHandlerObject.AddQuestion(pQuestion);

                if (tResultCode == (int) ResultCodesEnum.SUCCESS)
                {
                    TempData[MessageKey] = "Question added successfully";
                    TempData[ResponseKey] = SuccessKey;
                }
                else
                {
                    TempData[MessageKey] = "Something wrong happend while adding the question..";
                    TempData[ResponseKey] = DangerKey;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = "Something wrong happend while adding the question..";
                TempData[ResponseKey] = DangerKey;
                tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
            }

            return tResultCode == (int) ResultCodesEnum.SUCCESS ? RedirectToAction(IndexKey, QuestionsKey) : RedirectToAction(CreateKey, QuestionsKey);
        }

        public ActionResult Edit(int id = -1)
        {
            try
            {
                Question tOriginalQuestion = GetQuestionObject(id);

                if (id == -1 || tOriginalQuestion == null)
                {
                    TempData[MessageKey] = "There isn't a question with that specific id..";
                    TempData[ResponseKey] = InfoKey;
                    return RedirectToAction(IndexKey, QuestionsKey);
                }

                Question tCorrectInstance = QuestionsFactory.GetInstance(tOriginalQuestion.Type);
                tCorrectInstance.Id = tOriginalQuestion.Id;
                int tResultCode = QuestionsHandlerObject.GetQuestion(tCorrectInstance);

                if (tResultCode == (int)ResultCodesEnum.SUCCESS)
                {
                    TempData[MessageKey] = "Question edited successfully.";
                    TempData[ResponseKey] = SuccessKey;
                }
                else
                {
                    TempData[MessageKey] = "Something wrong happend while fetching the question.. the question is probably deleted.";
                    TempData[ResponseKey] = DangerKey;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = "Something wrong happend while adding the question..";
                TempData[ResponseKey] = DangerKey;
            }

            return RedirectToAction(IndexKey, QuestionsKey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Question pQuestion)
        {
            try
            {
                int tResultCode = QuestionsHandlerObject.EditQuestion(pQuestion);

                if (tResultCode == (int)ResultCodesEnum.SUCCESS)
                {
                    TempData[MessageKey] = "Question updated successfully";
                    TempData[ResponseKey] = SuccessKey;
                }
                else
                {
                    TempData[MessageKey] = "Something wrong happend while updating the question.. The question probably got deleted";
                    TempData[ResponseKey] = DangerKey;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = "Something wrong happend while editing the question..";
                TempData[ResponseKey] = DangerKey;
            }

            return RedirectToAction(IndexKey, QuestionsKey);
        }

        [HttpPost]
        public ActionResult OnDeleteQuestion(string pQuestionId)
        {
            bool tDidDelete = false;
            string tMessageResponse = "";
            string tRequestResponse = "";

            try
            {
                int tCurrentQuestionId = Convert.ToInt32(pQuestionId);
                Question tQuestion = GetQuestionObject(tCurrentQuestionId);
                
                tDidDelete = (tQuestion != null) && (QuestionsHandlerObject.RemoveQuestion(tQuestion) == (int)ResultCodesEnum.SUCCESS);

                if (tDidDelete)
                {
                    tMessageResponse = "The question was removed successfully!";
                    tRequestResponse = SuccessKey;
                }
                else
                {
                    tMessageResponse = "Something wrong happend while deleting the question.. The question might be already deleted";
                    tRequestResponse = DangerKey;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tDidDelete = false;
            }

            return Json(new
            {
                message = tMessageResponse,
                requestResponse = tRequestResponse,
                didDelete = tDidDelete
            });
        }

        public ActionResult Details(int id = -1)
        {
            Question tCorrectInstance = null;
            try
            {
                Question tOriginalQuestion = GetQuestionObject(id);

                if (id == -1 || tOriginalQuestion == null)
                {
                    TempData[MessageKey] = "There isn't a question with that specific id..";
                    TempData[ResponseKey] = InfoKey;
                    return RedirectToAction(IndexKey, QuestionsKey);
                }

                tCorrectInstance = QuestionsFactory.GetInstance(tOriginalQuestion.Type);
                tCorrectInstance.Id = tOriginalQuestion.Id;
                int tResultCode = QuestionsHandlerObject.GetQuestion(tCorrectInstance);

                if (tResultCode != (int) ResultCodesEnum.SUCCESS)
                {
                    TempData[MessageKey] = "Something wrong happend while fetching the question.. The question might be deleted";
                    TempData[ResponseKey] = DangerKey;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = "Something wrong happend while fetching the question.. please try again";
                TempData[ResponseKey] = DangerKey;
            }

            return View(tCorrectInstance);
        }

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