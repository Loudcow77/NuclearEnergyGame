using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;

public class ObjectPlacer : MonoBehaviour
{
    private AudioSource audioSource;
    
    static public bool OnMenu;

    public Transform centerPoint;

    public GameObject buildPanel;
    //public GameObject buildTut;
    public static bool isDoneBuilding = false;

    //public GameObject destroyButton;

	public GameObject[] prefabs;

    public bool builtOn = false;

    public static bool firstBuild = false;

    public ResourceManager resourceManager;

    public static bool tutDone;
    float delay = 0.2f;
    float clickTime;

    //public float TimeTillDestruction = 360f;

    public Image loadingToBuildBar;
    public Text loadingToBuildPercent;
    public bool isBuiding;
    public float timeTakingWhenBuild;
    float timeTakingToBuild;
    int prefabNumber;


    public int counterForBuildMenu = 0;

    void Start()
    {
        OnMenu = false;
        isBuiding = false;
        audioSource = GetComponent<AudioSource>();
        loadingToBuildBar.enabled = false;
        loadingToBuildPercent.enabled = false;
    }
    private void Update()
    {
        //if (isBuiding && !SeasonManager.popup)
        //    print("Yesssssssss open");
        //else
        //    print("Nooooooooo");
            //if (TimeTillDestruction >= 0f)
            //{
            //    TimeTillDestruction -= Time.deltaTime;
            //}
        if (isBuiding && !SeasonManager.popup)
        {
            timeTakingWhenBuild += Time.deltaTime;

            loadingToBuildPercent.text = (int)(timeTakingWhenBuild / timeTakingToBuild * 100) + "%";
            loadingToBuildBar.fillAmount = timeTakingWhenBuild / timeTakingToBuild;

            if (timeTakingWhenBuild >= timeTakingToBuild)
            {
                GameObject tmp = Instantiate(prefabs[prefabNumber], centerPoint.position, Quaternion.identity);
                tmp.transform.parent = this.gameObject.transform;
                tmp.gameObject.name = tmp.gameObject.name + this.transform.parent.gameObject.name;
                resourceManager.buildingsList.Add(tmp);
                FindObjectOfType<AudioManager>().Play("buildnoise");
                //resourceManager.BuildingCalculations(tmp);

                loadingToBuildBar.enabled = false;
                loadingToBuildPercent.enabled = false;
                isBuiding = false;
                
                isDoneBuilding = true;
                
            }
        }
    }
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickTime = Time.time;
        }
    }

    private void OnMouseUp()
    {
        if (!OnMenu && !SeasonManager.popup)
        {
            if (Input.GetMouseButtonUp(0))
            {
                FindObjectOfType<AudioManager>().Play("Click");

                if (Time.time - clickTime <= delay)
                {
                    RaycastHit hitInfo = new RaycastHit();
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (builtOn == false)
                    {
                        //if (Physics.Raycast(ray, out hitInfo))
                        //{
                        //if (hitInfo.collider.gameObject.tag == "BuildSquare")
                        //{
                        buildPanel.SetActive(true);
                        OnMenu = true;
                        
                        //Enable/Disable Buttons for Nuclear Plant
                        if (ResourceManager.totalMoney < 1000)
                            EnableDisableOptions(5, false);
                            //buildPanel.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<Button>().enabled = false;
                        else if (ResourceManager.totalMoney >= 1000)
                            EnableDisableOptions(5, true);
                            //buildPanel.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<Button>().enabled = true;

                        //Enable/Disable Buttons for Solar Farm
                        if (ResourceManager.totalMoney < 200)
                            EnableDisableOptions(6, false);
                        else if (ResourceManager.totalMoney >= 200)
                            EnableDisableOptions(6, true);

                        //Enable/Disable Buttons for Fossil Fuel Plant
                        if (ResourceManager.totalMoney < 400)
                            EnableDisableOptions(4, false);
                        else if (ResourceManager.totalMoney >= 400)
                            EnableDisableOptions(4, true);

                        //Enable/Disable Buttons for Wind Turbine Farm
                        if (ResourceManager.totalMoney < 250)
                            EnableDisableOptions(3, false);
                        else if (ResourceManager.totalMoney >= 250)
                            EnableDisableOptions(3, true);

                        //Enable/Disable Buttons for Oil Plant
                        if (ResourceManager.totalMoney < 300)
                            EnableDisableOptions(1, false);
                        else if (ResourceManager.totalMoney >= 300)
                            EnableDisableOptions(1, true);

                        //Enable/Disable Buttons for Hydrogen Plant
                        if (ResourceManager.totalMoney < 550)
                            EnableDisableOptions(0, false);
                        else if (ResourceManager.totalMoney >= 550)
                            EnableDisableOptions(0, true);


                        Debug.Log("Build Panel Open");
                        //}
                        //}
                        
                        firstBuild = true;
                    }
                    else if (builtOn == true)
                    {
                        //print("Already Built Here");
                        //if (TimeTillDestruction <= 0f)
                        //{
                        //    destroyButton.SetActive(true);
                        //}
                    }
                }
            }                          
        }
    }

    private void EnableDisableOptions(int childSpot, bool isActive)
    {
        //Enables and Disables button interaction
        buildPanel.transform.GetChild(childSpot).gameObject.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = isActive;
    }

    public void PlaceBuilding (int buildingType)
    {
        isBuiding = true;
        timeTakingWhenBuild = 0.0f;
        prefabNumber = buildingType;
        timeTakingToBuild = prefabs[buildingType].GetComponent<BuildingResources>().timeToBuild;
        loadingToBuildBar.enabled = true;
        loadingToBuildPercent.enabled = true;
        resourceManager.BuildingCalculations(prefabs[buildingType]);
        FindObjectOfType<AudioManager>().Play("Click");


        builtOn = true;
        buildPanel.SetActive(false);
        OnMenu = false;

        tutDone = true;
    }

    public void PlaceBuildingWhenGameStart(int buildingType)
    {
        builtOn = true;
        GameObject tmp = Instantiate(prefabs[buildingType], centerPoint.position, Quaternion.identity);
        tmp.transform.parent = this.gameObject.transform;
        resourceManager.buildingsList.Add(tmp);
        resourceManager.BuildingCalculations(tmp);
    }

    public void CloseBuildPanel()
    {
        buildPanel.SetActive(false);
        OnMenu = false;
    }

    //These two fuctions are used for build menu stuff
    public void CountAdd()
    {
        counterForBuildMenu++;
    }
    public void CountSubtract()
    {
        counterForBuildMenu--;
    }

    //public void DestroyBuildings()
    //{
    //    gameObject.transform.GetChild(1).gameObject.SetActive(false);
    //    builtOn = false;
    //    destroyButton.SetActive(false);
    //}

    //public void CloseDestroyPanel()
    //{
    //    destroyButton.SetActive(false);
    //}
}