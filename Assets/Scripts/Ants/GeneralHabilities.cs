using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralHabilities
    { 
    private GameObject ant;
    private bool initialized = false, alreadyMoving = false, alreadyMovingY = false;
    private Vector2 objectPosition;
    public float offset = 0.2f, currentXPath = 0.0f, currentYPath = 0.0f;
       


    public bool pathXCompleted = false, pathYCompleted = false;
    public void Initialize(GameObject antReference)
    {
        ant = antReference;
        initialized = true;
    }

    public IEnumerator MoveXTo2(Vector2 destination, float Speed, bool randomOffset = true)
    {
        currentXPath = destination.x;
        if (!alreadyMoving)
        {
            alreadyMoving = true;
            offset = (randomOffset && !pathXCompleted) ? Random.Range(0.1f, 0.6f) : offset;
            if (ant.transform.position.x < destination.x - offset)
            {
                pathXCompleted = false;
                while (ant.transform.position.x < destination.x - offset && currentXPath == destination.x)
                {
                    currentXPath = destination.x;
                    ant.transform.position = new Vector2(ant.transform.position.x + Speed * Time.deltaTime, ant.transform.position.y);
                    yield return new WaitForEndOfFrame();
                }
            }
            else if (ant.transform.position.x > destination.x + offset)
            {
                pathXCompleted = false;
                while (ant.transform.position.x > destination.x + offset && currentXPath == destination.x)
                {
                    currentXPath = destination.x;
                    ant.transform.position = new Vector2(ant.transform.position.x + Speed * Time.deltaTime * -1.0f, ant.transform.position.y);
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                currentXPath = 0.0f;
                pathXCompleted = true;
            }
            alreadyMoving = false;
        }
        yield return new WaitForEndOfFrame();

    }
    public IEnumerator MoveYTo2(Vector2 destination, float Speed, bool randomOffset = true)
    {
        currentYPath = destination.y;
        if (!alreadyMovingY)
        {
            alreadyMovingY = true;
            offset = (randomOffset && !pathYCompleted) ? Random.Range(0.1f, 0.6f) : offset;
            if (ant.transform.position.y < destination.y - offset)
            {
                pathYCompleted = false;
                while (ant.transform.position.y < destination.y - offset && currentYPath == destination.y)
                {
                    currentYPath = destination.y;
                    ant.transform.position = new Vector2(ant.transform.position.x, ant.transform.position.y + Speed * Time.deltaTime);
                    yield return new WaitForEndOfFrame();
                }
            }
            else if (ant.transform.position.y > destination.y + offset)
            {
                pathYCompleted = false;
                while (ant.transform.position.y > destination.y + offset && currentYPath == destination.y)
                {
                    currentYPath = destination.y;
                    ant.transform.position = new Vector2(ant.transform.position.x, ant.transform.position.y + Speed * Time.deltaTime * -1.0f);
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                currentYPath = 0.0f;
                pathYCompleted = true;
            }
            alreadyMovingY = false;
        }
        yield return new WaitForEndOfFrame();

    }

    private bool CheckInitialized()
    {
        if(initialized)
        {
            return true;
        }
        Debug.LogError("You have to initialize the class using the methotd: Initialize()");
        return false;
    }
}
