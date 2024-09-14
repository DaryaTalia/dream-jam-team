
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceButton : MonoBehaviour
{
    public BaseItem item;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI quantityText;

    public void SetItem(BaseItem item)
    {
        this.item = item;
        itemIcon.sprite = item.iconSprite;
        button.onClick.AddListener(() => GameManager.Instance.hubManager.BuyResource(item));
        nameText.text = item.itemName;
        costText.text = item.cost.ToString();
    }
}