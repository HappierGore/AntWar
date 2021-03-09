using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnviroment : MonoBehaviour
{
    [SerializeField] private Text foodAmount, stoneAmount, dirtAmount, antsAmount, honeyAmount;
    [SerializeField] private Inventory antHillInventory;

    // Update is called once per frame
    void Update()
    {
        foodAmount.text = "x" + antHillInventory.GetFood().ToString();
        stoneAmount.text = "x" + antHillInventory.GetStone().ToString();
        dirtAmount.text = "x" + antHillInventory.GetDirt().ToString();
        antsAmount.text = "x" + GameObject.FindGameObjectsWithTag("ant").Length.ToString();
        honeyAmount.text = "x" + antHillInventory.GetHoney().ToString();
    }
}
