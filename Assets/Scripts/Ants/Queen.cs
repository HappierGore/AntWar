using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Queen : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    // Start is called before the first frame update
    void Awake()
    {
        db.CheckIfDBExists();
        LoadAllAntsInDB();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject ant = GameObject.Instantiate(prefab, transform) as GameObject;
            ant.name = "ant " + (db.GetIDCounter() + 1);
            ant.GetComponent<AntManager>().startInitialize = true;
        }

    }

    private void OnApplicationPause(bool pause)
    {
        SaveAllAnts();
    }
    public void LoadAllAntsInDB()
    {
        for (int i = 1; i < db.GetIDCounter()+1; i++)
        {
            GameObject ant = GameObject.Instantiate(prefab, transform) as GameObject;
            ant.name = db.GetNameFromID(i);
            AntManager manager = ant.GetComponent<AntManager>();
            manager.SetTypeAnt(db.GetTypeFromID(i));
            ant.transform.position = db.GetPositionFromID(i);
            manager.SetTypeAnt(db.GetTypeFromID(i));
            manager.SetGroup(db.GetGroupFromID(i));
            manager.SetHealth(db.GetHealthFromID(i));
            manager.SetSpeed(db.GetSpeedFromID(i));
            manager.SetDamage(db.GetDamageFromID(i));
            manager.SetStrenght(db.GetStrenghtFromID(i));
            manager.startInitialize = true;
        }
    }
    public void SaveAllAnts()
    {
        int amount = 0;
        GameObject[] allAnts = GameObject.FindGameObjectsWithTag("ant");
        for (int i = 0; i < allAnts.Length; i++)
        {
            AntManager manager = allAnts[i].GetComponent<AntManager>();
            db.UpdateAntData(manager.gameObject.name, manager.GetAntType(), manager.transform.position, manager.GetGroup(), manager.GetHealth(), manager.GetSpeed(), manager.GetDamage(), manager.GetStrenght());
            amount += 1;
        }
        Debug.Log("Saved " + amount + " ants");
    }
}
