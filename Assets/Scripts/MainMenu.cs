using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject SettingsPanel;
    public GameObject CreditsPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene(2);
    }

    void Start()
    {
        SettingsPanel.SetActive(false);
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
}
