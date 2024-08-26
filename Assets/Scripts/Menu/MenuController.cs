using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct Settings{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public int resolutionIndex;
    public int qualityIndex;
    public bool isFullscreen;

    public Settings(SettingsData data){
        if(data == null){
            masterVolume = 0;
            musicVolume = 0;
            sfxVolume = 0;
            resolutionIndex = -1;
            qualityIndex = 2;
            isFullscreen = true;
            return;
        }

        masterVolume = data.masterVolume;
        musicVolume = data.musicVolume;
        sfxVolume = data.sfxVolume;
        resolutionIndex = data.resolutionIndex;
        qualityIndex = data.qualityIndex;
        isFullscreen = data.isFullscreen;
    }

    public Settings(float masterVolume, float musicVolume, float sfxVolume, int resolutionIndex, int qualityIndex, bool isFullscreen){
        this.masterVolume = masterVolume;
        this.musicVolume = musicVolume;
        this.sfxVolume = sfxVolume;
        this.resolutionIndex = resolutionIndex;
        this.qualityIndex = qualityIndex;
        this.isFullscreen = isFullscreen;
    }
}

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;

    [SerializeField] private GameObject creditTemplate;

    [SerializeField] private GameObject[] dayObjects;
    [SerializeField] private GameObject[] nightObjects;


    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private Resolution[] resolutions;
    private Settings settings;

    public void Start()
    {
        InitializeGraphicSettings();
        InitializeCredits();
        SetSceneTime();
        LoadSettings();
    }

    #region Starting Methods

    private void InitializeGraphicSettings(){
        resolutions = Screen.resolutions;

        // Clear the dropdown options
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        // Add each resolution to the dropdown
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Check if the resolution is the current resolution
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        //remove duplicates
        options = options.Distinct().ToList();

        // Add the options to the dropdown
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void InitializeCredits(){
        // leggi i crediti dal file di testo nella cartella Resources
        TextAsset textAsset = Resources.Load<TextAsset>("Credits");
        string[] lines = textAsset.text.Split('\n');

        foreach (string line in lines)
        {
            GameObject credit = Instantiate(creditTemplate, creditTemplate.transform.parent);
            credit.SetActive(true);
            credit.GetComponent<TextMeshProUGUI>().text = line;
        }
    }

    private void SetSceneTime(){
        // get actual hour from system
        int hour = System.DateTime.Now.Hour;
        if(hour >= 6 && hour < 18){
            // day
            foreach(GameObject obj in dayObjects){
                obj.SetActive(true);
            }
            foreach(GameObject obj in nightObjects){
                obj.SetActive(false);
            }
        } else {
            // night
            foreach(GameObject obj in dayObjects){
                obj.SetActive(false);
            }
            foreach(GameObject obj in nightObjects){
                obj.SetActive(true);
            }
        }
    }

    private void LoadSettings(){
        settings = SaveSystem.LoadSettings();
        SetMasterVolume(settings.masterVolume);
        SetMusicVolume(settings.musicVolume);
        SetSFXVolume(settings.sfxVolume);
        SetQuality(settings.qualityIndex);
        SetFullscreen(settings.isFullscreen);

        if(settings.resolutionIndex == -1)  settings.resolutionIndex = resolutions.Length - 1;
        
        SetResolution(settings.resolutionIndex);

        masterVolumeSlider.value = settings.masterVolume;
        musicVolumeSlider.value = settings.musicVolume;
        sfxVolumeSlider.value = settings.sfxVolume;
        resolutionDropdown.value = settings.resolutionIndex;
        qualityDropdown.value = settings.qualityIndex;
        fullscreenToggle.isOn = settings.isFullscreen;
    }

    #endregion

    #region Settings and Credits

    public void SetMasterVolume(float volume)
    {
        settings.masterVolume = volume;
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        settings.musicVolume = volume;
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        settings.sfxVolume = volume;
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        settings.qualityIndex = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        settings.isFullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        settings.resolutionIndex = resolutionIndex;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        SaveSystem.SaveSettings(settings);
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenCredits()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        // TODO: controllare se esiste un salvataggio e caricare il livello corretto
        SceneManager.LoadScene("Lobby");
    }

    #endregion
}
