using System;
using LoggerUtils;

namespace QuestionEntities
{
    public static class QuestionsFactory
    {
        private enum QuestionsTypeEnum
        {
            Smiley,
            Star,
            Slider
        }

        public static Question GetInstance(string type = "")
        {
            Question tQuestion = null;

            try
            {
                switch ((QuestionsTypeEnum)Enum.Parse(typeof(QuestionsTypeEnum), type))
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
