using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class worker : MonoBehaviour
{
    private GeneralHabilities habilities = new GeneralHabilities();
    private AntDetectionZone detectionZone;
    private AntManager manager;
    private Vector2 gizmosPosition;
    public ResourceStats resource;

    //Inventario
    private Inventory antihillInventory, ownInventory;
    private bool alreadyCollecting = false, alreadyDropping = false;

    //Base
    [SerializeField] private Transform basePosition;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    { 
        if (manager.startPath)
        {
            StartCoroutine(habilities.MoveXTo2(manager.pathPosition, manager.GetSpeed()));
            StartCoroutine(habilities.MoveYTo2(manager.pathPosition, manager.GetSpeed()));
            if (habilities.pathXCompleted && habilities.pathYCompleted)
            {
                manager.startPath = false;
            }
        }
        if(manager.objecToGo != null)
        {
            RecollectResource();
            DropInventory();
            if(resource != null && resource.GetAmount() <= 0)
            {
                LookForNewResource();
            }
        }

    }


    private void Initialize()
    {
        detectionZone = GetComponent<AntDetectionZone>();
        manager = GetComponent<AntManager>();
        ownInventory = GetComponentInChildren<Inventory>();
        antihillInventory = GameObject.Find("Queen").GetComponent<Inventory>();
        habilities.Initialize(gameObject);
        basePosition = GameObject.Find("Anthill").transform;
    }
    private void RecollectResource()
    {
        if (detectionZone.nearResource.Length != 0)
        {
            //Debug.Log("Entrando");
            for (int i = 0; i < detectionZone.nearResource.Length; i++)
            {
                if(detectionZone.nearResource[i] == manager.objecToGo)
                {
                    //Debug.Log("Condicion 2");
                    resource = detectionZone.nearResource[i].GetComponent<ResourceStats>();
                    StartCoroutine(CollectFromResource());
                }
            }
        }
    }
    private IEnumerator CollectFromResource()
    {
        if (!alreadyCollecting)
        {
            alreadyCollecting = true;
            while (ownInventory.totalCollected < manager.GetStrenght() && resource.GetAmount() > 0)
            {
                yield return new WaitForSecondsRealtime(2.0f);
                if (manager.objecToGo != null)
                {
                    resource.Take(1);
                    ownInventory.UpdateResource(1, resource.resourceType);
                    ownInventory.type = resource.resourceType;
                }
            }
            alreadyCollecting = false;
            ReturnToStore();
        }
        yield return new WaitForEndOfFrame();
    }
    private void ReturnToStore()
    {
        if(!manager.startPath)
        {
            habilities.pathXCompleted = false;
            habilities.pathYCompleted = false;
            manager.pathPosition = basePosition.position;
            manager.startPath = true;
        }
    }

    private void DropInventory()
    {
        if (detectionZone.nearResource.Length != 0)
        {
            //Debug.Log("Entrando");
            for (int i = 0; i < detectionZone.nearResource.Length; i++)
            {
                if (detectionZone.nearResource[i].transform == basePosition)
                {
                    //Debug.Log("Condicion 2");
                    StartCoroutine(DepositInventory());
                }
            }
        }
    }
    private IEnumerator DepositInventory()
    {
        if(!alreadyDropping && manager.pathPosition == new Vector2( basePosition.position.x, basePosition.position.y))
        {
            //print("Entrando");
            alreadyDropping = true;
            while (ownInventory.totalCollected > 0)
            {
                yield return new WaitForSecondsRealtime(2.0f);
                antihillInventory.UpdateResource(1, ownInventory.type);
                ownInventory.UpdateResource(-1, ownInventory.type);
            }
            ReturnToResource();
            alreadyDropping = false;
            manager.objecToGo = resource.gameObject;
        }
    }
    private void ReturnToResource()
    {
        if (!manager.startPath)
        {
            habilities.pathXCompleted = false;
            habilities.pathYCompleted = false;
            manager.pathPosition = manager.objecToGo.transform.position;
            manager.startPath = true;
        }
    }

    private void LookForNewResource()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("resource");
        Vector2 resourcePos = new Vector2(0.0f, 0.0f);
        float fixedDistance = 0.0f, tempDistance = 0.0f ;
        for (int i = 0; i < temp.Length; i++)
        {
            resourcePos = temp[i].transform.position;
            fixedDistance = (DistanceP1P2(resourcePos.x, transform.position.x) + DistanceP1P2(resourcePos.y, transform.position.y));
            if (tempDistance < fixedDistance)
            {
                tempDistance = fixedDistance;
                manager.objecToGo = temp[i];
                resource = temp[i].GetComponent<ResourceStats>();
                //Debug.Log("passing");
                continue;
            }
        }
    }
    public float DistanceP1P2(float P1, float P2)
    {
        float distance = Mathf.Abs(P1 - P2);
        return distance;
    }
}
