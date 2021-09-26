using LoggerUtils;
using QuestionEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace QuestionDatabase
{
    public class DatabaseController
    {
        public static ConnectionString ConnectionString { private set; get; }
        private int CurrentDataState { get; set; }
        private int MaxIntValue = 2147483647;
        private const string TestQueryString = "SELECT 1 FROM AllQuestions;";
        private const string GetAllQuestionsQueryString = "SELECT * FROM AllQuestions;";
        private const string GetCurrentStateQueryString = "SELECT MAX(CurrentState) FROM QuestionsState";
        private const string IdKey = "Id";
        private const string TextKey = "Text";
        private const string TypeKey = "Type";
        private const string OrderKey = "Order";
        private const string QuestionsString = "Questions";
        private const string AddString = "Add";
        private const string UpdateString = "Update";
        private const string DeleteString = "Delete";
        private const string GetString = "Get";

        public DatabaseController()
        {
            try
            {
                // Get a new instance of connection string from the app.config
                ConnectionString = new ConnectionString();
                CurrentDataState = GetCurrentDataState();
            }
            catch (Exception e)
            {
                Logger.WriteExceptionMessage(e);
            }
        }

        /// <summary>
        /// Returns the current databaseData state
        /// </summary>
        /// <returns>The current state of the data in the database</returns>
        private int GetCurrentDataState()
        {
            int tCurrentDataState = CurrentDataState;
            SqlCommand tSQLCommand = null;
            SqlConnection tSqlConnection = null;

            try
            {
                tSqlConnection = new SqlConnection(ConnectionString.ToString());
                tSQLCommand = new SqlCommand(GetCurrentStateQueryString, tSqlConnection);
                tSqlConnection.Open();

                tCurrentDataState = Convert.ToInt32(tSQLCommand.ExecuteScalar());
            }
            catch (SqlException tSQLException)
            {
                Logger.WriteExceptionMessage(tSQLException);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
            finally
            {
                if (tSQLCommand != null)
                {
                    tSQLCommand.Dispose();
                }

                if (tSqlConnection.State == ConnectionState.Open)
                {
                    tSqlConnection.Close();
                }
            }

            return tCurrentDataState;
        }

        /// <summary>
        /// Changes the connectionstring instance with a new one
        /// </summary>
        /// <param name="tNewConnectionString">The new connectionstring instance</param>
        /// <returns>a result code to be used to determine if success or failure</returns>
        public int ChangeConnectionString(ConnectionString tNewConnectionString)
        {
            int tResultCode = (int)ResultCodesEnum.CODE_FAILUER;

            try
            {
                ConnectionString = new ConnectionString(tNewConnectionString);
                tResultCode = (int)ResultCodesEnum.SUCCESS;
            }
            catch (Exception e)
            {
                Logger.WriteExceptionMessage(e);
            }

            return tResultCode;
        }

        /// <summary>
        /// Tests if the connection string given to it successfuly connects to the database
        /// </summary>
        /// <param name="tConnectionString">The connection string to test</param>
        /// <returns>a result code to be used to determine if success or failure</returns>
        public int TestDatabaseConnection(ConnectionString tConnectionString)
        {
            int tResultCode = (int)ResultCodesEnum.SUCCESS;
            SqlCommand tSQLCommand = null;
            SqlConnection tSqlConnection = null;

            try
            {
                string tomato = tConnectionString.ToString();
                tSqlConnection = new SqlConnection(tConnectionString.ToString());
                
                // We know that the AllQuestions table will always exist in the database, so test selecting from it
                tSQLCommand = new SqlCommand(TestQueryString, tSqlConnection);

                tSqlConnection.Open();
                tSQLCommand.ExecuteNonQuery();
            }
            catch (SqlException tSQLException)
            {
                Logger.WriteExceptionMessage(tSQLException);
                // Gets the corosponding database error code enum
                tResultCode = QuestionUtilities.GetDatabaseError(tSQLException.Number);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
            }
            finally
            {
                if (tSQLCommand != null)
                {
                    tSQLCommand.Dispose();
                }

                if (tSqlConnection.State == ConnectionState.Open)
                {
                    tSqlConnection.Close();
                }
            }

            return tResultCode;
        }

        /// <summary>
        /// Gets data from the database based on the given table names and constructs the DataSet
        /// </summary>
        /// <param name="pQuestionsDataSet">The dataset to be constructed</param>
        /// <param name="pTableNames">The table names to get from the database</param>
        /// <returns>a result code to be used to determine if success or failure</returns>
        public int GetData(List<Question> pQuestionsList)
        {
            SqlCommand tSQLCommand = null;
            SqlDataReader tSQLReader = null;
            SqlConnection tSqlConnection = null;

            int tResultCode = (int)ResultCodesEnum.SUCCESS;

            try
            {
                tSqlConnection = new SqlConnection(ConnectionString.ToString());

                tSQLCommand = new SqlCommand(GetAllQuestionsQueryString, tSqlConnection);
                tSqlConnection.Open();

                tSQLReader = tSQLCommand.ExecuteReader();

                // Loop over the records and add them to the questions list
                while (tSQLReader.Read())
                {
                    Question tQuestion = new Question();

                    tQuestion.Id = Convert.ToInt32(tSQLReader[IdKey]);
                    tQuestion.Text = Convert.ToString(tSQLReader[TextKey]);
                    tQuestion.Type = (QuestionsTypeEnum)Enum.Parse(typeof(QuestionsTypeEnum), Convert.ToString(tSQLReader[TypeKey]));
                    tQuestion.Order = Convert.ToByte(tSQLReader[OrderKey]);

                    if (tQuestion.ValidateQuestionFields())
                    {
                        pQuestionsList.Add(tQuestion);
                    }
                }

                // Whenever we get fresh data, reset the CurrentState int
                CurrentDataState = GetCurrentDataState();
            }
            catch (SqlException tSQLException)
            {
                Logger.WriteExceptionMessage(tSQLException);
                // Gets the corosponding database error code enum
                tResultCode = QuestionUtilities.GetDatabaseError(tSQLException.Number);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int)ResultCodesEnum.CODE_FAILUER;
            }
            finally
            {
                if (tSQLReader != null)
                {
                    tSQLReader.Close();
                }

                if (tSQLCommand != null)
                {
                    tSQLCommand.Dispose();
                }

                if (tSqlConnection.State == ConnectionState.Open)
                {
                    tSqlConnection.Close();
                }
            }

            return tResultCode;
        }

        /// <summary>
        /// Updates the data in the pQuestionsList with fresh data from the database
        /// </summary>
        /// <param name="pQuestionsList">The list of Questions</param>
        /// <returns>a result code to be used to determine if success or failure</returns>
        public int UpdateData(List<Question> pQuestionsList)
        {
            SqlCommand tSQLCommand = null;
            SqlDataReader tSQLReader = null;
            SqlConnection tSqlConnection = null;
            int tResultCode = (int)ResultCodesEnum.SUCCESS;
            int tCurrentState = CurrentDataState;

            try
            {
                tCurrentState = GetCurrentDataState();
                tSqlConnection = new SqlConnection(ConnectionString.ToString());

                // If the current state is the same as the database state, don't fetch any new data.
                if (tCurrentState == CurrentDataState)
                {
                    return (int) ResultCodesEnum.UP_TO_DATE;
                }

                tSQLCommand = new SqlCommand(GetAllQuestionsQueryString, tSqlConnection);
                tSqlConnection.Open();

                tSQLReader = tSQLCommand.ExecuteReader();
                tSQLReader.Read();

                int tQuestionsListPointer = 0;

                if (tSQLReader.HasRows)
                {
                    while (true)
                    {
                        bool tNextRow = false;
                        int tCurrentId = Convert.ToInt32(tSQLReader[IdKey]);

                        if (pQuestionsList[tQuestionsListPointer].Id < tCurrentId)
                        {
                            pQuestionsList.RemoveAt(tQuestionsListPointer);
                        }
                        else
                        {
                            Question tQuestion = pQuestionsList[tQuestionsListPointer].Id > tCurrentId ? new Question() : pQuestionsList[tQuestionsListPointer];

                            tQuestion.Id = tCurrentId;
                            tQuestion.Text = Convert.ToString(tSQLReader[TextKey]);
                            tQuestion.Type = (QuestionsTypeEnum)Enum.Parse(typeof(QuestionsTypeEnum), Convert.ToString(tSQLReader[TypeKey]));
                            tQuestion.Order = Convert.ToByte(tSQLReader[OrderKey]);

                            if (pQuestionsList[tQuestionsListPointer].Id > tCurrentId && tQuestion.ValidateQuestionFields())
                            {
                                pQuestionsList.Insert(tQuestionsListPointer, tQuestion);
                            }

                            tNextRow = true;
                            tQuestionsListPointer++;
                        }

                        if (tQuestionsListPointer == pQuestionsList.Count)
                            break;

                        if (tNextRow && !tSQLReader.Read())
                            break;
                    }

                    // Makes sure to add any extra questions present in the database but not in the List
                    while (tSQLReader.Read())
                    {
                        Question tQuestion = new Question();

                        tQuestion.Id = Convert.ToInt32(tSQLReader[IdKey]);
                        tQuestion.Text = Convert.ToString(tSQLReader[TextKey]);
                        tQuestion.Type = (QuestionsTypeEnum)Enum.Parse(typeof(QuestionsTypeEnum), Convert.ToString(tSQLReader[TypeKey]));
                        tQuestion.Order = Convert.ToByte(tSQLReader[OrderKey]);

                        if (tQuestion.ValidateQuestionFields())
                        {
                            pQuestionsList.Add(tQuestion);
                            tQuestionsListPointer++;
                        }
                    }
                }
                
                // Makes sure that if there are any extra questions, remove them
                for (int i = tQuestionsListPointer; i < pQuestionsList.Count; i++)
                {
                    pQuestionsList.RemoveAt(pQuestionsList.Count - 1);
                }

                // Assign the currentDataState with the new DataState after updating the current data
                CurrentDataState = tCurrentState;
            }
            catch (SqlException tSQLException)
            {
                Logger.WriteExceptionMessage(tSQLException);
                tResultCode = QuestionUtilities.GetDatabaseError(tSQLException.Number);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int)ResultCodesEnum.CODE_FAILUER;
            }
            finally
            {
                if (tSQLReader != null)
                {
                    tSQLReader.Close();
                }

                if (tSQLCommand != null)
                {
                    tSQLCommand.Dispose();
                }

                if (tSqlConnection.State == ConnectionState.Open)
                {
                    tSqlConnection.Close();
                }
            }

            return tResultCode;
        }

        /// <summary>
        /// Returns a Question retrieved from the database
        /// </summary>
        /// <param name="pQuestion">The Question reference to fill the data in</param>
        /// <returns>a result code to be used to determine if success or failure</returns>
        public int GetQuestion(Question pQuestion)
        {
            SqlCommand tSQLCommand = null;
            SqlDataReader tSQLReader = null;
            SqlConnection tSqlConnection = null;
            int tResultCode = (int) ResultCodesEnum.QUESTION_OUT_OF_DATE;

            try
            {
                int tQuestionId = pQuestion.Id;
                string tQuestionTableName = pQuestion.Type + QuestionsString;

                tSqlConnection = new SqlConnection(ConnectionString.ToString());

                // Get the correct procedure based on the tablename
                tSQLCommand = new SqlCommand(GetString + "_" + tQuestionTableName, tSqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                tSQLCommand.Parameters.Add(new SqlParameter("@" + IdKey, tQuestionId));

                tSqlConnection.Open();
                tSQLReader = tSQLCommand.ExecuteReader(CommandBehavior.KeyInfo);

                while (tSQLReader.Read())
                {
                    // Create a new data dictionary
                    Dictionary<string, string> tDataDictionary = new Dictionary<string, string>();
                    // Get the current question prop names
                    List<string> tDataParamNames = pQuestion.GetObjectParamNames();

                    // Loop over the prop names and inesrt the key value pairs into the Dictionary
                    foreach (string tDataParamName in tDataParamNames)
                    {
                        tDataDictionary.Add(tDataParamName, Convert.ToString(tSQLReader[tDataParamName]));
                    }

                    // Fill the current question instance with data which is present in the Dictionary
                    tResultCode = pQuestion.FillData(tDataDictionary) ? (int)ResultCodesEnum.SUCCESS : (int)ResultCodesEnum.CODE_FAILUER;
                }
            }
            catch (SqlException tSQLException)
            {
                Logger.WriteExceptionMessage(tSQLException);
                // Gets the corosponding database error code enum
                tResultCode = QuestionUtilities.GetDatabaseError(tSQLException.Number);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int)ResultCodesEnum.CODE_FAILUER;
            }
            finally
            {
                if (tSQLReader != null)
                {
                    tSQLReader.Close();
                }

                if (tSQLCommand != null)
                {
                    tSQLCommand.Dispose();
                }

                if (tSqlConnection.State == ConnectionState.Open)
                {
                    tSqlConnection.Close();
                }
            }

            return tResultCode;
        }

        /// <summary>
        /// Adds a new question to the database
        /// </summary>
        /// <param name="pQuestionRow">The new question to be added to the database</param>
        /// <returns>a result code to be used to determine if success or failure</returns>
        public int AddQuestion(Question pQuestion)
        {
            SqlCommand tSQLCommand = null;
            SqlConnection tSqlConnection = null;
            int tResultCode = (int)ResultCodesEnum.SUCCESS;

            try
            {
                string tTableName = pQuestion.Type + QuestionsString;

                tSqlConnection = new SqlConnection(ConnectionString.ToString());

                // Get the correct procedure based on the tablename
                tSQLCommand = new SqlCommand(AddString + "_" + tTableName, tSqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Get the current question key value pair object which represent the current question data
                Dictionary<string, string> tQuestionData = pQuestion.GetDataList();

                // Loop over the keys and add the procedure params accordingly
                foreach(string tQuestionDataKey in tQuestionData.Keys)
                {
                    tSQLCommand.Parameters.Add(new SqlParameter(tQuestionDataKey, tQuestionData[tQuestionDataKey]));
                }

                // Add the Id of the question as an output parameter to be used later on
                SqlParameter tNewQuestionId = new SqlParameter("@" + IdKey, SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                tSQLCommand.Parameters.Add(tNewQuestionId);

                tSqlConnection.Open();
                tResultCode = tSQLCommand.ExecuteNonQuery() != 0 ? (int)ResultCodesEnum.SUCCESS : (int)ResultCodesEnum.ADD_FAILURE;

                // If the procedure succeds, get the newly created Id from the database and insert it into the question instance
                pQuestion.Id = Convert.ToInt32(tNewQuestionId.Value);

                if ((int)ResultCodesEnum.SUCCESS == tResultCode)
                {
                    CurrentDataState = (CurrentDataState + 1) % MaxIntValue; ;
                }
            }
            catch (SqlException tSQLException)
            {
                Logger.WriteExceptionMessage(tSQLException);
                // Gets the corosponding database error code enum
                tResultCode = QuestionUtilities.GetDatabaseError(tSQLException.Number);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int)ResultCodesEnum.CODE_FAILUER;
            }
            finally
            {
                if (tSQLCommand != null)
                {
                    tSQLCommand.Dispose();
                }

                if (tSqlConnection.State == ConnectionState.Open)
                {
                    tSqlConnection.Close();
                }
            }

            return tResultCode;
        }

        /// <summary>
        /// Edits a question in the database
        /// </summary>
        /// <param name="pQuestionRow">The question that should be edited in the database</param>
        /// <returns>a result code to be used to determine if success or failure</returns>
        public int EditQuestion(Question pQuestion)
        {
            SqlCommand tSQLCommand = null;
            SqlConnection tSqlConnection = null;
            int tResultCode = (int)ResultCodesEnum.SUCCESS;

            try
            {
                string tTableName = pQuestion.Type + QuestionsString;

                tSqlConnection = new SqlConnection(ConnectionString.ToString());

                // Get the correct procedure based on the tablename
                tSQLCommand = new SqlCommand(UpdateString + "_" + tTableName, tSqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Get the current question key value pair object which represent the current question data
                Dictionary<string, string> tQuestionData = pQuestion.GetDataList();

                // Loop over the keys and add the procedure params accordingly
                foreach (string tQuestionDataKey in tQuestionData.Keys)
                {
                    tSQLCommand.Parameters.Add(new SqlParameter(tQuestionDataKey, tQuestionData[tQuestionDataKey]));
                }

                // Add the question Id individually
                tSQLCommand.Parameters.Add(new SqlParameter("@" + IdKey, pQuestion.Id));

                tSqlConnection.Open();
                tResultCode = tSQLCommand.ExecuteNonQuery() != 0 ? (int)ResultCodesEnum.SUCCESS : (int)ResultCodesEnum.QUESTION_OUT_OF_DATE;

                if ((int)ResultCodesEnum.SUCCESS == tResultCode)
                {
                    CurrentDataState = (CurrentDataState + 1) % MaxIntValue; ;
                }
            }
            catch (SqlException tSQLException)
            {
                Logger.WriteExceptionMessage(tSQLException);
                // Gets the corosponding database error code enum
                tResultCode = QuestionUtilities.GetDatabaseError(tSQLException.Number);
            }
            catch (Exception e)
            {
                Logger.WriteExceptionMessage(e);
                tResultCode = (int)ResultCodesEnum.CODE_FAILUER;
            }
            finally
            {
                if (tSQLCommand != null)
                {
                    tSQLCommand.Dispose();
                }

                if (tSqlConnection.State == ConnectionState.Open)
                {
                    tSqlConnection.Close();
                }
            }

            return tResultCode;
        }

        /// <summary>
        /// Deletes a question from the database
        /// </summary>
        /// <param name="pQuestionRow">The question row that should be removed</param>
        /// <returns>a result code to be used to determine if success or failure</returns>
        public int DeleteQuestion(Question pQuestion)
        {
            SqlCommand tSQLCommand = null;
            SqlConnection tSqlConnection = null;
            int tResultCode = (int)ResultCodesEnum.SUCCESS;

            try
            {
                // Get the table name to remove the quesand the question id
                string tTableName = pQuestion.Type + QuestionsString;
                int tQuestionId = pQuestion.Id;

                tSqlConnection = new SqlConnection(ConnectionString.ToString());

                // Get the correct procedure based on the tablename
                tSQLCommand = new SqlCommand(DeleteString + "_" + tTableName, tSqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add the @Id param with the current question Id
                tSQLCommand.Parameters.Add(new SqlParameter("@" + IdKey, tQuestionId));

                tSqlConnection.Open();
                tResultCode = tSQLCommand.ExecuteNonQuery() != 0 ? (int)ResultCodesEnum.SUCCESS : (int)ResultCodesEnum.QUESTION_OUT_OF_DATE;

                if ((int)ResultCodesEnum.SUCCESS == tResultCode)
                {
                    CurrentDataState = (CurrentDataState + 1) % MaxIntValue; ;
                }
            }
            catch (SqlException tSQLException)
            {
                Logger.WriteExceptionMessage(tSQLException);
                // Gets the corosponding database error code enum
                tResultCode = QuestionUtilities.GetDatabaseError(tSQLException.Number);
            }
            catch (Exception e)
            {
                Logger.WriteExceptionMessage(e);
                tResultCode = (int)ResultCodesEnum.CODE_FAILUER;
            }
            finally
            {
                if (tSQLCommand != null)
                {
                    tSQLCommand.Dispose();
                }

                if (tSqlConnection.State == ConnectionState.Open)
                {
                    tSqlConnection.Close();
                }
            }

            return tResultCode;
        }
    }
}
