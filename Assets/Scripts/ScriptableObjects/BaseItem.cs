using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/BaseItem")]
public class BaseItem : ScriptableObject
{
    public String itemName = "New Item";
    [TextArea(4,6)]
    public String itemDescription = "{Item Description Here}";
    public ItemType itemType;
    public Texture2D iconTexture;
    public Sprite iconSprite;
    public GameObject prefab;
    public int size = 1;
    public int cost;
    public CurrencyType currency;
    public int deliveryReward = 10;
}

[Flags]
public enum ItemType
{
    Resource = 1,
    Deliverable = 2,
    Buff = 4,
}

public enum CurrencyType
{
    Gold, Silver, Platinum
}