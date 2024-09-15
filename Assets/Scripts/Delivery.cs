using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Delivery
{
    [SerializeField]
    string name;
    [SerializeField]
    string deliveryGiverName;
    [SerializeField]
    string questDescription;
    [SerializeField]
    int myDestinationIndex;
    [SerializeField]
    List<BaseItem> deliverItems;

    public string Name
    {
        get => name;
        set
        {
            name = value;
        }
    }

    public string DeliveryGiverName
    {
        get => deliveryGiverName;
        set
        {
            deliveryGiverName = value;
        }
    }

    public string QuestDescription
    {
        get => questDescription;
        set
        {
            questDescription = value;
        }
    }

    public Destination MyDestination
    {
        get => GameManager.Instance.DeliveryDestinations[myDestinationIndex];
        set
        {
            myDestinationIndex = GameManager.Instance.DeliveryDestinations.IndexOf(value);
        }
    }

    public List<BaseItem> DeliverItems
    {
        get => deliverItems;
    }
}
