using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryDeliveryUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI storyNameText;
    [SerializeField]
    TextMeshProUGUI deliveryDescription;
    [SerializeField]
    GameObject itemsPanel;
    [SerializeField]
    TextMeshProUGUI goldCount;
    [SerializeField]
    Button selectButton;

    public void SetStoryDeliveryUI(string name, string description, string gold, UnityAction action)
    {
        storyNameText.text = name;
        deliveryDescription.text = description;
        goldCount.text = gold;
        selectButton.onClick.AddListener(action);
    }

    public GameObject GetItemsPanel()
    {
        return itemsPanel;
    }
}
