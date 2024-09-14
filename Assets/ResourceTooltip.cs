using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceTooltip : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemReward;
    public Vector2 tooltipOffset;

    public void SetText(string name, string reward)
    {
        itemName.text = name;
        itemReward.text = reward;
        
    }

    public void Show(Vector2 screenPosition)
    {
        transform.position = screenPosition + tooltipOffset;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
