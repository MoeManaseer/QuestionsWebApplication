using System;
using System.Configuration;
using LoggerUtils;

namespace QuestionDatabase
{
    public class ConnectionString
    {
        public string DataSource { get; private set; }
        public string DatabaseName { get; private set; }
        public string IntegratedSecurity { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public ConnectionString()
        {
            try
            {
                DataSource = ConfigurationManager.AppSettings["DataSource"];
                DatabaseName = ConfigurationManager.AppSettings["Database"];
                IntegratedSecurity = ConfigurationManager.AppSettings["IntegratedSecurity"];
                Username = ConfigurationManager.AppSettings["Username"];
                Password = ConfigurationManager.AppSettings["Password"];
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
                IntegratedSecurity = pIntegratedSecurity;
                Username = pUsername;
                Password = pPassword;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        public ConnectionString(ConnectionString pConnectionString)
            : this(pConnectionString.DataSource, pConnectionString.DatabaseName, pConnectionString.IntegratedSecurity, pConnectionString.Username, pConnectionString.Password)
        {

        }

        /// <summary>
        /// Saves the changes to app.config file with the current instance data
        /// </summary>
        public void ApplyChanges()
        {
            try
            {
                // Set back the value of the current object to app.config settings
                var tConfigurationManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                tConfigurationManager.AppSettings.Settings["DataSource"].Value = DataSource;
                tConfigurationManager.AppSettings.Settings["Database"].Value = DatabaseName;
                tConfigurationManager.AppSettings.Settings["IntegratedSecurity"].Value = IntegratedSecurity;
                tConfigurationManager.AppSettings.Settings["Username"].Value = Username;
                tConfigurationManager.AppSettings.Settings["Password"].Value = Password;

                tConfigurationManager.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        public override string ToString()
        {
            string tConnectionString = "";

            try
            {
                tConnectionString = IntegratedSecurity.Equals("SSPI") ?
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
