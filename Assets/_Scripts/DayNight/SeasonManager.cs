using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEngine.XR.WSA.Input;

public class SeasonManager : MonoBehaviour
{
    private AudioSource audioSource;
   public enum Season {SPRING, SUMMER, FALL, WINTER};
   public enum Weather {SUN, XSUN, RAIN, SNOW};

    public Season currentSeason;
    public Weather currentWeather;

    float randomHeatSummer;
    float randomHeatSpring;
    float randomColdFall;
    float randomColdWinter;

    public bool isHotSpring;
    public bool isHotSummer;
    public bool isColdFall;
    public bool isColdWinter;

    public static bool canHydrogen;

    public static bool firstSeason = false;
    public static bool firstTemp = false;
    public bool firstSpring = false;
    public bool firstSummer = false;
    public bool firstFall = false;
    public static bool destroySeason = false;

    public ParticleSystem snow;
    public ParticleSystem leaves;
    public ParticleSystem rain;
    public ParticleSystem summerLeaves;

    [Header("Weather Textures")]
    public Image heat;
    public Image frost;
    public Image normal_icon;
    public Image cold_icon;
    public Image hot_icon;

    [Header("Textures")]
  

    public Material materialSpring;
    public Material materialSummer;
    public Material materialFall;
    public Material materialWinter;
    public GameObject thePlane;


    [Header("Time Settings")]
    public float seasonTime;
    public float springTime;
    public float summerTime;
    public float fallTime;
    public float winterTime;

    [Header("Light Settings")]
    public Light weatherLight;

    public Color springColor;
    public Color summerColor;
    public Color fallColor;
    public Color winterColor;

    public Color heatLight;
    public Color coldLight;

    public int currentYear;

    //Resource/Event/Decision Stuff
    private ResourceManager resourceManager;

    public GameObject decisionPanel;
    public GameObject eventPanel;
    public GameObject eventPanel2;
    public GameObject eventPanel3;
    public GameObject endGamePanel;
    public GameObject dataStore;

    private bool bestOption;

    private float carbonChoiceReductionBest;
    private float carbonChoiceReductionGood;

    static public bool popup;

    public bool isGameDoneSoon = false;

    //year text
    public Text yearTracker;

    public bool firstWinter;

    private void Start()
    {
        isHotSpring = false;
        isHotSummer = false;
        isColdFall = false;
        isColdWinter = false;

        this.currentSeason = Season.SPRING;
        this.currentWeather = Weather.SUN;
        this.currentYear = 2020;

        this.seasonTime = this.springTime;

        this.springColor = this.weatherLight.color;
        this.leaves.Stop();
        this.snow.Stop();
        this.summerLeaves.Stop();
        this.rain.Play();

        heat.enabled = false;
        frost.enabled = false;
        randomHeatSpring = 7f;
        //Resource Stuff
        resourceManager = GameObject.Find("Resource Manager").GetComponent<ResourceManager>();

        //string eventText;
        popup = true;

       // eventText = "Welcome to Net Zero. In this game you will build power plants to aid you in creating a power grid. " +
       //     "You will also be trying to get Carbon Emissions to net 0 by the year 2050(displayed by carbon gauge). " +
       //     "Your current goal is to start producing more energy than is being consumed. To do this you're going to need some power plants. " +
       //     "To build power plants click on an empty plot and select which plant you want to build."; 

        //SetEventText(eventText);
    }

    public void SetBestOptionTrueOrFalse(bool option)
    {
        bestOption = option;
        CarbonReductionChoice();
    }
    /*public void SetBestOptionFalse()
    {
        bestOption = false;
        CarbonReductionChoice();
    }*/

    public void SetPopupFalse()
    {
        if (!isGameDoneSoon)
            popup = false;
    }
    
    public void SetDecisionText(string theDecision, string opt1, string opt2)
    {
        decisionPanel.SetActive(true);
        decisionPanel.transform.GetChild(1).gameObject.GetComponent<Text>().text = theDecision;
        decisionPanel.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = opt1;
        decisionPanel.transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = opt2;

    }

    public void CarbonReductionChoice(/*float reduction*/)
    {
        if (bestOption)
            resourceManager.carbonReductions += carbonChoiceReductionBest;
        else
            resourceManager.carbonReductions += carbonChoiceReductionGood;

        //Display informative text somewhere, then make a continue button********
    }

    public void SetEventText(string theText)
    {
        eventPanel.SetActive(true);
        eventPanel.transform.GetChild(1).gameObject.GetComponent<Text>().text = theText;
    }

    public void ChangeSeason( Season seasonType)
    {
        //Decision Variables
        string decision;
        string option1;
        string option2;

        //Event Variables
        string eventText;

        if (seasonType != this.currentSeason)
        {
            switch(seasonType)
            {

                case Season.SPRING:
                    currentSeason = Season.SPRING;
                    
                    FindObjectOfType<AudioManager>().Play("SummerSound");
                    FindObjectOfType<AudioManager>().Stop("Winter");


                    randomHeatSpring = Random.Range(1f, 11f);
                    //NOTHING
                    if (currentYear == 2020)
                    {
                        //DELETE IF NOT NEEDED
                        //resourceManager.startEnergyNeed = 410;
                    }
                    //EVENT
                    else if (currentYear == 2035)
                    {

                        //Only EV will be sold (Electric Vehicles?)
                        destroySeason = true;

                        popup = true;

                        eventPanel2.SetActive(true);
                        eventPanel2.transform.GetChild(1).gameObject.GetComponent<Text>().text = "A new law has been passed, only electric vehicles will be sold. This will reduce your carbon emissions but require more energy to be produced!"; //WHat is the impact of this?

                        resourceManager.canDestroy = true;

                        //SetEventText(eventText);

                        resourceManager.energyNeed = 850000f; //Not sure if these are being used

                        resourceManager.startEnergyNeed = 570;

                        resourceManager.carbonReductions += 35;

                        BuildingResources.turnOnTheDestroy = true;

                        //add the tracking of the year for the UI 
                        yearTracker.text = "" + currentYear;
                    }
                    //NOTHING
                    else if (currentYear == 2050)
                    {
                        //SHOULD REMOVE IF NOT NEEDED                        
                        resourceManager.energyNeed = 1100000f; //Not sure if these are being used

                        //I wanna be your End Game! - Taylor Swift
                        if (isGameDoneSoon)
                        {
                            //set the end game panel to true
                            endGamePanel.SetActive(true);
                            canHydrogen = false;

                            //Tutorial.firstNight = false;
                            //Tutorial.isFirstBuild = false;
                            //Tutorial.firstOnOff = false;
                            //Tutorial.firstOnOffMenu = false;
                            //Tutorial.isFirstDestroy = false;

                            //ObjectPlacer.firstBuild = false;
                            //BuildingResources.firstDestroy = false;
                            //BuildingResources.firstOnOffClick = false;
                            //BuildingResources.firstOnOff = false;
                            //DayNight.isNight = false;
                        }

                        resourceManager.startEnergyNeed = 650;

                        //add the tracking of the year for the UI 
                        yearTracker.text = "" + currentYear;
                    }
                    break;
                case Season.SUMMER:

                    currentSeason = Season.SUMMER;
                    if (firstSpring == false)
                    {
                        firstSummer = true;
                        randomHeatSummer = 6f;
                        Debug.Log("FIRST SUMMER" + randomHeatSummer);
                    }
                    else
                    {
                        randomHeatSummer = Random.Range(1f, 11f);
                    }
                    //NOTHING
                    if (currentYear == 2020)
                    {
                        //SHOULD REMOVE IF NOT NEEDED
                        resourceManager.energyNeed = 700000f; //Not sure if these are being used

                        //resourceManager.startEnergyNeed = 410;
                    }
                    //DECISION
                    else if (currentYear == 2035)
                    {
                        //Outlaw Canadians driving on Tuesdays and Thursdays (53.5 million tons ) OR enforce Canadians can only eat meat on Mondays, Thursdays, and Saturdays (36 million Tons a year)

                        popup = true;
                        FindObjectOfType<AudioManager>().Play("Alert");

                        decision = "Time to make a choice! Will you outlaw Canadians driving on Tuesdays and Thursdays OR will you enforce Canadians to only be allowed to eat meat on Mondays, Thursdays, and Saturdays?";
                        option1 = "Outlaw Driving";
                        option2 = "Meat Eating Regulations";

                        SetDecisionText(decision, option1, option2);

                        //carbonChoiceReductionBest = 53500000f;
                        //carbonChoiceReductionGood = 36000000f;
                        carbonChoiceReductionBest = 50;
                        carbonChoiceReductionGood = 45;

                        resourceManager.energyNeed = 900000f; //Not sure if these are being used

                        resourceManager.startEnergyNeed = 590;
                    }
                    //EVENT
                    else if (currentYear == 2050)
                    {
                        //Phase out Natural Gas Heating (Oil and Coal)

                        popup = true;

                        eventText = "Natural Gas Heating is contributing too much to carbon emissions! These should be phased out of energy production.";

                        SetEventText(eventText);

                        resourceManager.energyNeed = 1200000f; //Not sure if these are being used

                        resourceManager.startEnergyNeed = 670;
                    }
                    break;
                case Season.FALL:
                    currentSeason = Season.FALL;
                    if (firstFall == false)
                    {
                        firstFall = true;
                        randomColdFall = 1f;
                        Debug.Log("FIRST FALL" + randomColdFall);
                        firstTemp = true;
                    }
                    else
                    {
                        randomColdFall = Random.Range(1f, 11f);
                    }
                    //DECISION
                    if (currentYear == 2020)
                    {
                        //Telling them that Energy Demand will now increase on season changesau
                        //popup = true;

                       // eventText = "When the seasons change from this point on, the Power Consumption will gradually increase. This may require you to turn on some of the plants you have previously turned off to meet the new Consumption. Plants are turned back On the same way they are turned Off.";

                        //SetEventText(eventText);
                        //CarbonReductionChoice(15500000f, 10000000f);

                        resourceManager.energyNeed = 750000f; //Not sure if these are being used

                        resourceManager.startEnergyNeed = 525;
                    }
                    //EVENT
                    else if (currentYear == 2035)
                    {
                        //Option to build a hydrogen plant to store excess energy and use for trains, busses, and trucking

                        popup = true;
                        eventPanel3.SetActive(true);
                        eventPanel3.transform.GetChild(1).gameObject.GetComponent<Text>().text = "You are now able to build Hydrogen Plants! These can be used to store excess energy that has been produced.";

                        GameObject[] buildAreas = new GameObject[10];

                        for (int i = 0; i < buildAreas.Length; i++)
                        {
                            buildAreas[i] = GameObject.Find("Build Area " + (i + 1));
                        }

                        foreach (GameObject gameObject in buildAreas)
                        {
                            gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.SetActive(false);
                        }

                        //SetEventText(eventText);

                        resourceManager.energyNeed = 950000f; //Not sure if these are being used

                        resourceManager.startEnergyNeed = 610;
                    }
                    //DECISION
                    else if (currentYear == 2050)
                    {
                        //Ban air travel worldwide (saving 747 million tons of CO2) OR shut down internet (1 billion tonnes)

                        popup = true;
                        FindObjectOfType<AudioManager>().Play("Alert");

                        decision = "You must make a decision to reduce carbon emissions. Either shut down the internet or ban air travel.";
                        option1 = "Shut Down Internet";
                        option2 = "Air Travel";

                        SetDecisionText(decision, option1, option2);

                        carbonChoiceReductionBest = 65;
                        carbonChoiceReductionGood = 60;

                        resourceManager.energyNeed = 1300000f; //Not sure if these are being used

                        resourceManager.startEnergyNeed = 690;


                    }
                    break;
                case Season.WINTER:
                    FindObjectOfType<AudioManager>().Play("Winter");
                    FindObjectOfType<AudioManager>().Stop("SummerSound");

                    if (firstWinter == false)
                    {
                        firstWinter = true;
                    }
                    currentSeason = Season.WINTER;
                    randomColdWinter = Random.Range(1f, 11f);
                    //NOTHING
                  
                    if (currentYear == 2020)
                    {
                        //Change every lightbulb in Canada to LED (15.5 million tons of C02 a year) or ban all air conditioning units (10 million tons of CO2)
                        //You must make a decision. Either change every lightbulb in Canada to LED or ban all air conditioning units.         

                        popup = true;
                        FindObjectOfType<AudioManager>().Play("Alert");
                        decision = "This is your First Policy. These are scenarios where you must pick one of two options. The choice you make will impact your Carbon Output so choose wisely. You must make a decision. Either change every lightbulb in Canada to LED or ban all air conditioning units.";
                        option1 = "LED Lights";
                        option2 = "Air Conditioners";

                        SetDecisionText(decision, option1, option2);

                        carbonChoiceReductionBest = 50;
                        carbonChoiceReductionGood = 45;

                        resourceManager.energyNeed = 800000f; //Not sure if these are being used

                        resourceManager.startEnergyNeed = 540;

                        currentYear = 2035;
                    }
                    //NOTHING
                    else if (currentYear == 2035)
                    {
                        //SHOULD REMOVE IF NOT NEEDED
                        
                        resourceManager.energyNeed = 1000000f; //Not sure if these are being used

                        currentYear = 2050;

                        resourceManager.startEnergyNeed = 630;
                    }
                    //NOTHING
                    else if (currentYear == 2050)
                    {
                        //SHOULD REMOVE IF NOT NEEDED
                        
                        resourceManager.energyNeed = 1400000f; //Not sure if these are being used

                        resourceManager.startEnergyNeed = 710;

                        isGameDoneSoon = true;
                    }
                    break;
            }
        }
    }

    public void ChangeWeather (Weather weatherType)
    {
        switch (weatherType)
        {
            case Weather.SUN:
                currentWeather = Weather.SUN;
                this.leaves.Stop();
                this.snow.Stop();
                this.summerLeaves.Stop();
                this.rain.Play();
                break;
            case Weather.XSUN:
                currentWeather = Weather.XSUN;
                this.leaves.Stop();
                this.snow.Stop();
                this.rain.Stop();
                this.summerLeaves.Play();
                break;
            case Weather.RAIN:
                //FALL
                currentWeather = Weather.RAIN;
                this.snow.Stop();
                this.summerLeaves.Stop();
                this.rain.Stop();
                this.leaves.Play();
                break;
            case Weather.SNOW:
                currentWeather = Weather.SNOW;
                this.leaves.Stop();
                this.summerLeaves.Stop();
                this.rain.Stop();
                this.snow.Play();
                break;
        }

    }
    private void Update()
    {
        //Stop weather time from flowing
        if (!popup && !ObjectPlacer.OnMenu)
            this.seasonTime -= Time.deltaTime;

        if(this.currentSeason == Season.SPRING)
        {
            ChangeWeather(Weather.SUN);

            //greater than 4 it will be a normal spring
            //less than 4 it will be a hot spring
            if (randomHeatSpring >= 4)
            {
                isColdWinter = false;
                frost.enabled = false;
                heat.enabled = false;
                isHotSpring = false;
            }
            else if (randomHeatSpring <=4)
            {
                isColdWinter = false;
                frost.enabled = false;
                heat.enabled = true;
                isHotSpring = true;


            }
            thePlane.GetComponent<MeshRenderer>().material = materialSpring;

            if(this.seasonTime <=0f)
            {
                ChangeSeason(Season.SUMMER);
                this.seasonTime = this.summerTime;
            }
        }
        if (this.currentSeason == Season.SUMMER)
        {
            firstSeason = true;
            ChangeWeather(Weather.XSUN);


            if (randomHeatSummer >= 4)
            {
                isHotSpring = false;
                heat.enabled = false;
                isHotSummer = false;
            }
            else if (randomHeatSummer <= 4)
            {
                isHotSpring = false;
                heat.enabled = true;
                isHotSummer = true;
                FindObjectOfType<AudioManager>().Stop("SummerSound");
                FindObjectOfType<AudioManager>().Play("GridFailure");
            }
            thePlane.GetComponent<MeshRenderer>().material = materialSummer;

            if (this.seasonTime <= 0f)
            {
                ChangeSeason(Season.FALL);
                this.seasonTime = this.fallTime;
            }
        }
        if (this.currentSeason == Season.FALL)
        {
            ChangeWeather(Weather.RAIN);

            if (randomColdFall >= 4)
            {
                isHotSummer = false;
                heat.enabled = false;
                frost.enabled = false;
                isColdFall = false;
            }
            else if (randomColdFall <=4)
            {
                isHotSummer = false;
                heat.enabled = false;
                frost.enabled = true;
                isColdFall = true;
            }
            thePlane.GetComponent<MeshRenderer>().material = materialFall;

            if (this.seasonTime <= 0f)
            {
                ChangeSeason(Season.WINTER);
                this.seasonTime = this.winterTime;
            }
        }
        if (this.currentSeason == Season.WINTER)
        {
            ChangeWeather(Weather.SNOW);

            if (randomColdWinter >= 4)
            {
                isColdFall = false;
                frost.enabled = false;
                isColdWinter = false;
            }
            else if (randomColdWinter <= 4)
            {
                isColdFall = false;
                frost.enabled = true;
                isColdWinter = true;
            }

            thePlane.GetComponent<MeshRenderer>().material = materialWinter;

            if (this.seasonTime <= 0f)
            {
                ChangeSeason(Season.SPRING);
                this.seasonTime = this.springTime;
            }
        }

        /* DEAD CODE FOR ICONS
        //adding the changing of icons, it matches along with the overlays 
        if(isHotSpring == true || isHotSummer == true)
        {
            normal_icon.enabled = false;
            hot_icon.enabled = true;
        }

        if (isColdFall == true || isColdWinter == true)
        {
            normal_icon.enabled = false;
            cold_icon.enabled = true;
        }

        else
        {
            normal_icon.enabled = true;
            cold_icon.enabled = false;
            hot_icon.enabled = false;
        }*/
    }
    
    private void LerpLightColor (Light light, Color c)
    {
        light.color = Color.Lerp(light.color, c, 0.2f * Time.deltaTime);
    }
}
