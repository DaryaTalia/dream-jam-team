using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    [SerializeField] Image equipmentIcon;
    [SerializeField] TextMeshProUGUI quantityText;

    public void SetEquipment(ItemStack stack)
    {
        equipmentIcon.sprite = stack.baseItem.iconSprite;
        quantityText.text = stack.quantity.ToString("00");
    }
}
