using LoggerUtils;
using System;
using System.Collections.Generic;

namespace QuestionEntities
{
    public class Question
    {
        private int _Id;
        private byte _Order;
        private string _type;
        private string _Text;

        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public byte Order
        {
            get { return _Order; }
            set
            {
                if (value > 100 || value < 0)
                {
                    throw new Exception("Order can't be bigger than 100 or lower than 0");
                }

                _Order = value;
            }
        }
        public string Text
        {
            get { return _Text; }
            set
            {
                if (value.Length > 250 && string.IsNullOrEmpty(value))
                {
                    throw new Exception("Text validation error, please make sure the string is not empty or longer than 250");
                }

                _Text = value;
            }
        }
        public string Type
        {
            get { return _type; }
            set
            {
                if (value.Length > 250 && string.IsNullOrEmpty(value))
                {
                    throw new Exception("Type validation error, please make sure the string is not empty or longer than 250");
                }

                _type = value;
            }
        }

        public Question(int pId, byte pOrder, string pText, string pType)
        {
            try
            {
                Id = pId;
                Order = pOrder;
                Text = pText;
                Type = pType;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        public Question()
            : this(0, 0, "", "")
        {

        }

        /// <summary>
        /// Util function that returns a dictionary of the current object values
        /// </summary>
        /// <returns>A key value pair of the values needed</returns>
        public virtual Dictionary<string, string> GetDataList()
        {
            Dictionary<string, string> tDataDictionary = new Dictionary<string, string>();

            try
            {
                tDataDictionary.Add("Order", Order.ToString());
                tDataDictionary.Add("Text", Text);
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
        public virtual bool FillData(Dictionary<string, string> pDataDictionary)
        {
            bool tUpdated = false;

            try
            {
                Order = Convert.ToByte(pDataDictionary["Order"]);
                Text = pDataDictionary["Text"];

                tUpdated = true;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tUpdated;
        }

        /// <summary>
        /// Util function that returns the class proporties names
        /// </summary>
        /// <returns>A list of string consisting of the class proporties names</returns>
        public virtual List<string> GetObjectParamNames()
        {
            List<string> tParamNames = new List<string>();

            try
            {
                tParamNames.Add("Id");
                tParamNames.Add("Order");
                tParamNames.Add("Type");
                tParamNames.Add("Text");
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tParamNames;
        }

        public bool UpdateQuestion(Question pQuestion)
        {
            bool tUpdated = false;

            try
            {
                Order = pQuestion.Order;
                Text = pQuestion.Text;
                tUpdated = true;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tUpdated;
        }
    }
}
