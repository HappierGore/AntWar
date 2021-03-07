using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int food, dirt, stone, honey;
    [HideInInspector] public int totalCollected;
    [HideInInspector] public resourceType type;
    public enum resourceType { food, dirt, stone, honey };

    public int GetFood()
    {
        return food;
    }
    public int GetDirt()
    {
        return dirt;
    }
    public int GetStone()
    {
        return stone;
    }
    public int GetHoney()
    {
        return honey;
    }

    public void UpdateResource(int amount,resourceType resourceType)
    {
        totalCollected += amount;
        switch (resourceType.ToString())
        {
            case "food":
                {
                    food = totalCollected;
                    break;
                }
            case "dirt":
                {
                    dirt = totalCollected;
                    break;
                }
            case "stone":
                {
                    stone = totalCollected;
                    break;
                }
            case "honey":
                {
                    honey = totalCollected;
                    break;
                }
        }
    }
}
