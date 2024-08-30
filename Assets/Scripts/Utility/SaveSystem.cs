using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private static string settingsPath = Application.persistentDataPath + "/settings.kevin";
    private static string gameDataPath = Application.persistentDataPath + "/data.kevin";


    private static void Save<T>(T data, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        Debug.Log("Saving to " + path);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    private static object Load(string path)
    {
        Debug.Log("Loading");
        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(settingsPath, FileMode.Open);

            Debug.Log("Loading from " + path);

            object data = formatter.Deserialize(stream);
            stream.Close();
            return data;
        } else {
            return null;
        }
    }

    public static void SaveGame(GameData gameData)
    {
        Save(gameData, gameDataPath);
    }

    public static GameData LoadGame()
    {
        GameData data = Load(gameDataPath) as GameData;

        if (data != null) return data;
        else return null;
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
