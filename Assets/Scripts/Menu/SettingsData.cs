using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public int resolutionIndex;
    public int qualityIndex;
    public bool isFullscreen;

    public SettingsData(Settings settings)
    {
        masterVolume = settings.masterVolume;
        musicVolume = settings.musicVolume;
        sfxVolume = settings.sfxVolume;
        resolutionIndex = settings.resolutionIndex;
        qualityIndex = settings.qualityIndex;
        isFullscreen = settings.isFullscreen;
    }
}
