using System;
using System.Linq;
using System.Web.Mvc;

namespace QuestionsWebApplication.Extensions
{
    public class QuestionsModelBinder : DefaultModelBinder
    {
        private static readonly string ModelTypeKey = "ModelTypeName";
        private static readonly string InvalidKey = "Invalid";
        private static readonly string QuestionKey = "Question";
        private static readonly string ErrorMessage = "View does not contain";

        protected override object CreateModel(ControllerContext pControllerContext, ModelBindingContext pBindingContext, Type pModelType)
        {
            if (pModelType.Name.Equals(QuestionKey))
            {
                var tModelTypeValue = pControllerContext.Controller.ValueProvider.GetValue(ModelTypeKey);
                if (tModelTypeValue == null)
                    throw new Exception(ErrorMessage + ModelTypeKey);

                var tModelTypeName = tModelTypeValue.AttemptedValue;

                var tType = pModelType.Assembly.GetTypes().SingleOrDefault(x => x.IsSubclassOf(pModelType) && x.Name == tModelTypeName);
                if (tType == null)
                    throw new Exception(InvalidKey + ModelTypeKey);

                var tConcreteInstance = Activator.CreateInstance(tType);

                pBindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => tConcreteInstance, tType);

                return tConcreteInstance;

            }

            return base.CreateModel(pControllerContext, pBindingContext, pModelType);
        }
    }
}