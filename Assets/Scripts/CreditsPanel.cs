using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsPanel : MonoBehaviour
{

    public GameObject Credits;

    void Start()
    {
        Credits.SetActive(false);
    }

    public void OpenCreditsPanel()
    {
        Credits.SetActive(true);
    }
    public void CloseCreditsPanel()
    {
        Credits.SetActive(false);
    }
}
