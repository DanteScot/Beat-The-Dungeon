using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private static string settingsPath = Application.persistentDataPath + "/settings.kevin";

    public static void SaveSettings(Settings settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(settingsPath, FileMode.Create);
        SettingsData data = new SettingsData(settings);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static Settings LoadSettings()
    {
        if (File.Exists(settingsPath)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(settingsPath, FileMode.Open);

            SettingsData data = formatter.Deserialize(stream) as SettingsData;
            stream.Close();
            return new Settings(data);
        } else {
            return new Settings(0,0,0,-1,2,true);
        }
    }
}
