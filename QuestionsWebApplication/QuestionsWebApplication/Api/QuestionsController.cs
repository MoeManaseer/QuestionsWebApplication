using LoggerUtils;
using QuestionEntities;
using QuestionsController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace QuestionsWebApplication.Api
{
    [RoutePrefix("api/questions")]
    public class QuestionsController : ApiController
    {
        private readonly QuestionsHandler QuestionsHandlerInstance;
        private const string AllDataKey = "allData";
        private const string GetQuestionKey = "GetQuestion";
        public QuestionsController(QuestionsHandler pQuestionsHandler)
        {
            QuestionsHandlerInstance = pQuestionsHandler;
        }

        /// <summary>
        /// Gets all the questions from the database, accepts a queryParam that is allData, which specifies whether to return
        /// all the data of the questions or not, if it exists that means all the data should come back, if it doesn't only
        /// the main data comes back
        /// </summary>
        /// <returns>The </returns>
        public IHttpActionResult Get()
        {
            try
            {
                // Get the query string params
                Dictionary<string, string> queryStrings = Request.GetQueryNameValuePairs().ToDictionary(tQueryString => tQueryString.Key, tQueryString => tQueryString.Value);
                // Initialize the questions list
                List<Question> tQuestionsList = null;

                if (queryStrings.ContainsKey(AllDataKey))
                {
                    tQuestionsList = new List<Question>();

                    // Loop over the questions and get all the questions data
                    foreach (Question tQuestion in QuestionsHandlerInstance.QuestionsList)
                    {
                        Question tCorrectInstance = QuestionsFactory.GetInstance(tQuestion.Type);
                        tCorrectInstance.Id = tQuestion.Id;
                        QuestionsHandlerInstance.GetQuestion(tCorrectInstance);
                        tQuestionsList.Add(tCorrectInstance);
                    }
                }
                else
                {
                    tQuestionsList = QuestionsHandlerInstance.QuestionsList;
                }
                
                return Ok(tQuestionsList);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                return InternalServerError();
            }
        }

        /// <summary>
        /// Gets a specific question by It's id
        /// </summary>
        /// <param name="pQuestionId"></param>
        /// <returns></returns>
        [Route("{pQuestionId:int}", Name = GetQuestionKey)]
        public IHttpActionResult Get(int pQuestionId)
        {
            try
            {
                // Try getting the origianl question data
                Question tQuestion = QuestionsHandlerInstance.QuestionsList.Find((tempQuestion) => tempQuestion.Id == pQuestionId);
                
                if (tQuestion == null)
                {
                    return BadRequest();
                }

                // Get the query string params
                Dictionary<string, string> queryStrings = Request.GetQueryNameValuePairs().ToDictionary(tQueryString => tQueryString.Key, tQueryString => tQueryString.Value);

                // if allData is not specified, just return the general question data
                if (!queryStrings.ContainsKey(AllDataKey)) return Ok(tQuestion);

                // Get all the question data
                Question tCorrectInstance = QuestionsFactory.GetInstance(tQuestion.Type);
                tCorrectInstance.Id = tQuestion.Id;
                int tResultCode = QuestionsHandlerInstance.GetQuestion(tCorrectInstance);

                if (tResultCode == (int) ResultCodesEnum.SUCCESS) return Ok(tCorrectInstance);

                return InternalServerError();
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                return InternalServerError();
            }
        }

        /// <summary>
        /// Creates a new question
        /// </summary>
        /// <param name="pQuestionData">The new question key/value pairs</param>
        /// <returns></returns>
        public IHttpActionResult Post(Dictionary<string, string> pQuestionData)
        {
            try
            {
                // Get the new question type
                QuestionsTypeEnum tCurrentType = (QuestionsTypeEnum)Enum.Parse(typeof(QuestionsTypeEnum), pQuestionData["Type"]);
                // Create a correct subtype instance of class Question
                Question tQuestion = QuestionsFactory.GetInstance(tCurrentType);
                // Fill the data in the newly created Question subtype instance
                tQuestion.FillData(pQuestionData);

                // Check if the new values are valid
                if (!tQuestion.ValidateQuestionFields()) return BadRequest();

                // Add the new question
                int tResultCode = QuestionsHandlerInstance.AddQuestion(tQuestion);

                if (tResultCode == (int)ResultCodesEnum.SUCCESS) return CreatedAtRoute(GetQuestionKey, new { pQuestionId = tQuestion.Id }, tQuestion);

                return InternalServerError();
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                return InternalServerError();
            }
        }
    }
}
