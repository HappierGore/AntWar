using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupsManager : MonoBehaviour
{
    //UI
    [SerializeField] private Text  actualName;
    [SerializeField] InputField inputName;
    [SerializeField] private Scrollbar scrollbarSaveGroup, scrollbarLoadGroup;
    [SerializeField] private GameObject[] antsOfGroup;
    public string[] obtanied;
    private Controller controller;
    private int groupSelected;
    private string groupInputName;

    private string[] groupsName = new string[10];
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller>();
        LoadGroupsNamesFromDB();   
    }

    // Update is called once per frame
    void Update()
    {
        LoadGroupNameToUI();
    }

    public void UpdateValues()
    {
        groupSelected = FixedScrollBarValue(scrollbarSaveGroup.value);
        groupInputName = inputName.text;
        inputName.text = "";
        db.UpdateGroupName(groupSelected, groupInputName);
        LoadGroupsNamesFromDB();
        SaveAntsToDB();
    }

    private int FixedScrollBarValue(float scrollvalue)
    {
        int temp = 0;
        float value = scrollvalue * 10;
        if(value <= 0)
        {
            return 1;
        }
        int tempI = 1;
        for (int i = 0; i < 10; i++)
        {
            if(value > tempI && value < tempI + 1 )
            {
                temp = tempI + 1;
                break;
            }
            tempI += 1;
        }
        if(temp == 0)
        {
            return 1;
        }
        else if(temp == 11)
        {
            return 10;
        }
        return temp;
    }

    private void SaveAntsToDB()
    {
        string buildName = "";
        if(controller.antSelected != null)
        {
            for (int i = 0; i < controller.antSelected.Length; i++)
            {
                buildName += "/" + controller.antSelected[i].name;
            }
            db.SaveAntsToGroup(FixedScrollBarValue(scrollbarSaveGroup.value), buildName);
            //Debug.Log(buildName);
        }

    }

    //Getters
    private void LoadGroupsNamesFromDB()
    {
        int temp = 1;
        for (int i = 0; i < groupsName.Length; i++)
        {
            groupsName[i] = db.GetGroupToUI(temp);
            temp++;
        }
    }
    private void LoadGroupNameToUI()
    {
        string temp = "";
        int tempI = 1;
        for (int i = 0; i < groupsName.Length; i++)
        {
            if(FixedScrollBarValue(scrollbarSaveGroup.value) == tempI )
            {
                temp = groupsName[i];
            }
            tempI++;
        }
        actualName.text = temp;
    }
    public void GetAntsFromGroup()
    {
        string temp = db.GetAntsOfGroup(FixedScrollBarValue(scrollbarLoadGroup.value));
        char helper = '0';
        obtanied = temp.Split(helper, '/');
        controller.antSelected = new GameObject[obtanied.Length];
        for (int i = 0; i < obtanied.Length; i++)
        {
            if (obtanied[i] != "")
            {
                controller.antSelected[i] = GameObject.Find(obtanied[i]);
            }
        }
        controller.antSelected = Controller.ResizeArray(controller.antSelected);
    }
}
