using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherDetectionZone : MonoBehaviour
{
    public Vector2[] positions = new Vector2[4];
    public Vector2[] fixedPositions = new Vector2[4];

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            fixedPositions[i].x = transform.position.x + positions[i].x;
            fixedPositions[i].y = transform.position.y + positions[i].y;
        }
    }
    private void OnDrawGizmos()
    {

        Gizmos.DrawLine(fixedPositions[0], fixedPositions[1]);
        Gizmos.DrawLine(fixedPositions[1], fixedPositions[2]);
        Gizmos.DrawLine(fixedPositions[2], fixedPositions[3]);
        Gizmos.DrawLine(fixedPositions[3], fixedPositions[0]);
    }
}
