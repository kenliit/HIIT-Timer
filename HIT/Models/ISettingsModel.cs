namespace HIT.Models
{
    public interface ISettingsModel
    {
        int Break { get; set; }
        int Rounds { get; set; }
        int TimeOff { get; set; }
        int TimeOn { get; set; }

        void ReadConfig();
        void SaveConfig();
        void DeleteConfig();
    }
}