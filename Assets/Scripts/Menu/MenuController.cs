using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private Resolution[] resolutions;

    public void Start()
    {
        InitializeGraphicSettings();
        InitializeCredits();
        SetSceneTime();
    }

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

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
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
        SceneManager.LoadScene("Level 1");
    }
}
