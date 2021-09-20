using LoggerUtils;
using System;
using System.Collections.Generic;

namespace QuestionEntities
{
    public class SliderQuestion : Question
    {
        private byte _StartValue;
        private byte _EndValue;
        private string _StartValueCaption;
        private string _EndValueCaption;

        public byte StartValue
        {
            get { return _StartValue; }
            set
            {
                if (value > 100 || value < 0)
                {
                    throw new Exception("StartValue validation error, please make sure the value is lower than or equal 100 and bigger than 0");
                }

                _StartValue = value;
            }
        }
        public byte EndValue
        {
            get { return _EndValue; }
            set
            {
                if (value > 100 || value < 0)
                {
                    throw new Exception("EndValue validation error, please make sure the value is lower than or equal 100 and bigger than 0");
                }

                _EndValue = value;
            }
        }
        public string StartValueCaption
        {
            get { return _StartValueCaption; }
            set
            {
                if (value.Length > 250 && string.IsNullOrEmpty(value))
                {
                    throw new Exception("StartValueCaption validation error, please make sure the string is not empty or longer than 250");
                }

                _StartValueCaption = value;
            }
        }
        public string EndValueCaption
        {
            get { return _EndValueCaption; }
            set
            {
                if (value.Length > 250 && string.IsNullOrEmpty(value))
                {
                    throw new Exception("EndValueCaption validation error, please make sure the string is not empty or longer than 250");
                }

                _EndValueCaption = value;
            }
        }

        public SliderQuestion
            (int pId, byte pOrder, string pText, byte pStartValue, byte pEndValue, string pStartValueCaption, string pEndValueCaption)
            : base (pId, pOrder, pText, "Slider")
        {
            try
            {
                StartValue = pStartValue;
                EndValue = pEndValue;
                StartValueCaption = pStartValueCaption;
                EndValueCaption = pEndValueCaption;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        public SliderQuestion()
            : this(0, 0, "", 0, 0, "", "")
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
                tDataDictionary.Add("StartValue", StartValue.ToString());
                tDataDictionary.Add("EndValue", EndValue.ToString());
                tDataDictionary.Add("StartValueCaption", StartValueCaption);
                tDataDictionary.Add("EndValueCaption", EndValueCaption);
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
                StartValue = Convert.ToByte(pDataDictionary["StartValue"]);
                EndValue = Convert.ToByte(pDataDictionary["EndValue"]);
                StartValueCaption = pDataDictionary["StartValueCaption"];
                EndValueCaption = pDataDictionary["EndValueCaption"];
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
                tParamNames.Add("StartValue");
                tParamNames.Add("EndValue");
                tParamNames.Add("StartValueCaption");
                tParamNames.Add("EndValueCaption");
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tParamNames;
        }
    }
}
