using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRubyShared;
public class Controller : MonoBehaviour
{
    [SerializeField] Camera camara;
    public LineRenderer[] lines = new LineRenderer[4];
    public GameObject[] antSelected;
    public GameObject resourceSelected;
    private TapGestureRecognizer tapGesture;
    private TapGestureRecognizer doubleTapGesture;
    private TapGestureRecognizer tripleTapGesture;
    private ScaleGestureRecognizer scaleGesture;
    private LongPressGestureRecognizer longPressGesture;
    private PanGestureRecognizer panGesture;
    private Vector2 startClickPos, endClickPos;
    private float scalingZ = 5.0f;
    public static Vector3 clickedPosition;
    // Start is called before the first frame update
    void Start()
    {
        CreateDoubleTapGesture();
        CreateTapGesture();
        CreatePanGesture();
        CreateScaleGesture();
        CreateLongPressGesture();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    // --------------- GESTOS --------------
    private void CreateTapGesture()
    {
        tapGesture = new TapGestureRecognizer();
        tapGesture.StateUpdated += TapGestureCallback;
        tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
        FingersScript.Instance.AddGesture(tapGesture);
    }
    private void CreateDoubleTapGesture()
    {
        doubleTapGesture = new TapGestureRecognizer();
        doubleTapGesture.NumberOfTapsRequired = 2;
        doubleTapGesture.StateUpdated += DoubleTapGestureCallback;
        FingersScript.Instance.AddGesture(doubleTapGesture);
    }
    private void CreateScaleGesture()
    {
        scaleGesture = new ScaleGestureRecognizer();
        scaleGesture.StateUpdated += ScaleGestureCallback;
        FingersScript.Instance.AddGesture(scaleGesture);
    }
    private void CreateLongPressGesture()
    {
        longPressGesture = new LongPressGestureRecognizer();
        longPressGesture.MaximumNumberOfTouchesToTrack = 1;
        longPressGesture.StateUpdated += LongPressGestureCallback;
        FingersScript.Instance.AddGesture(longPressGesture);
    }
    private void CreatePanGesture()
    {
        panGesture = new PanGestureRecognizer();
        panGesture.MinimumNumberOfTouchesToTrack = 1;
        panGesture.StateUpdated += PanGestureCallback;
        FingersScript.Instance.AddGesture(panGesture);
    }

    // Activos
    private void TapGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            InitSelectionBox(gesture.FocusX, gesture.FocusY);
            TakeResourceTapOption();
            ResetPathOfAntsSelected();
            OnResourceAndAntsSelected();
        }
    }
    private void DoubleTapGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            InitSelectionBox(gesture.FocusX, gesture.FocusY);
            TakeAntsDoubleTapOption();
            antSelected = ResizeArray(antSelected);
        }
    }
    private void LongPressGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Began)
        {
            InitSelectionBox(gesture.FocusX, gesture.FocusY);
            resourceSelected = null;
            print("Start Long press");
            //BeginDrag(gesture.FocusX, gesture.FocusY);
        }
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            print("Ejecutando long press");
            UpdateSelectionBox(gesture.FocusX, gesture.FocusY);
            TakeAntsHoldOption();
            //DragTo(gesture.FocusX, gesture.FocusY);
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            ResetLines();
            //Debug.Log("Before: " + antSelected.Length);
            antSelected = ResizeArray(antSelected);
            //Debug.Log("After: " + antSelected.Length);
            print("Terminó long press");
            // EndDrag(longPressGesture.VelocityX, longPressGesture.VelocityY);
        }
    }
    private void ScaleGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            scalingZ = scaleGesture.ScaleMultiplier * camara.orthographicSize;
            camara.orthographicSize = scalingZ;
        }
    }
    private void PanGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            float deltaX = panGesture.DeltaX / 100.0f;
            float deltaY = panGesture.DeltaY / 100.0f;
            Vector3 pos = camara.transform.position;
            pos.x += deltaX * -1;
            pos.y += deltaY * -1;
            camara.transform.position = pos;
        }
    }

    private void InitSelectionBox(float screenX, float screenY)
    {
        Vector3 clickPos = new Vector3(screenX, screenY, 0.0f);
        clickPos = Camera.main.ScreenToWorldPoint(clickPos);
        startClickPos = clickPos;
        clickedPosition = clickPos;
    }
    private void UpdateSelectionBox(float screenX, float screenY)
    {
        endClickPos = new Vector3(screenX, screenY, 0.0f);
        endClickPos = Camera.main.ScreenToWorldPoint(endClickPos);
        Vector2 clickPos1 = new Vector2(endClickPos.x, startClickPos.y);
        Vector2 clickPos2 = new Vector2(startClickPos.x, endClickPos.y);
        Vector2 clickPos3 = new Vector2(clickPos1.x, endClickPos.y);
        DrawLine(startClickPos, clickPos1, lines[0]);
        DrawLine(startClickPos, clickPos2, lines[1]);
        DrawLine(clickPos1, clickPos3, lines[2]);
        DrawLine(clickPos3, clickPos2, lines[3]);
    }
    private void TakeAntsHoldOption()
    {
        GameObject[] allAnts = GameObject.FindGameObjectsWithTag("ant");
        antSelected = new GameObject[allAnts.Length];
        for (int i = 0; i < allAnts.Length; i++)
        {
            if (startClickPos.x < endClickPos.x)
            {
                if (startClickPos.y > endClickPos.y)
                {
                    if (allAnts[i].transform.position.x > startClickPos.x && allAnts[i].transform.position.x < endClickPos.x
                        && allAnts[i].transform.position.y < startClickPos.y && allAnts[i].transform.position.y > endClickPos.y)
                    {
                        antSelected[i] = (allAnts[i].GetComponent<AntManager>().isActiveAndEnabled) ? allAnts[i] : null;
                    }
                }
                else if (startClickPos.y < endClickPos.y)
                {
                    if (allAnts[i].transform.position.x > startClickPos.x && allAnts[i].transform.position.x < endClickPos.x
                        && allAnts[i].transform.position.y > startClickPos.y && allAnts[i].transform.position.y < endClickPos.y)
                    {
                        antSelected[i] = (allAnts[i].GetComponent<AntManager>().isActiveAndEnabled) ? allAnts[i] : null;
                    }
                }
            }
            else if (startClickPos.x > endClickPos.x)
            {
                if (startClickPos.y > endClickPos.y)
                {
                    if (allAnts[i].transform.position.x < startClickPos.x && allAnts[i].transform.position.x > endClickPos.x
                        && allAnts[i].transform.position.y < startClickPos.y && allAnts[i].transform.position.y > endClickPos.y)
                    {
                        antSelected[i] = (allAnts[i].GetComponent<AntManager>().isActiveAndEnabled) ? allAnts[i] : null;
                    }
                }
                else if (startClickPos.y < endClickPos.y)
                {
                    if (allAnts[i].transform.position.x < startClickPos.x && allAnts[i].transform.position.x > endClickPos.x
                        && allAnts[i].transform.position.y > startClickPos.y && allAnts[i].transform.position.y < endClickPos.y)
                    {
                        antSelected[i] = (allAnts[i].GetComponent<AntManager>().isActiveAndEnabled) ? allAnts[i] : null;
                    }
                }
            }

        }
    }
    private void TakeAntsDoubleTapOption()
    {
        GameObject[] allAnts = GameObject.FindGameObjectsWithTag("ant");
        antSelected = new GameObject[allAnts.Length];
        float tempScalingZ = scalingZ / 2.0f;
        for (int i = 0; i < allAnts.Length; i++)
        {
            if (allAnts[i].transform.position.x > startClickPos.x - tempScalingZ && allAnts[i].transform.position.x < startClickPos.x + tempScalingZ
                && allAnts[i].transform.position.y < startClickPos.y + tempScalingZ && allAnts[i].transform.position.y > startClickPos.y - tempScalingZ)
            {
                antSelected[i] = (allAnts[i].GetComponent<AntManager>().isActiveAndEnabled) ? allAnts[i] : null;
            }
        }
    }
    private void TakeResourceTapOption()
    {
        GameObject[] allResources = GameObject.FindGameObjectsWithTag("resource");
        for (int i = 0; i < allResources.Length; i++)
        {
            if (allResources[i].transform.position.x > startClickPos.x - 0.3f && allResources[i].transform.position.x < startClickPos.x + 0.3f
                && allResources[i].transform.position.y < startClickPos.y + 0.3f && allResources[i].transform.position.y > startClickPos.y - 0.3f)
            {
                resourceSelected = allResources[i];
                break;
            }
            resourceSelected = null;
        }

    }
    

    public float DistanceP1P2(float P1, float P2)
    {
        float distance = Mathf.Abs(P1 - P2);
        return distance;
    }
    public void DrawLine(Vector2 origin, Vector2 tracker, LineRenderer lineRenderer)
    {
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, tracker);
    }
    private void ResetLines()
    {
        lines[0].SetPosition(0, new Vector3(0.0f, 0.0f, 0.0f));
        lines[0].SetPosition(1, new Vector3(0.0f, 0.0f, 0.0f));
        lines[1].SetPosition(0, new Vector3(0.0f, 0.0f, 0.0f));
        lines[1].SetPosition(1, new Vector3(0.0f, 0.0f, 0.0f));
        lines[2].SetPosition(0, new Vector3(0.0f, 0.0f, 0.0f));
        lines[2].SetPosition(1, new Vector3(0.0f, 0.0f, 0.0f));
        lines[3].SetPosition(0, new Vector3(0.0f, 0.0f, 0.0f));
        lines[3].SetPosition(1, new Vector3(0.0f, 0.0f, 0.0f));
        endClickPos = new Vector2(0, 0);
        startClickPos = new Vector2(0, 0);
    }

    // Funciones del juego (Mecánicas)
    private void OnResourceAndAntsSelected()
    {
        if(resourceSelected != null && antSelected.Length > 0 )
        {
            for (int i = 0; i < antSelected.Length; i++)
            {
                AntManager manager = antSelected[i].GetComponent<AntManager>();
                manager.startPath = true;
                manager.objecToGo = resourceSelected.gameObject;
                manager.pathPosition = resourceSelected.transform.position;
            }
        }
    }
    private void ResetPathOfAntsSelected()
    {
        if(antSelected.Length != 0)
        {
            for (int i = 0; i < antSelected.Length; i++)
            {
                AntManager manager = antSelected[i].GetComponent<AntManager>();
                manager.startPath = true;
                manager.objecToGo = null;
                manager.pathPosition = startClickPos;
            }
        }
    }

    public static GameObject[] ResizeArray(GameObject[] ArrayToResize)
    {
        int size = 0;
        for (int i = 0; i < ArrayToResize.Length; i++)
        {
            if(ArrayToResize[i] != null)
            {
                size += 1;
            }
        }
        GameObject[] temp = new GameObject[size];
        for (int i = 0; i < ArrayToResize.Length; i++)
        {
            if(ArrayToResize[i] != null)
            {
                //print("Primer condicional en " + i);
                for (int j = 0; j < temp.Length; j++)
                {
                    if(temp[j] == null)
                    {
                       // print("Segunda condicional i = " + i + " j = " + j);
                        temp[j] = ArrayToResize[i];
                        break;
                    }
                }
            }
        }
        
        return temp;
    }

    
}

