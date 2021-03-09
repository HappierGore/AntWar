using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntHill : MonoBehaviour
{
    public static GameObject store, exit;
    public static Transform entry;
    // Start is called before the first frame update
    private void Awake()
    {
        store = GameObject.Find("Store");
        exit = GameObject.Find("AnthillExit");
        entry = GameObject.Find("Anthill").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
