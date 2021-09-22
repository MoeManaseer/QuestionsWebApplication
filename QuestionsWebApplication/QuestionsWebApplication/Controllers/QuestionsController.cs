﻿using LoggerUtils;
using QuestionEntities;
using QuestionsController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuestionsWebApplication.Controllers
{
    public class QuestionsController : Controller
    {
        private QuestionsHandler QuestionsHandlerObject;
        private static readonly string MessageKey = "Message";
        private static readonly string ResponseKey = "Response";
        private static readonly string DangerKey = "danger";
        private static readonly string InfoKey = "info";
        private static readonly string SuccessKey = "success";
        private static readonly string QuestionsKey = "Questions";
        private static readonly string IndexKey = "Index";
        private static readonly string CreateKey = "Create";

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
                TempData[MessageKey] = "Something wrong happend while getting the questions list..";
                TempData[ResponseKey] = DangerKey;
            }

            return View(tQuestions);
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

        public ActionResult Delete(int id)
        {
            try
            {
                Question tQuestion = GetQuestionObject(id);

                if (tQuestion == null)
                {
                    TempData[MessageKey] = "Something wrong happend while deleting the question.. The question might be already deleted";
                    TempData[ResponseKey] = DangerKey;
                    return RedirectToAction(IndexKey, QuestionsKey);
                }

                int tResultCode = QuestionsHandlerObject.RemoveQuestion(tQuestion);

                if (tResultCode == (int) ResultCodesEnum.SUCCESS)
                {
                    TempData[MessageKey] = "Question deleted successfully";
                    TempData[ResponseKey] = SuccessKey;
                }
                else
                {
                    TempData[MessageKey] = "Something wrong happend while deleting the question.. The question might be already deleted";
                    TempData[ResponseKey] = DangerKey;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData[MessageKey] = "Something wrong happend while deleting the question..";
                TempData[ResponseKey] = DangerKey;
            }

            return RedirectToAction(IndexKey, QuestionsKey);
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
    }
}