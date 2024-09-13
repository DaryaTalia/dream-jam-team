using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyResource : MonoBehaviour
{
    ItemStack myItem;

    public ItemStack MyItem
    {
        get => myItem;
        set
        {
            myItem = value;
        }
    }
}
