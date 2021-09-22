using LoggerUtils;
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
            return View(QuestionsHandlerObject.QuestionsList);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Question pQuestion)
        {
            try
            {
                int resultCode = QuestionsHandlerObject.AddQuestion(pQuestion);

                if (resultCode == (int)ResultCodesEnum.SUCCESS)
                {
                    TempData["Message"] = "Question added successfully";
                    TempData["Response"] = "success";
                    return RedirectToAction("Index", "Questions");
                }
                else
                {
                    TempData["Message"] = "Something wrong happend while adding the question..";
                    TempData["Response"] = "danger";
                }

                return RedirectToAction("Create", "Questions", pQuestion);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData["Message"] = "Something wrong happend while adding the question..";
                TempData["Response"] = "danger";
            }

            return RedirectToAction("Create", "Questions", pQuestion);
        }

        public ActionResult Edit(int id = -1)
        {
            Question t = GetQuestionObject(id);

            if (id == -1 || string.IsNullOrEmpty(t.Type.ToString()))
            {
                TempData["Message"] = "There isn't a question with that specific id..";
                TempData["Response"] = "info";
                return RedirectToAction("Create", "Questions");
            }

            Question tCorrectInstance = QuestionsFactory.GetInstance(t.Type);
            tCorrectInstance.Id = t.Id;
            int resultCode = QuestionsHandlerObject.GetQuestion(tCorrectInstance);

            if (resultCode != (int)ResultCodesEnum.SUCCESS)
            {
                TempData["Message"] = "Something wrong happend while fetching the question.. the question is probably deleted.";
                TempData["Response"] = "danger";
                return RedirectToAction("Index", "Questions");
            }

            return View(tCorrectInstance);
        }

        [HttpPost]
        public ActionResult Edit(Question pQuestion)
        {
            try
            {
                int resultCode = QuestionsHandlerObject.EditQuestion(pQuestion);

                if (resultCode == (int)ResultCodesEnum.SUCCESS)
                {
                    TempData["Message"] = "Question updated successfully";
                    TempData["Response"] = "success";
                    return RedirectToAction("Index", "Questions");
                }
                else
                {
                    TempData["Message"] = "Something wrong happend while updating the question.. The question probably got deleted";
                    TempData["Response"] = "danger";
                }

                return RedirectToAction("Edit", "Questions", pQuestion);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData["Message"] = "Something wrong happend while editing the question..";
                TempData["Response"] = "danger";
            }

            return RedirectToAction("Edit", "Questions", pQuestion);
        }

        public ActionResult Delete(int id)
        {
            try
            {
                int resultCode = QuestionsHandlerObject.RemoveQuestion(GetQuestionObject(id));

                if (resultCode == (int)ResultCodesEnum.SUCCESS)
                {
                    TempData["Message"] = "Question deleted successfully";
                    TempData["Response"] = "success";
                }
                else
                {
                    TempData["Message"] = "Something wrong happend while deleting the question.. The question might be already deleted";
                    TempData["Response"] = "danger";
                }

                return RedirectToAction("Index", "Questions");
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                TempData["Message"] = "Something wrong happend while deleting the question..";
                TempData["Response"] = "danger";
            }

            return RedirectToAction("Index", "Questions");
        }

        public ActionResult Details(int id = -1)
        {
            Question t = GetQuestionObject(id);

            if (id == -1 || string.IsNullOrEmpty(t.Type.ToString()))
            {
                TempData["Message"] = "There isn't a question with that specific id..";
                TempData["Response"] = "info";
                return RedirectToAction("Index", "Questions");
            }

            Question tCorrectInstance = QuestionsFactory.GetInstance(t.Type);
            tCorrectInstance.Id = t.Id;
            int resultCode = QuestionsHandlerObject.GetQuestion(tCorrectInstance);

            return View(tCorrectInstance);
        }

        private Question GetQuestionObject(int id)
        {
            Question t = new Question();

            try
            {
                t = QuestionsHandlerObject.QuestionsList.Find((question) => id == question.Id);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return t;
        }
    }
}