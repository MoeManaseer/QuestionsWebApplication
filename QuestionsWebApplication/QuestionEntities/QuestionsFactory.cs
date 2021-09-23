using System;
using LoggerUtils;

namespace QuestionEntities
{
    public static class QuestionsFactory
    {
        public static Question GetInstance(QuestionsTypeEnum pType = QuestionsTypeEnum.Smiley)
        {
            Question tQuestion = null;

            try
            {
                switch (pType)
                {
                    case QuestionsTypeEnum.Smiley:
                        tQuestion = new SmileyQuestion();
                        break;
                    case QuestionsTypeEnum.Star:
                        tQuestion = new StarQuestion();
                        break;
                    case QuestionsTypeEnum.Slider:
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
