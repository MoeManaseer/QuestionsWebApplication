using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using LoggerUtils;
using QuestionDatabase;
using QuestionEntities;

namespace QuestionsController
{
    public class QuestionsHandler
    {
        private DatabaseController DatabaseController;
        private Timer UpdateDataTimer;
        public event EventHandler UpdateData;
        public List<Question> QuestionsList { get; private set; }
        private ListSortDirection CurrentSortDirection;
        private int CurrentSortValueEnum; 
        private enum SortableValueNames
        {
            Type,
            Order,
            Id,
            Text
        };

        public QuestionsHandler()
        {
            try
            {
                QuestionsList = new List<Question>();
                DatabaseController = new DatabaseController();
                CurrentSortDirection = ListSortDirection.Ascending;
                CurrentSortValueEnum = (int) SortableValueNames.Id;
                InitTimer();
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        /// <summary>
        /// Initializes the UpdateTimer
        /// </summary>
        private void InitTimer()
        {
            try
            {
                // Create a timer with ten seconds interval.
                UpdateDataTimer = new Timer(20000);
                UpdateDataTimer.Elapsed += OnTimedEvent;
                UpdateDataTimer.Enabled = true;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        /// <summary>
        /// The timer onElapsed function that fires whenever the timer finishes It's interval
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                int tResultCode = UpdateQuestionsData();

                // If new data cameback, invoke the UI to update It's data
                if (tResultCode == (int)ResultCodesEnum.SUCCESS)
                {
                    UpdateData?.Invoke(this, new EventArgs());
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        /// <summary>
        /// Fills the List with data from the database
        /// </summary>
        /// <returns>A response code represnting what happend</returns>
        public int FillQuestionsData()
        {
            int tResultCode = (int) ResultCodesEnum.SUCCESS;

            try
            {
                // Create a new instance of the List so that data Isn't duplicated 
                tResultCode = DatabaseController.GetData(QuestionsList);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
            }

            return tResultCode;
        }

        /// <summary>
        /// Updates the List with data from the database
        /// </summary>
        /// <returns>A response code represnting what happend</returns>
        public int UpdateQuestionsData()
        {
            int tResultCode = (int) ResultCodesEnum.SUCCESS;

            try
            {
                // Only sort the array when we actually need to, meaning if It's sorted in a different way than the original state
                bool tShouldSortList = !((int)SortableValueNames.Id == CurrentSortValueEnum && ListSortDirection.Ascending == CurrentSortDirection);

                if (tShouldSortList)
                {
                    // Sort the array by the Id so that the update operating works correctly
                    SortQuestions(SortableValueNames.Id.ToString(), ListSortDirection.Ascending, false);
                }

                tResultCode = DatabaseController.UpdateData(QuestionsList);

                if (tShouldSortList)
                {
                    // Resort the list using the already selected sort types after refreshing the data
                    SortQuestions(Enum.GetName(typeof(SortableValueNames), CurrentSortValueEnum) , CurrentSortDirection, false);
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
            }

            return tResultCode;
        }

        /// <summary>
        /// Gets a specific question from the database and assigns the new value retrieved from the database to the
        /// question instance
        /// </summary>
        /// <param name="pQuestion">The question instance to fill data in</param>
        /// <returns>A response code represnting what happend</returns>
        public int GetQuestion(Question pQuestion)
        {
            int tResultCode = (int)ResultCodesEnum.SUCCESS;

            try
            {
                tResultCode = DatabaseController.GetQuestion(pQuestion);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int)ResultCodesEnum.CODE_FAILUER;
            }

            return tResultCode;
        }

        /// <summary>
        /// Adds a new question to the database, and if that succeeds, add it to the List
        /// </summary>
        /// <param name="pQuestion"></param>
        /// <returns>A response code represnting what happend</returns>
        public int AddQuestion(Question pQuestion)
        {
            int tResultCode = (int) ResultCodesEnum.SUCCESS;

            try
            {
                tResultCode = DatabaseController.AddQuestion(pQuestion);

                // On success, add the current object to the List
                if ((int)ResultCodesEnum.SUCCESS == tResultCode)
                {
                    QuestionsList.Add(pQuestion);
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
            }

            return tResultCode;
        }

        /// <summary>
        /// Edits a question in the database, and if that succeeds, edit it in the List then notify the UI
        /// </summary>
        /// <param name="pQuestion"></param>
        /// <returns>A response code represnting what happend</returns>
        public int EditQuestion(Question pQuestion)
        {
            int tResultCode = (int) ResultCodesEnum.SUCCESS;

            try
            {
                tResultCode = DatabaseController.EditQuestion(pQuestion);

                if ((int)ResultCodesEnum.SUCCESS == tResultCode)
                {
                    // Get the instance of the question in the List then update It's data with the new instance passed
                    QuestionsList.FirstOrDefault(tQuestion => tQuestion.Id == pQuestion.Id).UpdateQuestion(pQuestion);
                    // Notify the UI that a value was updated so that the UI also updates
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
            }

            return tResultCode;
        }

        /// <summary>
        /// Removes a question in the database, and if that succeeds, remove it from the List
        /// </summary>
        /// <param name="pQuestion"></param>
        /// <returns>A response code represnting what happend</returns>
        public int RemoveQuestion(Question pQuestion)
        {
            int tResultCode = (int) ResultCodesEnum.SUCCESS;

            try
            {
                tResultCode = DatabaseController.DeleteQuestion(pQuestion);

                if ((int)ResultCodesEnum.SUCCESS == tResultCode)
                {
                    QuestionsList.Remove(pQuestion);
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
            }

            return tResultCode;
        }

        /// <summary>
        /// Tests a ConnectionString if it can access the database/server or not 
        /// </summary>
        /// <param name="pConnectionString"></param>
        /// <returns>A response code represnting what happend</returns>
        public int TestConnection(ConnectionString pConnectionString)
        {
            int tResultCode = (int) ResultCodesEnum.SUCCESS;

            try
            {
                tResultCode = DatabaseController.TestDatabaseConnection(pConnectionString);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
            }

            return tResultCode;
        }

        /// <summary>
        /// Changes the current ConnectionString instance in the Database object
        /// </summary>
        /// <param name="pConnectionString"></param>
        /// <returns>A response code represnting what happend</returns>
        public int ChangeConnectionString(ConnectionString pConnectionString)
        {
            int tResultCode = (int) ResultCodesEnum.SUCCESS;

            try
            {
                // Changes the values that are present in the app.config file
                pConnectionString.ApplyChanges();

                // Change the current instance of ConnectionString present in the Database to the new instance
                tResultCode = DatabaseController.ChangeConnectionString(pConnectionString);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
            }

            return tResultCode;
        }

        /// <summary>
        /// Returns the current ConnectionString instance that the Database object currently has
        /// </summary>
        /// <returns>A connectionString instance that is currently present in the database</returns>
        public ConnectionString GetConnectionString()
        {
            ConnectionString tConnectionString = null;

            try
            {
                // Gets the current instance present in the Database object
                tConnectionString = DatabaseController.ConnectionString;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tConnectionString;
        }

        /// <summary>
        /// Util function that sorts the questions List
        /// </summary>
        /// <param name="pValueName">The name value to sort by</param>
        /// <param name="pDirection">The sort direction, ASC, DECS</param>
        /// <returns>A response code represnting what happend</returns>
        public int SortQuestions(string pValueName, ListSortDirection pDirection, bool pIsUserAction = true)
        {
            int tResultCode = (int) ResultCodesEnum.SUCCESS;

            try
            {
                Enum.TryParse(pValueName, out SortableValueNames tSortableValueEnum);

                // Based on the valueName sent in the params, pick which proportie to sort by
                switch (tSortableValueEnum)
                {
                    case SortableValueNames.Type:
                        QuestionsList = (pDirection == ListSortDirection.Descending) ? 
                            QuestionsList.OrderByDescending(tQuestion => tQuestion.Type).ToList() : QuestionsList.OrderBy(tQuestion => tQuestion.Type).ToList();
                        break;
                    case SortableValueNames.Order:
                        QuestionsList = (pDirection == ListSortDirection.Descending) ?
                            QuestionsList.OrderByDescending(tQuestion => tQuestion.Order).ToList() : QuestionsList.OrderBy(tQuestion => tQuestion.Order).ToList();
                        break;
                    case SortableValueNames.Text:
                        QuestionsList = (pDirection == ListSortDirection.Descending) ?
                            QuestionsList.OrderByDescending(tQuestion => tQuestion.Text).ToList() : QuestionsList.OrderBy(tQuestion => tQuestion.Text).ToList();
                        break;
                    default:
                        QuestionsList = (pDirection == ListSortDirection.Descending) ?
                            QuestionsList.OrderByDescending(tQuestion => tQuestion.Id).ToList() : QuestionsList.OrderBy(tQuestion => tQuestion.Id).ToList();
                        break;
                }

                if (pIsUserAction)
                {
                    CurrentSortDirection = pDirection;
                    CurrentSortValueEnum = (int)tSortableValueEnum;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int)ResultCodesEnum.CODE_FAILUER;
            }

            return tResultCode;
        }
    }
}
