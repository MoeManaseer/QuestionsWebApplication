using Languages;
using LoggerUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuestionEntities
{
    public class Question
    {
        private static readonly string IdKey = "Id";
        private static readonly string TypeKey = "Type";
        private static readonly string OrderKey = "Order";
        private static readonly string TextKey = "Text";

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "QuestionOrderRequired", ErrorMessageResourceType = typeof(Language))]
        [Range(1, 100, ErrorMessageResourceName = "QuestionOrderLength", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "QuestionOrder", ResourceType = typeof(Language))]
        public byte Order { get; set; }
        
        [Required(ErrorMessageResourceName = "QuestionTextRequired", ErrorMessageResourceType = typeof(Language))]
        [StringLength(maximumLength: 255, MinimumLength = 0, ErrorMessageResourceName = "QuestionTextLength", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "QuestionText", ResourceType = typeof(Language))]
        public string Text { get; set; }

        [Required(ErrorMessageResourceName = "QuestionTypeRequired", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "QuestionType", ResourceType = typeof(Language))]
        public QuestionsTypeEnum Type { get; set; }

        public Question(int pId, byte pOrder, string pText, QuestionsTypeEnum pType)
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
            : this(0, 0, "", QuestionsTypeEnum.Smiley)
        {

        }

        /// <summary>
        /// Validates the question fields
        /// </summary>
        /// <returns>Whether the question fields are valid or no</returns>
        public virtual bool ValidateQuestionFields()
        {
            bool tAreFieldsValid = true;

            try
            {
                if (string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(Type.ToString()))
                {
                    tAreFieldsValid = false;
                }

                if (Text.Length > 255)
                {
                    tAreFieldsValid = false;
                }

                if (Order < 0 || Order > 100)
                {
                    tAreFieldsValid = false;
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tAreFieldsValid;
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
                tDataDictionary.Add(OrderKey, Order.ToString());
                tDataDictionary.Add(TextKey, Text);
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
                Order = Convert.ToByte(pDataDictionary[OrderKey]);
                Text = pDataDictionary[TextKey];

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
                tParamNames.Add(IdKey);
                tParamNames.Add(OrderKey);
                tParamNames.Add(TypeKey);
                tParamNames.Add(TextKey);
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
