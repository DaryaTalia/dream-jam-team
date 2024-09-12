using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsSettingsController : MonoBehaviour
{
    public void VeryLow()
    {
        QualitySettings.SetQualityLevel(0, true);
    }

    public void Low()
    {
        QualitySettings.SetQualityLevel(1, true);
    }

    public void Medium()
    {
        QualitySettings.SetQualityLevel(2, true);
    }

    public void High()
    {
        QualitySettings.SetQualityLevel(3, true);
    }

    public void VeryHigh()
    {
        QualitySettings.SetQualityLevel(4, true);
    }

    public void Ultra()
    {
        QualitySettings.SetQualityLevel(5, true);
    }
}
