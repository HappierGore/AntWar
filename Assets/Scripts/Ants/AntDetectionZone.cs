 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntDetectionZone : MonoBehaviour
{
    public Vector2[] positions = new Vector2[4];
    public Vector2[] fixedPositions = new Vector2[4];
    public GameObject[] nearResource;
    private bool alreadyLookingFor = false;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(WhatObjectIsNear());
        for (int i = 0; i < positions.Length; i++)
        {
            fixedPositions[i].x = transform.position.x + positions[i].x;
            fixedPositions[i].y = transform.position.y + positions[i].y;
        }
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            fixedPositions[i].x = transform.position.x + positions[i].x;
            fixedPositions[i].y = transform.position.y + positions[i].y;
        }
        Gizmos.DrawLine(fixedPositions[0], fixedPositions[1]);
        Gizmos.DrawLine(fixedPositions[1], fixedPositions[2]);
        Gizmos.DrawLine(fixedPositions[2], fixedPositions[3]);
        Gizmos.DrawLine(fixedPositions[3], fixedPositions[0]);
    }
    private IEnumerator WhatObjectIsNear()
    {
        GameObject[] detected = GameObject.FindGameObjectsWithTag("detector");
        nearResource = new GameObject[detected.Length];
        Vector2[] tempPos = new Vector2[detected.Length];
        if (!alreadyLookingFor)
        {
            alreadyLookingFor = true;
            for (int i = 0; i < detected.Length; i++)
            {
                tempPos = detected[i].GetComponent<OtherDetectionZone>().fixedPositions;
                //fixedPositions[0].x < tempPos.x - offset
                if ((fixedPositions[0].x < tempPos[0].x || fixedPositions[0].x < tempPos[1].x) && (fixedPositions[1].x > tempPos[0].x || fixedPositions[1].x > tempPos[1].x)
                    && (fixedPositions[1].y > tempPos[2].y || fixedPositions[1].y > tempPos[1].y) && (fixedPositions[2].y < tempPos[2].y || fixedPositions[2].y < tempPos[1].y))
                {
                    nearResource[i] = detected[i].transform.parent.gameObject;
                }
            }
            nearResource = Controller.ResizeArray(nearResource);
            alreadyLookingFor = false;
        }
        yield return new WaitForEndOfFrame();
    }

}
