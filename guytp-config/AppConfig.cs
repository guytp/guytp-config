using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Guytp.Config
{
    /// <summary>
    /// The app config provides a way to easily access JSON-based configuration files.
    /// </summary>
    public class AppConfig
    {
        #region Declarations
        /// <summary>
        /// Defines the raw JObject that defines the underlying configuration.
        /// </summary>
        private JObject _config;

        /// <summary>
        /// Defines a dictionary keyed by name and storing application settings.
        /// </summary>
        private Dictionary<string, object> _appSettings;

        /// <summary>
        /// Defines a dictionary keyed by name and storing connection strings.
        /// </summary>
        private Dictionary<string, string> _connectionStrings;
        #endregion

        #region Properties
        /// <summary>
        /// Get a handle to the application-wide config file.
        /// </summary>
        public static AppConfig ApplicationInstance { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Handle one time application setup.
        /// </summary>
        static AppConfig()
        {
            try
            {
                string defaultConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "app-config.json");
                ApplicationInstance = new AppConfig(defaultConfigPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed to load application config\r\n" + ex.ToString());
                ApplicationInstance = new AppConfig((JObject)null);
            }
        }

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="filename">
        /// The file to load the config from.
        /// </param>
        public AppConfig(string filename)
        {
            if (!File.Exists(filename))
                throw new Exception("Config does not exist at " + filename);
            string json = File.ReadAllText(filename);
            JObject obj = JObject.Parse(json);
            ParseJObject(obj);
        }

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="rawConfigJson">
        /// The JObject containing the configuration file.
        /// </param>
        public AppConfig(JObject rawConfigJson)
        {
            ParseJObject(rawConfigJson);
        }
        #endregion

        /// <summary>
        /// Parses a JObject into this class.
        /// </summary>
        /// <param name="jObject">
        /// The JObject containing the configuration file.
        /// </param>
        private void ParseJObject(JObject jObject)
        {
            _config = jObject ?? new JObject();
            if (_config.ContainsKey("ConnectionStrings"))
                _connectionStrings = _config["ConnectionStrings"].ToObject<Dictionary<string, string>>();
            else
                _connectionStrings = new Dictionary<string, string>();
            if (_config.ContainsKey("AppSettings"))
                _appSettings = _config["AppSettings"].ToObject<Dictionary<string, object>>();
            else
                _appSettings = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets a value from the AppSettings config.
        /// </summary>
        /// <param name="name">
        /// The name of the setting to retrieve.
        /// </param>
        /// <param name="throwIfNotPresent">
        /// Throws an exception if the specified object is not present in the config, otherwise null is returned.
        /// </param>
        public T GetAppSetting<T>(string name, bool throwIfNotPresent = false)
        {
            if (!_appSettings.ContainsKey(name))
                if (throwIfNotPresent)
                    throw new Exception("Setting not defined: " + name);
                else
                    return default(T);
            object obj = _appSettings[name];
            if (obj is JObject)
                return ((JObject)obj).ToObject<T>();
            return (T)obj;
        }

        /// <summary>
        /// Gets a value from the ConnectionStrings config.
        /// </summary>
        /// <param name="name">
        /// The name of the connection string to retrieve.
        /// </param>
        /// <param name="throwIfNotPresent">
        /// Throws an exception if the specified object is not present in the config, otherwise null is returned.
        /// </param>
        public string GetConnectionString(string name, bool throwIfNotPresent = false)
        {
            if (!_connectionStrings.ContainsKey(name))
                if (throwIfNotPresent)
                    throw new Exception("Connection string not defined: " + name);
                else
                    return null;
            return _connectionStrings[name];
        }

        /// <summary>
        /// Gets a strongly-typed value from the configuration.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to return.
        /// </typeparam>
        /// <param name="name">
        /// The name of the key for the object.
        /// </param>
        /// <param name="throwIfNotPresent">
        /// Throws an exception if the specified object is not present in the config, otherwise null is returned.
        /// </param>
        /// <returns>
        /// The object specified.
        /// </returns>
        public T GetObject<T>(string name, bool throwIfNotPresent = false)
        {
            if (!_config.ContainsKey(name))
                if (throwIfNotPresent)
                    throw new Exception("Config section not defined: " + name);
                else
                    return default(T);
            return _config[name].ToObject<T>();
        }
    }
}