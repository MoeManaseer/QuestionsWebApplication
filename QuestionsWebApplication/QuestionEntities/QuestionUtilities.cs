using LoggerUtils;
using System;

namespace QuestionEntities
{
    public enum QuestionsTypeEnum
    {
        Smiley,
        Star,
        Slider,
        Default
    }
    public enum ResultCodesEnum
    {
        SUCCESS = 0,
        QUESTION_OUT_OF_DATE = 1,
        CODE_FAILUER = 2,
        ADD_FAILURE = 3,
        NOTHING_TO_UPDATE = 4,
        EMPTY_FIELDS = 5,
        CONNECTION_STRING_FAILURE = 6,
        UP_TO_DATE = 7,
        DATABASE_ERROR = 10,
        DATABASE_AUTHENTICATION_FAILUER = 11,
        DATABASE_CONNECTION_DENIED = 12,
        DATABASE_CONNECTION_FAILURE = 13,
        SERVER_CONNECTION_FAILURE = 14,
        SERVER_PAUSED = 15,
        SERVER_NOT_FOUND_OR_DOWN = 16,
        DATABASE_FAILURE = 17,
        DATABASE_SQL_INCORRECT = 18,
    }

    public enum DatabaseErrorNumbersEnum
    {
        DATABASE_CONNECTION_FAILURE = 4060,
        DATABASE_AUTHENTICATION_FAILUER = 229,
        DATABASE_CONNECTION_DENIED = 18456,
        DATABASE_SQL_INCORRECT = 102,
        SERVER_PAUSED = 17142,
        SERVER_NOT_FOUND_OR_DOWN = 2,
        SERVER_CONNECTION_FAILURE = 53,
    }

    public static class QuestionUtilities
    {
        public static int GetDatabaseError(int pErrorNumber)
        {
            int tResultCode = (int) ResultCodesEnum.DATABASE_ERROR;

            try
            {
                switch((DatabaseErrorNumbersEnum) pErrorNumber)
                {
                    case DatabaseErrorNumbersEnum.SERVER_CONNECTION_FAILURE:
                        tResultCode = (int) ResultCodesEnum.SERVER_CONNECTION_FAILURE;
                        break;
                    case DatabaseErrorNumbersEnum.DATABASE_CONNECTION_FAILURE:
                        tResultCode = (int) ResultCodesEnum.DATABASE_CONNECTION_FAILURE;
                        break;
                    case DatabaseErrorNumbersEnum.DATABASE_AUTHENTICATION_FAILUER:
                        tResultCode = (int) ResultCodesEnum.DATABASE_AUTHENTICATION_FAILUER;
                        break;
                    case DatabaseErrorNumbersEnum.DATABASE_CONNECTION_DENIED:
                        tResultCode = (int) ResultCodesEnum.DATABASE_CONNECTION_DENIED;
                        break;
                    case DatabaseErrorNumbersEnum.DATABASE_SQL_INCORRECT:
                        tResultCode = (int) ResultCodesEnum.DATABASE_SQL_INCORRECT;
                        break;
                    case DatabaseErrorNumbersEnum.SERVER_NOT_FOUND_OR_DOWN:
                        tResultCode = (int) ResultCodesEnum.SERVER_NOT_FOUND_OR_DOWN;
                        break;
                    case DatabaseErrorNumbersEnum.SERVER_PAUSED:
                        tResultCode = (int) ResultCodesEnum.SERVER_PAUSED;
                        break;
                    default:
                        tResultCode = (int) ResultCodesEnum.DATABASE_ERROR;
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.WriteExceptionMessage(e);
            }

            return tResultCode;
        }
    }
}
