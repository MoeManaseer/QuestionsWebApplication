using System;
using System.IO;
using System.Text;

namespace LoggerUtils
{
    public static class Logger
    {
        private const string TxtFileExtension = "-log.txt";
        private const string GeneralTimeFormat = "MM-dd";
        private const string ExactTimeFormat = "MM/dd HH:mm:ss";

        /// <summary>
        /// Helper function to write exceptions to a txt file
        /// </summary>
        /// <param name="pThrownException">The exception that was thrown</param>
        public static void WriteExceptionMessage(Exception pThrownException)
        {
            try
            {
                DateTime tDateTime = DateTime.Now;
                string tCurrentDate = tDateTime.ToString(GeneralTimeFormat);
                string tFilePath = Environment.CurrentDirectory + "\\" + tCurrentDate + TxtFileExtension;
                CheckFile(tFilePath);

                StringBuilder tErrorString = new StringBuilder();
                tErrorString.AppendLine(tDateTime.ToString(ExactTimeFormat) + " :-");
                tErrorString.AppendLine(pThrownException.Message);
                tErrorString.AppendLine(pThrownException.StackTrace);
                tErrorString.AppendLine(pThrownException.HelpLink);

                WriteExceptionMessageToFile(tErrorString.ToString(), tFilePath);
            }
            catch (Exception tException)
            {
                Console.WriteLine(tException.Message);
            }
        }

        /// <summary>
        /// Writes the exception string to the specific file
        /// </summary>
        /// <param name="pErrorString">The string to be written to the file</param>
        /// <param name="pFilePath">The file string path</param>
        private static void WriteExceptionMessageToFile(string pErrorString, string pFilePath)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(pFilePath))
                {
                    sw.Write(pErrorString);
                }
            }
            catch (Exception tException)
            {
                Console.WriteLine(tException.Message);
            }
        }

        /// <summary>
        /// Helper function to check if the file exists, if it doesn't create it
        /// </summary>
        /// <param name="pFilePath">The file string path</param>
        private static void CheckFile(string pFilePath)
        {
            try
            {
                if (!File.Exists(pFilePath))
                {
                    File.Create(pFilePath).Close();
                }
            }
            catch (Exception tException)
            {
                Console.WriteLine(tException.Message);
            }
        }
    }
}
