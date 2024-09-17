using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryPanel : MonoBehaviour
{
    public GameObject Story;

    void Start()
    {
        Story.SetActive(false);
    }

    public void OpenStoryPanel()
    {
        Story.SetActive(true);
    }
    public void CloseStoryPanel()
    {
        Story.SetActive(false);
    }
}
