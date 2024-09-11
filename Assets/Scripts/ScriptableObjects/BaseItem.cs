using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/BaseItem")]
public class BaseItem : ScriptableObject
{
    public String itemName = "New Item";
    public ItemType itemType;
    public Texture2D icon;
    public GameObject prefab;
    public int size = 1;
    public int cost;
    public int deliveryReward = 10;
}

public enum ItemType
{
    Resource,
    Deliverable,
    Buff
}