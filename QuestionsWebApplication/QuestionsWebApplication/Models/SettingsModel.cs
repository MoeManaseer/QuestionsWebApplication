using Languages;
using QuestionDatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuestionsWebApplication.Models
{
    public class SettingsModel
    {
        public enum LanguagesEnum
        {
            English,
            Arabic,
        };

        public ConnectionString ConnectionStringObject { get; set; }
        [Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "LanguageKey", ResourceType = typeof(Language))]
        public LanguagesEnum Language { get; set; }
    }
}