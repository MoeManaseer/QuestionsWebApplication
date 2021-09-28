using LoggerUtils;
using QuestionEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuestionsWebApplication.Extentions
{
    public static class MessagesUtilities
    {
        public static string GetResponseMessage(int pResultCode)
        {
            string tResponseMessage = "";

            try
            {
                switch ((ResultCodesEnum) pResultCode)
                {
                    case ResultCodesEnum.QUESTION_OUT_OF_DATE:
                        tResponseMessage = Languages.Language.QuestionOutOfDate;
                        break;
                    case ResultCodesEnum.CODE_FAILUER:
                        tResponseMessage = Languages.Language.CodeFailure;
                        break;
                    case ResultCodesEnum.ADD_FAILURE:
                        tResponseMessage = Languages.Language.AddFailure;
                        break;
                    case ResultCodesEnum.NOTHING_TO_UPDATE:
                        tResponseMessage = Languages.Language.NothingToUpdate;
                        break;
                    case ResultCodesEnum.EMPTY_FIELDS:
                        tResponseMessage = Languages.Language.EmptyFields;
                        break;
                    case ResultCodesEnum.CONNECTION_STRING_FAILURE:
                        tResponseMessage = Languages.Language.ConnectionStringFailure;
                        break;
                    case ResultCodesEnum.UP_TO_DATE:
                        tResponseMessage = Languages.Language.UpToDate;
                        break;
                    case ResultCodesEnum.DATABASE_ERROR:
                        tResponseMessage = Languages.Language.DatabaseError;
                        break;
                    case ResultCodesEnum.DATABASE_AUTHENTICATION_FAILUER:
                        tResponseMessage = Languages.Language.DatabaseAuthenticationError;
                        break;
                    case ResultCodesEnum.DATABASE_CONNECTION_DENIED:
                        tResponseMessage = Languages.Language.DatabaseConnectionDenied;
                        break;
                    case ResultCodesEnum.SERVER_CONNECTION_FAILURE:
                        tResponseMessage = Languages.Language.ServerConnectionFailure;
                        break;
                    case ResultCodesEnum.SERVER_PAUSED:
                        tResponseMessage = Languages.Language.ServerPaused;
                        break;
                    case ResultCodesEnum.SERVER_NOT_FOUND_OR_DOWN:
                        tResponseMessage = Languages.Language.ServerNotFoundOrDown;
                        break;
                    case ResultCodesEnum.DATABASE_FAILURE:
                        tResponseMessage = Languages.Language.DatabaseFailure;
                        break;
                    case ResultCodesEnum.DATABASE_SQL_INCORRECT:
                        tResponseMessage = Languages.Language.DatabaseSQLIncorrect;
                        break;
                    default:
                        tResponseMessage = Languages.Language.DefaultResponse;
                        break;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tResponseMessage ;
        }
    }
}