using saper1.Data;

namespace saper1.IServices
{
    public interface ISettingsService
    {
        SettingsData SettingsData { get; set; }
        void Load();
        void Save(SettingsData settingsData);
    }
}
