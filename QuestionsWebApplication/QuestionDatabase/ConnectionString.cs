using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Languages;
using LoggerUtils;
using QuestionEntities;

namespace QuestionDatabase
{
    public class ConnectionString
    {
        private static readonly string DataSourceKey = "DataSource";
        private static readonly string DatabaseKey = "Database";
        private static readonly string IntegratedSecurityKey = "IntegratedSecurity";
        private static readonly string UsernameKey = "Username";
        private static readonly string PasswordKey = "Password";
        public enum IntegratedSecurityEnum
        {
            [Display(Name = "SSPIKey", ResourceType = typeof(Language))]
            SSPI,
            [Display(Name = "OtherKey", ResourceType = typeof(Language))]
            Other
        }

        [Required(ErrorMessageResourceName = "DataSourceRequired", ErrorMessageResourceType = typeof(Language))]
        [StringLength(maximumLength: 255, MinimumLength = 0, ErrorMessageResourceName = "DataSourceLength", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "DataSource", ResourceType = typeof(Language))]
        public string DataSource { get; private set; }

        [Required(ErrorMessageResourceName = "DatabaseNameRequired", ErrorMessageResourceType = typeof(Language))]
        [StringLength(maximumLength: 255, MinimumLength = 0, ErrorMessageResourceName = "DatabaseNameLength", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "DatabaseName", ResourceType = typeof(Language))]
        public string DatabaseName { get; private set; }

        [Required(ErrorMessageResourceName = "IntegratedSecurityRequired", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "IntegratedSecurity", ResourceType = typeof(Language))]
        public IntegratedSecurityEnum IntegratedSecurity { get; private set; }

        [StringLength(maximumLength: 255, MinimumLength = 0, ErrorMessageResourceName = "UsernameLength", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "Username", ResourceType = typeof(Language))]
        public string Username { get; private set; }

        [StringLength(maximumLength: 255, MinimumLength = 0, ErrorMessageResourceName = "PasswordLength", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "Password", ResourceType = typeof(Language))]
        public string Password { get; private set; }

        public ConnectionString()
        {
            try
            {
                DataSource = ConfigurationManager.AppSettings[DataSourceKey];
                DatabaseName = ConfigurationManager.AppSettings[DatabaseKey];
                IntegratedSecurity = (IntegratedSecurityEnum)Enum.Parse(typeof(IntegratedSecurityEnum), ConfigurationManager.AppSettings[IntegratedSecurityKey]);
                Username = ConfigurationManager.AppSettings[UsernameKey];
                Password = ConfigurationManager.AppSettings[PasswordKey];
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        public ConnectionString(string pDataSource, string pDatabaseName, string pIntegratedSecurity, string pUsername, string pPassword)
        {
            try
            {
                DataSource = pDataSource;
                DatabaseName = pDatabaseName;
                IntegratedSecurity = (IntegratedSecurityEnum) Enum.Parse(typeof(IntegratedSecurityEnum), pIntegratedSecurity);
                Username = pUsername;
                Password = pPassword;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        public ConnectionString(ConnectionString pConnectionString)
            : this(pConnectionString.DataSource, pConnectionString.DatabaseName, pConnectionString.IntegratedSecurity.ToString(), pConnectionString.Username, pConnectionString.Password)
        {

        }

        /// <summary>
        /// Saves the changes to app.config file with the current instance data
        /// </summary>
        public int ApplyChanges()
        {
            int tResultCode = (int) ResultCodesEnum.SUCCESS;

            try
            {
                // Set back the value of the current object to app.config settings

                Configuration tConfigurationManager = null;
                
                if (System.Web.HttpContext.Current != null)
                {
                    tConfigurationManager =
                        System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                }
                else
                {
                    tConfigurationManager =
                        ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                }

                tConfigurationManager.AppSettings.Settings[DataSourceKey].Value = DataSource;
                tConfigurationManager.AppSettings.Settings[DatabaseKey].Value = DatabaseName;
                tConfigurationManager.AppSettings.Settings[IntegratedSecurityKey].Value = IntegratedSecurity.ToString();
                tConfigurationManager.AppSettings.Settings[UsernameKey].Value = Username;
                tConfigurationManager.AppSettings.Settings[PasswordKey].Value = Password;

                tConfigurationManager.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
                tResultCode = (int) ResultCodesEnum.CODE_FAILUER;
            }

            return tResultCode;
        }

        public override string ToString()
        {
            string tConnectionString = "";

            try
            {
                tConnectionString = IntegratedSecurity == IntegratedSecurityEnum.SSPI ?
                string.Format("Data Source={0}; database={1};Integrated Security=SSPI;", DataSource, DatabaseName)
                : string.Format("Data Source={0}; database={1};User ID={2};Password={3};", DataSource, DatabaseName, Username, Password);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }

            return tConnectionString;
        }
    }
}
