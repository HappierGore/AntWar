using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStats : MonoBehaviour
{
    [SerializeField] private int amount = 10;
    public Inventory.resourceType resourceType;

    public void Take(int amountTaked)
    {
        amount -= amountTaked;
    }


    public int GetAmount()
    {
        return amount;
    }


}
