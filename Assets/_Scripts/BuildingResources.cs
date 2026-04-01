using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

public class BuildingResources : MonoBehaviour
{
    public static bool firstOnOff = false;

    public float timeToBuild; // In second
    public float costToBuild; // In billions
    //public float costPerDay; // In billions
    public float costToProduceEnergy; // In billions ($0.17/kWh)
    public float energyIncome; // The are for where the building is built. 1 = residential, 2 = industrial and 3 = agriculture NO LONGER CONSIDERING LOCATION
    //public float costPerNight;
    //public float size;
    public float emissionsPerDay; // In Tonnes CO2/Day (360 in days right now)
    //public float emissionsPerNight;
    public float energyProductionPerDay; // In GWh
    public GameObject ModelSelf;
    public static bool firstDestroy = false;
    public bool isActive;

    float delay = 0.2f;
    float clickTime;

    public GameObject OuterBar;
    public float maxHydrogenStorage = 0.3f;
    public float hydrogenStorage = 0.0f;
    public float blinkSpeed = 0.2f;

    public GameObject powerPlantControlMenu;
    public Image turnOn;
    public Image turnOff;
    public Text turnOnOff;
    public Button destroy;
    public static bool turnOnTheDestroy;
    public static bool firstOnOffClick;

    public GameObject light1;
    public GameObject light2;

    public enum buidingType
    {
        nuclear,
        solar,
        coal,
        wind,
        Oil,
        hydrogen,
        hydro
    };
    public buidingType powerType;


    private void Start()
    {
        if (powerType != buidingType.hydrogen) {
            isActive = true;
            powerPlantControlMenu.SetActive(false);
            turnOn.enabled = true;
            turnOff.enabled = false;
            turnOnOff.text = "TURN OFF";
            destroy.interactable = false;
            light1.SetActive(true);
            light2.SetActive(true);
        }
        else
        {
            isActive = false;
            powerPlantControlMenu.SetActive(false);
            turnOn.enabled = false;
            turnOff.enabled = true;
            turnOnOff.text = "TURN ON";
            destroy.interactable = false;
            ModelSelf.GetComponent<Renderer>().material.color
                            = new Color32(
                                (byte)(ModelSelf.GetComponent<Renderer>().material.color.r * 255 * 0.33),
                                (byte)(ModelSelf.GetComponent<Renderer>().material.color.g * 255 * 0.33),
                                (byte)(ModelSelf.GetComponent<Renderer>().material.color.b * 255 * 0.33), 1);
            light1.SetActive(false);
            light2.SetActive(false);
        }
    }

    private void Update()
    {
        if (powerType != buidingType.hydro)
        {
            if (isActive)
            {
                ModelSelf.GetComponent<Outline>().OutlineColor = Color.green;
            }
            else
            {
                ModelSelf.GetComponent<Outline>().OutlineColor = Color.red;
            }
        }
        if (powerType == buidingType.hydrogen)
        {
            if (hydrogenStorage < maxHydrogenStorage)
            {
                OuterBar.transform.localScale = new Vector3(1, 10 * (hydrogenStorage / maxHydrogenStorage), 1);
                OuterBar.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                OuterBar.transform.localScale = new Vector3(1, 10, 1);
                if (Time.fixedTime % .5 < blinkSpeed)
                {
                    OuterBar.GetComponent<Renderer>().enabled = true;
                }
                else
                {
                    OuterBar.GetComponent<Renderer>().enabled = false;
                }
            }
        }

        if (turnOnTheDestroy)
        {
            destroy.interactable = true;
        }
        else
            destroy.interactable = false;

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
        if (!ObjectPlacer.OnMenu && !SeasonManager.popup && powerType != buidingType.hydro)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (Time.time - clickTime <= delay)
                {
                    powerPlantControlMenu.SetActive(true);
                    ObjectPlacer.OnMenu = true;
                    firstOnOffClick = true;
                }
            }
        }
    }

    public void turnBuidingOnOrOff()
    {
        if (isActive)
        {
            ModelSelf.GetComponent<Renderer>().material.color
                = new Color32(
                    (byte)(ModelSelf.GetComponent<Renderer>().material.color.r * 255 * 0.33),
                    (byte)(ModelSelf.GetComponent<Renderer>().material.color.g * 255 * 0.33),
                    (byte)(ModelSelf.GetComponent<Renderer>().material.color.b * 255 * 0.33), 1);
            turnOn.enabled = false;
            turnOff.enabled = true;
            turnOnOff.text = "TURN OFF";
            isActive = false;
            FindObjectOfType<AudioManager>().Play("PowerOff");
            firstOnOff = true;
            light1.SetActive(false);
            light2.SetActive(false);
        }
        else
        {
            ModelSelf.GetComponent<Renderer>().material.color
                = new Color32(
                (byte)(ModelSelf.GetComponent<Renderer>().material.color.r * 255 * 3),
                (byte)(ModelSelf.GetComponent<Renderer>().material.color.g * 255 * 3),
                (byte)(ModelSelf.GetComponent<Renderer>().material.color.b * 255 * 3), 1);
            turnOn.enabled = true;
            turnOff.enabled = false;
            turnOnOff.text = "TURN ON";
            isActive = true;
            FindObjectOfType<AudioManager>().Play("PowerOn");


            light1.SetActive(true);
            light2.SetActive(true);
        }
    }

    public void turnMenuOff()
    {
        powerPlantControlMenu.SetActive(false);
        ObjectPlacer.OnMenu = false;
    }

    public void destroyThisObject()
    {
        firstDestroy = true;
        ObjectPlacer.OnMenu = false;
        this.transform.parent.gameObject.GetComponent<ObjectPlacer>().resourceManager.buildingsList.Remove(this.gameObject);
        this.transform.parent.gameObject.GetComponent<ObjectPlacer>().builtOn = false;
        Destroy(this.gameObject);

        FindObjectOfType<AudioManager>().Play("Destroy");

        ResourceManager.totalMoney -= 700;
    }
 }
