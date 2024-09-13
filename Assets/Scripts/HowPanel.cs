using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowsPanel : MonoBehaviour
{

    public GameObject Hows;

    void Start()
    {
        Hows.SetActive(false);
    }

    public void OpenHowPanel()
    {
        Hows.SetActive(true);
    }
    public void CloseHowPanel()
    {
        Hows.SetActive(false);
    }
}
