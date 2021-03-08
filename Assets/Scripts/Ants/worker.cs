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
    public Vector2 pathPos2;
    private AntHill antHill;
    public ResourceStats resource;

    //Inventario
    private Inventory antihillInventory, ownInventory;
    private bool alreadyCollecting = false, alreadyDropping = false, backToWorkHelper = false;
    public bool inAntHill = false;

    //Base
    [SerializeField] private Transform basePosition;
    //Regresar al trabajo si no hay cambios en el path
    GameObject objectToGotemp = null;


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
                pathPos2 = (!inAntHill) ? new Vector2(0, 0) : pathPos2;
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

        AntHillInside();
        StartCoroutine(BackToWork());
    }


    private void Initialize()
    {
        detectionZone = GetComponent<AntDetectionZone>();
        manager = GetComponent<AntManager>();
        ownInventory = GetComponentInChildren<Inventory>();
        antihillInventory = GameObject.Find("Queen").GetComponent<Inventory>();
        habilities.Initialize(gameObject);
        basePosition = GameObject.Find("Anthill").transform;
        antHill = basePosition.GetComponent<AntHill>();
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
            ReturnToAnthill();
        }
        yield return new WaitForEndOfFrame();
    }

    private void ReturnToAnthill()
    {
        if(!manager.startPath && !inAntHill)
        {
            habilities.pathXCompleted = false;
            habilities.pathYCompleted = false;
            manager.pathPosition = basePosition.position;
            manager.startPath = true;
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

    private void DropInventory()
    {
        if (detectionZone.nearResource.Length != 0)
        {
            for (int i = 0; i < detectionZone.nearResource.Length; i++)
            {
                if (detectionZone.nearResource[i].transform == basePosition && manager.pathPosition == new Vector2(basePosition.position.x,basePosition.position.y)
                    && !manager.startPath)
                {
                    AnthillJoining();
                    //StartCoroutine(DepositInventory());
                }
            }
        }
    }
    private IEnumerator DepositInventory()
    {
        if(!alreadyDropping && manager.pathPosition == new Vector2( antHill.store.transform.position.x, antHill.store.transform.position.y))
        {
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

    private void LookForNewResource()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("resource");
        Vector2 resourcePos = new Vector2(0.0f, 0.0f);
        float fixedDistance = 0.0f, tempDistance = 0.0f;
        for (int i = 0; i < temp.Length; i++)
        {
            resourcePos = temp[i].transform.position;
            fixedDistance = (DistanceP1P2(resourcePos.x, transform.position.x) + DistanceP1P2(resourcePos.y, transform.position.y));
            if (tempDistance < fixedDistance)
            {
                tempDistance = fixedDistance;
                manager.objecToGo = temp[i];
                resource = temp[i].GetComponent<ResourceStats>();
                continue;
            }
        }
    }

    //Anthill behaviour

    private void AnthillJoining()
    {
        inAntHill = true;
        manager.enabled = false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        manager.pathPosition = antHill.store.transform.position;
        manager.startPath = true;
        transform.position = antHill.exit.transform.position;
    }
    private void AntHillInside()
    {
        if(inAntHill)
        {
            if(manager.pathPosition != new Vector2(antHill.store.transform.position.x,antHill.store.transform.position.y))
            {
                pathPos2 = (pathPos2 == new Vector2(0,0)) ? new Vector2(Controller.clickedPosition.x,Controller.clickedPosition.y) : pathPos2;
                manager.pathPosition = antHill.exit.transform.position;
            }
        }

        if (detectionZone.nearResource.Length > 0 && inAntHill)
        {
            for (int i = 0; i < detectionZone.nearResource.Length; i++)
            {
                if (detectionZone.nearResource[i] == antHill.store && !manager.startPath)
                {
                    StartCoroutine(DepositInventory());
                }
            }
        }
        AnthillLeaving();
    }
    private void AnthillLeaving()
    {
        if (detectionZone.nearResource.Length > 0 && manager.pathPosition != new Vector2(antHill.store.transform.position.x,antHill.store.transform.position.y) && inAntHill)
        {
            if(pathPos2 == new Vector2(0.0f,0.0f))
            {
                pathPos2 = (manager.objecToGo == null && new Vector3(manager.pathPosition.x,manager.pathPosition.y) != antHill.exit.transform.position) ? manager.pathPosition : new Vector2(manager.objecToGo.transform.position.x,manager.objecToGo.transform.position.y);
            }
            else if(manager.objecToGo != null && objectToGotemp != manager.objecToGo)
            {
                pathPos2 = Controller.clickedPosition;
            }
            if(manager.objecToGo != null && manager.objecToGo != resource.gameObject)
            {
                resource = manager.objecToGo.GetComponent<ResourceStats>();
                manager.pathPosition = manager.objecToGo.transform.position;
            }//Soluciona el problema de que cambio de recurso
            manager.pathPosition = antHill.exit.transform.position;
            manager.startPath = true;
            if(detectionZone.nearResource.Length > 0)
            {
                for (int i = 0; i < detectionZone.nearResource.Length; i++)
                {
                    if (detectionZone.nearResource[i] == antHill.exit)
                    {
                        manager.pathPosition = (objectToGotemp != null && manager.objecToGo != null) ? pathPos2 : new Vector2(Controller.clickedPosition.x,Controller.clickedPosition.y);
                        inAntHill = false;
                        manager.enabled = true;
                        GetComponent<SpriteRenderer>().enabled = true;
                        transform.position = new Vector3(0.0f, -5.0f, 0.0f);
                    }
                }
            }
        }
        else if (manager.objecToGo == null && inAntHill)
        {
            pathPos2 = Controller.clickedPosition;
        }
        else if(manager.objecToGo != null && inAntHill)
        {
            pathPos2 = manager.objecToGo.transform.position;
        }

    }

    //En caso de que vaya a un sitio sin recurso, regresar a su trabajo despues de cierto tiempo
    private IEnumerator BackToWork()
    {
        if(!backToWorkHelper && !manager.startPath)
        {
            backToWorkHelper = true;
            objectToGotemp = (objectToGotemp == null || manager.objecToGo != null && objectToGotemp != manager.objecToGo) ? manager.objecToGo : objectToGotemp;
            if (manager.objecToGo == null)
            {
                yield return new WaitForSecondsRealtime(2.0f);
                if(ownInventory.totalCollected >= manager.GetStrenght())
                {
                    manager.objecToGo = objectToGotemp;
                    ReturnToAnthill();
                    Debug.Log("OJ");
                }
                else if (manager.objecToGo == null && resource != null)
                {
                    manager.objecToGo = objectToGotemp;
                    manager.startPath = true;
                    manager.pathPosition = objectToGotemp.transform.position;
                }
            }
            backToWorkHelper = false;
        }
    }


    public float DistanceP1P2(float P1, float P2)
    {
        float distance = Mathf.Abs(P1 - P2);
        return distance;
    }
}
