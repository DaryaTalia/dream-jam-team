using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;

    public AudioSource MainMenuMusic;
    public UnityEngine.UI.Slider SoundSlider;

    public void PlayGame()
    {
        SceneManager.LoadScene(2);
    }

    void Start()
    {
        SettingsPanel.SetActive(true);
        if (MainMenuMusic != null && SoundSlider != null)
        {
            SetSoundVolume(.2f);
        }
        SettingsPanel.SetActive(false);
        PlayerPrefs.Save();
    }

    public void OpenSettingsPanel()
    {
        SettingsPanel.SetActive(true);
    }
    public void CloseSettingsPanel()
    {
        SettingsPanel.SetActive(false);
    }

    public void OpenCreditsPanel()
    {
        CreditsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetSoundVolume(float value)
    {
        if (MainMenuMusic != null)
        {
            PlayerPrefs.SetFloat("Sound Volume", value);
            if (MainMenuMusic != null && SoundSlider != null)
            {
                MainMenuMusic.volume = PlayerPrefs.GetFloat("Sound Volume");
                SoundSlider.value = MainMenuMusic.volume;
                PlayerPrefs.SetInt("Music Toggle", 1);
            }
        }
        PlayerPrefs.Save();
    }

    public void SetMusicToggle()
    {
        if(PlayerPrefs.GetInt("Music Toggle") == 0)
        {
            if (MainMenuMusic != null && SoundSlider != null)
            {
                MainMenuMusic.volume = PlayerPrefs.GetFloat("Sound Volume");
                SoundSlider.value = MainMenuMusic.volume;
                PlayerPrefs.SetInt("Music Toggle", 1);
            }
        }
        else if (PlayerPrefs.GetInt("Music Toggle") == 1)
        {
            if (MainMenuMusic != null && SoundSlider != null)
            {
                MainMenuMusic.volume = 0;
                SoundSlider.value = MainMenuMusic.volume;
                PlayerPrefs.SetInt("Music Toggle", 0);
            }
        }
        PlayerPrefs.Save();
    }

}
