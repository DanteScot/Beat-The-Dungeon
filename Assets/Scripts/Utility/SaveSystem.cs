using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private static string settingsPath = Application.persistentDataPath + "/settings.kevin";


    private static void Save<T>(T data, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    private static object Load(string path)
    {
        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(settingsPath, FileMode.Open);

            object data = formatter.Deserialize(stream);
            stream.Close();
            return data;
        } else {
            return null;
        }
    }


    public static void SaveSettings(Settings settings)
    {
        Save(new SettingsData(settings), settingsPath);
    }

    public static Settings LoadSettings()
    {
        SettingsData data = Load(settingsPath) as SettingsData;

        if (data != null) return new Settings(data);
        else return new Settings(null);
    }
}
