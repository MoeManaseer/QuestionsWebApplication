using Languages;
using LoggerUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuestionEntities
{
    public class SliderQuestion : Question
    {
        private static readonly string StartValueKey = "StartValue";
        private static readonly string EndValueKey = "EndValue";
        private static readonly string StartValueCaptionKey = "StartValueCaption";
        private static readonly string EndValueCaptionKey = "EndValueCaption";

        [Required(ErrorMessageResourceName = "QuestionStartValueRequired", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "QuestionStartValue", ResourceType = typeof(Language))]
        [Range(1, 100, ErrorMessageResourceName = "QuestionStartValueRange", ErrorMessageResourceType = typeof(Language))]
        public byte StartValue { get; set; }

        [Required(ErrorMessageResourceName = "QuestionEndValueRequired", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "QuestionEndValue", ResourceType = typeof(Language))]
        [Range(1, 100, ErrorMessageResourceName = "QuestionEndValueRange", ErrorMessageResourceType = typeof(Language))]
        public byte EndValue { get; set; }

        [Required(ErrorMessageResourceName = "QuestionStartValueCaptionRequired", ErrorMessageResourceType = typeof(Language))]
        [StringLength(maximumLength: 255, MinimumLength = 1, ErrorMessageResourceName = "QuestionStartValueCaptionLength", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "QuestionStartValueCaption", ResourceType = typeof(Language))]
        public string StartValueCaption { get; set; }

        [Required(ErrorMessageResourceName = "QuestionEndValueCaptionRequired", ErrorMessageResourceType = typeof(Language))]
        [StringLength(maximumLength: 255, MinimumLength = 1, ErrorMessageResourceName = "QuestionEndValueCaptionLength", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "QuestionEndValueCaption", ResourceType = typeof(Language))]
        public string EndValueCaption { get; set; }

        public SliderQuestion
            (int pId, byte pOrder, string pText, byte pStartValue, byte pEndValue, string pStartValueCaption, string pEndValueCaption)
            : base (pId, pOrder, pText, QuestionsTypeEnum.Slider)
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
        /// Validates the question fields
        /// </summary>
        /// <returns>Whether the question fields are valid or no</returns>
        public override bool ValidateQuestionFields()
        {
            bool tAreFieldsValid = base.ValidateQuestionFields();

            try
            {
                if (string.IsNullOrEmpty(StartValueCaption) || string.IsNullOrEmpty(EndValueCaption))
                {
                    tAreFieldsValid = false;
                }

                if (StartValueCaption.Length > 255 || EndValueCaption.Length > 255)
                {
                    tAreFieldsValid = false;
                }

                if (StartValue < 0 || StartValue > 100)
                {
                    tAreFieldsValid = false;
                }

                if (EndValue < 0 || EndValue > 100 || (EndValue < StartValue && EndValue != 100))
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
        public override Dictionary<string, string> GetDataList()
        {
            Dictionary<string, string> tDataDictionary = base.GetDataList();

            try
            {
                tDataDictionary.Add(StartValueKey, StartValue.ToString());
                tDataDictionary.Add(EndValueKey, EndValue.ToString());
                tDataDictionary.Add(StartValueCaptionKey, StartValueCaption);
                tDataDictionary.Add(EndValueCaptionKey, EndValueCaption);
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
                StartValue = Convert.ToByte(pDataDictionary[StartValueKey]);
                EndValue = Convert.ToByte(pDataDictionary[EndValueKey]);
                StartValueCaption = pDataDictionary[StartValueCaptionKey];
                EndValueCaption = pDataDictionary[EndValueCaptionKey];
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
                tParamNames.Add(StartValueKey);
                tParamNames.Add(EndValueKey);
                tParamNames.Add(StartValueCaptionKey);
                tParamNames.Add(EndValueCaptionKey);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tParamNames;
        }
    }
}
