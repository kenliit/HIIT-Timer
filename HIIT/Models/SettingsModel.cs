using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIIT.Models
{
    public class SettingsModel : ISettingsModel
    {
        private const string ConfigFileName = "Settings.json";
        private const string AppName = "HIIT Timer";

        private string appRootFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{AppName}";
        private string configFileName; 

        /// <summary>
        /// Default Settings
        /// </summary>
        public int TimeOn { get; set; } = 35;
        public int TimeOff { get; set; } = 10;
        public int Rounds { get; set; } = 3;
        public int Break { get; set; } = 60;

        public SettingsModel()
        {
            configFileName = $"{appRootFolder}\\{ConfigFileName}";
        }

        /// <summary>
        /// Save the settings as a Json file
        /// </summary>
        public void SaveConfig()
        {
            try
            {
                ConfirmAppFolder();

                using (StreamWriter fs = File.CreateText(configFileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(fs, this);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ReadConfig()
        {
            SettingsModel sm = new SettingsModel();

            try
            {
                using (StreamReader fs = new StreamReader(configFileName))
                {
                    string json = fs.ReadToEnd();
                    sm = JsonConvert.DeserializeObject<SettingsModel>(json);
                }

                this.Break = sm.Break;
                this.Rounds = sm.Rounds;
                this.TimeOff = sm.TimeOff;
                this.TimeOn = sm.TimeOn;
            }
            catch (FileNotFoundException)
            {
                SaveConfig();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteConfig()
        {
            try
            {
                if (File.Exists(configFileName))
                {
                    File.Delete(configFileName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Make Sure the Application Folder is exist
        /// </summary>
        private void ConfirmAppFolder()
        {
            try
            {
                if (!Directory.Exists(appRootFolder))
                {
                    Directory.CreateDirectory(appRootFolder);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
