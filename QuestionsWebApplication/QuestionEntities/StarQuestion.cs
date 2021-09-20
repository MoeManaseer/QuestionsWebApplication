using LoggerUtils;
using System;
using System.Collections.Generic;

namespace QuestionEntities
{
    public class StarQuestion : Question
    {
        private byte _NumberOfStar;

        public byte NumberOfStar
        {
            get { return _NumberOfStar; }
            set
            {
                if (value > 10 || value < 1)
                {
                    throw new Exception("NumberOfStar validation error, please make sure the value is lower than or equal 10 and bigger than 0");
                }

                _NumberOfStar = value;
            }
        }
        public StarQuestion
            (int pId, byte pOrder, string pText, byte pNumberOfStar)
            : base(pId, pOrder, pText, "Star")
        {
            try
            {
                NumberOfStar = pNumberOfStar;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        public StarQuestion()
            : this(0, 0, "", 1)
        {

        }

        /// <summary>
        /// Util function that returns a dictionary of the current object values
        /// </summary>
        /// <returns>A key value pair of the values needed</returns>
        public override Dictionary<string, string> GetDataList()
        {
            Dictionary<string, string> tDataDictionary = base.GetDataList();

            try
            {
                tDataDictionary.Add("NumberOfStar", NumberOfStar.ToString());
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tDataDictionary;
        }

        /// <summary>
        /// Util function that sets the current object data with data from a dictionary
        /// </summary>
        /// <param name="pDataDictionary">The dictionary that holds the data to be set</param>
        /// <returns>whether or not the current object got updated with new values or not</returns>
        public override bool FillData(Dictionary<string, string> pDataDictionary)
        {
            bool tUpdated = base.FillData(pDataDictionary);

            try
            {
                NumberOfStar = Convert.ToByte(pDataDictionary["NumberOfStar"]);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tUpdated = false;
            }

            return tUpdated;
        }

        /// <summary>
        /// Util function that returns the class proporties names
        /// </summary>
        /// <returns>A list of string consisting of the class proporties names</returns>
        public override List<string> GetObjectParamNames()
        {
            List<string> tParamNames = base.GetObjectParamNames();

            try
            {
                tParamNames.Add("NumberOfStar");
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tParamNames;
        }
    }
}
