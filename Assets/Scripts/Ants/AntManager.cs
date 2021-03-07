using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AntManager : MonoBehaviour
{
    
    public bool startInitialize = false, startPath = false;
    public Vector2 pathPosition;
    public GameObject objecToGo = null;
    public enum groups { group1, group2, group3, group4, group5, group6, group7, group8, group9, group10 };
    public enum type { worker, soldier };
    private float health = 5.0f, speed = 1.0f, damage = 3.0f, strenght = 1.0f;
    private string antType, antGroup = "none";
    [HideInInspector] public bool checking, selected;
    private Controller controller;
    private SpriteRenderer selectedImage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(startInitialize)
        {
            Initialize();
        }
        StartCoroutine(CheckSelected());
        if (Input.GetKeyDown(KeyCode.S))
        {
            db.UpdateAntData(gameObject.name, antType, transform.position, antGroup, health, speed, damage, strenght);
        }
        FollowUI();
    }

    private IEnumerator CheckSelected()
    {
        if (!checking)
        {
            checking = true;
            for (int i = 0; i < controller.antSelected.Length; i++)
            {
                if (controller.antSelected[i] == this.gameObject)
                {
                    selected = true;
                    break;
                }
                selected = false;
            }

            yield return new WaitForEndOfFrame();
            checking = false;
        }
    }

    private void Initialize()
    {
        selectedImage = transform.GetChild(0).GetComponent<SpriteRenderer>();
        controller = GameObject.Find("controller").GetComponent<Controller>();
        if (!db.CheckAlreadyExists(gameObject.name))
        {
            db.NewAnt(gameObject.name, antType, transform.position, antGroup, health, speed, damage, strenght);
        }
        startInitialize = false;
    }
    // UI
    /*private void UIInit()
    {
        selectedImage = transform.GetChild(0).GetComponent<Image>();
        selectedImage.gameObject.name = gameObject.name + " select";
        selectedImage.transform.SetParent(GameObject.Find("Canvas").transform);
        selectedImage.rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        selectedImage.rectTransform.sizeDelta = new Vector2(50.0f, 50.0f);
    }*/
    private void FollowUI()
    {
        selectedImage.transform.position = gameObject.transform.position;
        if(selected)
        {
            selectedImage.enabled = true;
            return;
        }
        selectedImage.enabled = false;
    }

    //Setters
    public void SetTypeAnt(string type)
    {
        antType = type;
    }
    public void SetGroup(string group)
    {
        antGroup = group;
    }
    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
    public void SetStrenght(float newStrenght)
    {
        strenght = newStrenght;
    }
    //Getters
    public float GetSpeed()
    {
        return speed;
    }
    public string GetAntType()
    {
        return antType;
    }
    public string GetGroup()
    {
        return antGroup;
    }
    public float GetHealth()
    {
        return health;
    }
    public float GetDamage()
    {
        return damage;
    }
    public float GetStrenght()
    {
        return strenght;
    }
}
