using UnityEngine;


public class PauseManager : MonoBehaviour
{
    public GameObject PausePanel;
    private bool m_isPaused = false;

    void Update()
    {
        PauseToggle();
    }

    public void PauseToggle()
    {
        m_isPaused = !m_isPaused;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_isPaused)
            {
                PausePanel.SetActive(false);
                Time.timeScale = 1;

            }
            else
            {
                PausePanel.SetActive(true);
                Time.timeScale = 0;

            }

        }

    }

    public void QuitGame()
    {
        Application.Quit();
        PausePanel.SetActive(true);
    }


    public void ResumeGame()
    {
        Time.timeScale = 1;
        PausePanel.SetActive(false);
    }


}
