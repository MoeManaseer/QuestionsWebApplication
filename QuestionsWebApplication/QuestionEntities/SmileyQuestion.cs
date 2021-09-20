using LoggerUtils;
using System;
using System.Collections.Generic;

namespace QuestionEntities
{
    public class SmileyQuestion : Question
    {
        private byte _NumberOfSmiley;

        public byte NumberOfSmiley
        {
            get { return _NumberOfSmiley; }
            set
            {
                if (value > 5 || value < 2)
                {
                    throw new Exception("NumberOfSmiley validation error, please make sure the value is lower than or equal 5 and bigger or equal to 2");
                }

                _NumberOfSmiley = value;
            }
        }

        public SmileyQuestion
            (int pId, byte pOrder, string pText, byte pNumberOfSmiley)
            : base(pId, pOrder, pText, "Smiley")
        {
            try
            {
                NumberOfSmiley = pNumberOfSmiley;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        public SmileyQuestion()
            : this(0, 0, "", 2)
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
                tDataDictionary.Add("NumberOfSmiley", NumberOfSmiley.ToString());
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
                NumberOfSmiley = Convert.ToByte(pDataDictionary["NumberOfSmiley"]);
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
                tParamNames.Add("NumberOfSmiley");
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tParamNames;
        }
    }
}
