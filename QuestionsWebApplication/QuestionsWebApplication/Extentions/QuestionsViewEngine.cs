using LoggerUtils;
using System;
using System.Linq;
using System.Web.Mvc;

namespace QuestionsWebApplication.Extensions
{
    public class QuestionsViewEngine : RazorViewEngine
    {
        private static readonly string[] QuestionsPartialViewFormat = new[] {
          "~/Views/Questions/Partials/{0}.cshtml"
       };

        public QuestionsViewEngine()
        {
            try
            {
                PartialViewLocationFormats = PartialViewLocationFormats.Union(QuestionsPartialViewFormat).ToArray();
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }
    }
}