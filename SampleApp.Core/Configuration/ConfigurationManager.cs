using SystemConfigurationManager = System.Configuration.ConfigurationManager;

namespace SampleApp.Core.Configuration
{
    public class ConfigurationManager
    {
        #region Public methods

        public static string GetAppSettingsValue(string appSettingsName)
        {
            string appSettingsValue = SystemConfigurationManager.AppSettings[appSettingsName];
            if (string.IsNullOrEmpty(appSettingsValue))
                return string.Empty;

            return appSettingsValue;
        }

        public static bool GetAppSettingsValueAsBool(string appSettingsName)
        {
            bool result;
            bool.TryParse(GetAppSettingsValue(appSettingsName), out result);
            return result;
        }

        public static int GetAppSettingsValueAsInt(string appSettingsName)
        {
            int result;
            int.TryParse(GetAppSettingsValue(appSettingsName), out result);
            return result;
        }

        #endregion

    }
}
