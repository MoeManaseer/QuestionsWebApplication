using System;
using LoggerUtils;

namespace QuestionEntities
{
    public static class QuestionsFactory
    {
        public static Question GetInstance(QuestionsTypes pQuestionType)
        {
            Question tQuestion = null;

            try
            {
                switch (pQuestionType)
                {
                    case QuestionsTypes.Smiley:
                        tQuestion = new SmileyQuestion();
                        break;
                    case QuestionsTypes.Star:
                        tQuestion = new StarQuestion();
                        break;
                    case QuestionsTypes.Slider:
                        tQuestion = new SliderQuestion();
                        break;
                    default:
                        tQuestion = new Question();
                        break;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tQuestion;
        }

    }
}
