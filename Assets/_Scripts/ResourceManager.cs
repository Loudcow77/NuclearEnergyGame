using AwesomeCharts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    //TODO
    // Global money
    // When building is place, subtract monies
    // Start is called before the first frame update

    public Light theLight;
    public GameObject theLightOBJ;
    Color color1;
    public List<GameObject> buildingsList = new List<GameObject>();

    public static float totalMoney = 500;
    public float moneySpent;
    public float energyProduction;
    public float energyNeed; //This will be different every season and year
    public float globalEmissions;
    public float carbonReductions = 0; //This will be reduced per day - Maybe I will not use.
    public float globalTemperature = 1;    

    public Text resourceDisplay;
    public Text warning;
    public Text loser;

    private bool buildingsMade;
    private bool warningCounted = false; 

    public float timeleft;
    public float resetTimer;

    public SeasonManager seasonManager;
    public DayNight dayNight;

    public Image co2; //needs to be changed to a gauge, fill code has been commented out for now
    //public Image co2Constant;
    public Image energyBar;
    public Image energyNeedBar;
    public Image temperatureFill;
    //public Image energyPredictBar;
    public Image hydrogenBar;
    public Text energyNeedText;
    public Text energyProducedText;
    
    public Image warningImage;

    public GameObject water;
    private bool isRiseOne = false;
    private bool isRiseTwo = false;
    private bool isRiseThree = false;
    private bool isRiseFour = false;

    public float energyNeedDaily;
    public float energyNeedPrediction;
    public float startEnergyNeed = 510;
    float startTime;
    float hydrogenEfficiency = 0.45f;
    float totalHydrogenStorage;
    float totalHydrogenFill;
    public int warningCounter = 0;
    private bool warningFailure = false; 

    private float baseLine = 500;

    public GameObject dam;

    float tmpT = 0f;

    public string warningGrade;
    public string carbonGrade;
    public string moneyGrade;
    string eventText;
    int energyBarCounter;

    //[DebugGUIPrint, DebugGUIGraph(min: 0, max: 800, group: 4, r: 1, g: 0, b: 0)]
    float EnergyConsumingDaily;
    float EnergyConsumingPrediction;

    //[DebugGUIPrint, DebugGUIGraph(min: 0, max: 800, group: 4, r: 0, g: 1, b: 0)]
    float EnergyProducedDaily;
    bool isWarningPlaying = false;

    Queue<float> pastEnergyProducedDaily;
    Queue<float> pastWindEnergy;
    Queue<float> pastSolarEnergy;

    public GameObject powerGraph;
    public GameObject windGraph;
    public GameObject solarGraph;

    public GameObject dayIcon;
    public GameObject nightIcon;
    Queue<string> timePeriod;

    public bool canDestroy;

    private bool isFailing = false;

    void Awake()
    {
        pastEnergyProducedDaily = new Queue<float>();
        for (int i = 0; i < 5; i++)
        {
            pastEnergyProducedDaily.Enqueue(0);
        }
        pastWindEnergy = new Queue<float>();
        for (int i = 0; i < 4; i++)
        {
            pastWindEnergy.Enqueue(0);
        }
        pastSolarEnergy = new Queue<float>();
        for (int i = 0; i < 4; i++)
        {
            pastSolarEnergy.Enqueue(0);
        }
        timePeriod = new Queue<string>();
        for(int i = 0; i < 25; i++)
        {
            if (i == 4)
                timePeriod.Enqueue("6");
            else if (i == 9)
                timePeriod.Enqueue("12");
            else if (i == 14)
                timePeriod.Enqueue("18");
            else if (i == 19)
                timePeriod.Enqueue("24");
            else if (i == 24)
                timePeriod.Enqueue("6");
            else
                timePeriod.Enqueue("");
        }
    }

    void Start()
    {
        buildingsMade = false;
        canDestroy = false;
        //formating to replicate cost
        //totalMoney.ToString("#.00");
        resourceDisplay.text = "Total Money: $" + totalMoney;// + "     " + "Energy Produced: " + energyProduction + "     " + "Energy Consumed: " + EnergyConsumingDaily;// + "     " + "Global Emission: " + globalEmissions + "     " + "Energy Produced: " + energyProduction;

        buildingsMade = true;

        StartCoroutine(Flashing()); 
        StartCoroutine(CalculationsPerDay());

        totalMoney = 500;

        energyNeed = 700000;
        //energyProduction = 650000;
        globalEmissions = 10000;

        //co2Constant.fillAmount = baseLine / 1000; 

        totalMoney = 500;

        startTime = 0;
        timeleft = 60f;
        resetTimer = timeleft;

        buildingsList.Add(dam);

        totalHydrogenStorage = 0.0f;
        totalHydrogenFill = 0.0f;

        warningImage.enabled = false;
    }

    private void Update()
    {
        if (dayNight.nightTime)
        {
            dayIcon.SetActive(false);
            nightIcon.SetActive(true);
        }
        else
        {
            dayIcon.SetActive(true);
            nightIcon.SetActive(false);
        }


        //Make sure things dont change while popup is open
        if (!SeasonManager.popup && !ObjectPlacer.OnMenu)
        {
            startTime += Time.deltaTime;
            EnergyConsumingDaily = EnergyNeedDailyCalculation();
            EnergyConsumingPrediction = EnergyNeedPredictionCalculation();
        }

        //resourceDisplay.text = "COST: $" + totalMoney + " BIL.";// + "     " + "Global Emission: " + globalEmissions + "     " + "Energy Produced: " + energyProduction;

        resourceDisplay.text = "$" + totalMoney;// + "     " + "Energy Produced: " + energyProduction + "     " + "Energy Consumed: " + EnergyConsumingDaily;

        //UI TOP MENU COMPONENT
        energyBar.fillAmount = energyProduction / 1500;
        energyNeedBar.fillAmount = EnergyConsumingDaily / 1500;
        //energyPredictBar.fillAmount = energyNeedPrediction / 2500;
        hydrogenBar.fillAmount = totalHydrogenFill/ totalHydrogenStorage;
        co2.fillAmount = globalEmissions / 50000;
        //co2Constant.fillAmount = baseLine / 1000;

        int energyNeedForText = (int)EnergyConsumingDaily;
        int energyProducedForText = (int)energyProduction;

        energyNeedText.text = energyNeedForText.ToString();
        energyProducedText.text = energyProducedForText.ToString();

        //Temperature fill stuff
        //1.5 degrees
        if (globalEmissions >= 20000 && !isRiseOne)
        {
            temperatureFill.fillAmount = 0.52f;
            isRiseOne = true;
        }
        //2.4 degrees
        else if (globalEmissions >= 30000 && !isRiseTwo)
        {
            temperatureFill.fillAmount = 0.65f;
            water.transform.position = new Vector3 ( -70f, -0.65f, 38f);
            isRiseTwo = true;
        }
        //3.0 degrees
        else if (globalEmissions >= 35000 && !isRiseThree)
        {
            temperatureFill.fillAmount = 0.8f;
            isRiseThree = true;
        }
        //4.9 degrees
        else if (globalEmissions >= 50000 && !isRiseFour)
        {
            temperatureFill.fillAmount = 1.0f;
            isRiseFour = true;
        }

        //Disable destroy button if not enough money
        if (totalMoney < 700)
            BuildingResources.turnOnTheDestroy = false;
        else if (totalMoney >= 700 && seasonManager.currentYear >= 2035 && canDestroy)
            BuildingResources.turnOnTheDestroy = true;

        if (energyProduction < EnergyConsumingDaily - 1 && !SeasonManager.popup && !ObjectPlacer.OnMenu)
        {
            timeleft -= Time.deltaTime;
            //blinkingImage.enabled = true;
            theLightOBJ.SetActive(true);
            //warning.text = "Energy Need Not Met " + timeleft.ToString("F0") + "s " + "Until Grid Failure";
            warningImage.enabled = true;
            if (timeleft <= 0)
            {
                if (isFailing == false)
                {
                    theLight.enabled = true;
                    isFailing = true;
                }
                timeleft = 0;
                //add lose pop here
                globalEmissions = globalEmissions + 5000f;
                totalMoney = totalMoney - 200f;
                moneySpent = moneySpent + 200f;
                timeleft = resetTimer;

                warningFailure = true;

                //string loseText = "You did not produce enough energy! You have lost the game!";

                SeasonManager.popup = true;
                eventText = "Your Grid Failed. The City Had to Use Coal Factories To Make Up For The Failure. Your CO2 Emissions Increase As a Result and You Lost some Money.";
                seasonManager.SetEventText(eventText);

                //loser.enabled = true;
                //Debug.Log("You Lost");
            }
        }
        if (energyProduction >= EnergyConsumingDaily)
        {
            //   FindObjectOfType<AudioManager>().Stop("GridFailure");

            theLightOBJ.SetActive(false);
            warningImage.enabled = false;
            timeleft = resetTimer;
            warning.text = "";

            //  FindObjectOfType<AudioManage>().Stop("GridFailure");

            if (energyBarCounter < 1)
            {
                //seasonManager.popup = true;
                //eventText = "You have met the Power Consumption, this is shown at the top. The bottom bar represents Power Production and the top bar represents Power Consumption. When you are producing more energy than required, you can turn off your power plants to reduce Carbon Emissions. You can do this by Clicking on the Power Plants themselves. A red outline means it is turned Off.";
                //seasonManager.SetEventText(eventText);
                energyBarCounter++;
            }


        }
        //Grid failure sound
        if (energyProduction < EnergyConsumingDaily && isWarningPlaying == false && !SeasonManager.popup)
        {
            isWarningPlaying = true;
            FindObjectOfType<AudioManager>().Play("GridFailure");

            if (warningCounted == false)
            {
                warningCounter++;
                warningCounted = true;
            }
        }

        else if (energyProduction >= EnergyConsumingDaily && isWarningPlaying == true && !SeasonManager.popup)
        {
            isWarningPlaying = false;
            FindObjectOfType<AudioManager>().Stop("GridFailure");

            if (warningCounted == true)
            {
                warningCounted = false;
            }

        }
    

        if(warningCounter == 0 || warningCounter == 1)
        {
            warningGrade = "A";
        }

        if (warningCounter == 2)
        {
            warningGrade = "B";
        }

        if (warningCounter == 3)
        {
            warningGrade = "C";
        }

        if (warningCounter >= 4)
        {
            warningGrade = "D";
        }

        else if(warningFailure == true)
        {
            warningGrade = "F";
        }

        //Carbon Grade
        if (globalEmissions <= 0)
        {
            carbonGrade = "A";
        }
        else if (globalEmissions >= 0 && globalEmissions <= 6250)
        {
            carbonGrade = "B";
        }
        else if (globalEmissions >= 6251 && globalEmissions <= 12500)
        {
            carbonGrade = "C";
        }
        else if (globalEmissions >= 12501 && globalEmissions <= 18750)
        {
            carbonGrade = "D";
        }
        else if (globalEmissions >= 18751)
        {
            carbonGrade = "F";
        }
        //Money Grade
        if (moneySpent <= 10000)
        {
            moneyGrade = "A";
        }
        else if (moneySpent >= 10001 && moneySpent <= 12000)
        {
            moneyGrade = "B";
        }
        else if (moneySpent >= 12001 && moneySpent <= 13000)
        {
            moneyGrade = "C";
        }
        else if (moneySpent >= 13001 && moneySpent <= 15000)
        {
            moneyGrade = "D";
        }
        else if (moneySpent >= 15001)
        {
            moneyGrade = "F";
        }
    }

public void BuildingCalculations(GameObject building)
    {
        totalMoney -= building.GetComponent<BuildingResources>().costToBuild;

        moneySpent += building.GetComponent<BuildingResources>().costToBuild;
        //buildingsMade = true;

        //StartCoroutine(CalculationsPerDay());
    }

    float EnergyNeedDailyCalculation()
    {
        //print(startTime);
        //Make energy need higher when these are true
        if (seasonManager.isHotSpring || seasonManager.isHotSummer || seasonManager.isColdFall || seasonManager.isColdWinter)
            energyNeedDaily = (float)(startTime * 0.28f + Mathf.Cos(startTime * 0.21f - 4.24f) * 1.5f + startEnergyNeed);
        else
            energyNeedDaily = (float)(startTime * 0.2f + Mathf.Cos(startTime * 0.21f - 3.38f) * 8f + startEnergyNeed);
        Debug.Log("Start Time: " + startTime + " Energy Need Daily: " + energyNeedDaily + " Start Energy Need: " + startEnergyNeed
            + "\nHot Spring: " + seasonManager.isHotSpring + "Hot Summer: " + seasonManager.isHotSummer + "Cold Fall: " + seasonManager.isColdFall + "Cold Winter: " + seasonManager.isColdWinter);
        return energyNeedDaily;
    }

    float EnergyNeedPredictionCalculation()
    {
        //print(startTime);
        //Make energy need higher when these are true
        if (seasonManager.isHotSpring || seasonManager.isHotSummer || seasonManager.isColdFall || seasonManager.isColdWinter)
            energyNeedPrediction = (float)(startTime + 30 * 0.28f + Mathf.Cos(startTime * 0.21f - 4.24f) * 1.5f + startEnergyNeed);
        else
            energyNeedPrediction = (float)(startTime + 30 * 0.2f + Mathf.Cos(startTime * 0.21f - 3.38f) * 8f + startEnergyNeed);
        //Debug.Log(energyNeedDaily);
        return energyNeedPrediction;
    }

    float EnergyNeedGraphPredictionDailyCalculation(float time)
    {
        //print(startTime);
        //Make energy need higher when these are true
        if (seasonManager.isHotSpring || seasonManager.isHotSummer || seasonManager.isColdFall || seasonManager.isColdWinter)
            energyNeedDaily = (float)(time * 0.28f + Mathf.Cos(startTime * 0.21f - 4.24f) * 1.5f + startEnergyNeed);
        else
            energyNeedDaily = (float)(time * 0.2f + Mathf.Cos(startTime * 0.21f - 3.38f) * 8f + startEnergyNeed);
        //Debug.Log(energyNeedDaily);
        return energyNeedDaily;
    }


    private IEnumerator CalculationsPerDay()
    {
        while (buildingsMade)
        {
            EnergyConsumingDaily = EnergyNeedDailyCalculation();
            float totalEnerggGenerate = 0.0f;
            float totalEmissionsGenerate = 0.0f;
            if (!SeasonManager.popup && !ObjectPlacer.OnMenu)
                energyProduction = 0;
            foreach (GameObject building in buildingsList)
            {
                if (building.GetComponent<BuildingResources>().isActive && !SeasonManager.popup && !ObjectPlacer.OnMenu)
                {
                    //totalMoney -= building.GetComponent<BuildingResources>().costPerDay;
                    totalMoney -= building.GetComponent<BuildingResources>().costToProduceEnergy;
                    totalMoney += building.GetComponent<BuildingResources>().energyIncome;
                    globalEmissions += building.GetComponent<BuildingResources>().emissionsPerDay;
                    //Update gauge
                    
                    //globalEmissions -= carbonReductions; //They may be done in lump sums rather than per day - lets do per day

                    //Varying solar productions by season. Energy production is reduced by 1 at night. Depending on weather fluctuations, it changes enetgy production too
                    if (building.tag == "Solar")
                    {
                        if (seasonManager.currentSeason == SeasonManager.Season.SPRING)
                        {
                            building.GetComponent<BuildingResources>().energyProductionPerDay = 55;

                            if (dayNight.nightTime)
                                building.GetComponent<BuildingResources>().energyProductionPerDay = 0;

                            if (seasonManager.isHotSpring && !dayNight.nightTime)
                                building.GetComponent<BuildingResources>().energyProductionPerDay += 5;
                        }
                        else if (seasonManager.currentSeason == SeasonManager.Season.SUMMER)
                        {
                            building.GetComponent<BuildingResources>().energyProductionPerDay = 65;
                            
                            if (dayNight.nightTime)
                                building.GetComponent<BuildingResources>().energyProductionPerDay = 0;

                            if (seasonManager.isHotSummer && !dayNight.nightTime)
                                building.GetComponent<BuildingResources>().energyProductionPerDay += 5;
                        }
                        else if (seasonManager.currentSeason == SeasonManager.Season.FALL)
                        {
                            building.GetComponent<BuildingResources>().energyProductionPerDay = 45;
                            
                            if (dayNight.nightTime)
                                building.GetComponent<BuildingResources>().energyProductionPerDay = 0;
                        }
                        else if (seasonManager.currentSeason == SeasonManager.Season.WINTER)
                        {
                            building.GetComponent<BuildingResources>().energyProductionPerDay = 35;

                            if (dayNight.nightTime)
                                building.GetComponent<BuildingResources>().energyProductionPerDay = 0;
                        }
                    }

                    //Varying wind production by season
                    if (building.tag == "Wind")
                    {
                        if (seasonManager.currentSeason == SeasonManager.Season.SPRING)
                        {
                            building.GetComponent<BuildingResources>().energyProductionPerDay = 45;
                        }
                        else if (seasonManager.currentSeason == SeasonManager.Season.SUMMER)
                        {
                            building.GetComponent<BuildingResources>().energyProductionPerDay = 35;

                        }
                        else if (seasonManager.currentSeason == SeasonManager.Season.FALL)
                        {
                            building.GetComponent<BuildingResources>().energyProductionPerDay = 50;

                            if (seasonManager.isColdFall)
                                building.GetComponent<BuildingResources>().energyProductionPerDay += 5;

                        }
                        else if (seasonManager.currentSeason == SeasonManager.Season.WINTER)
                        {
                            building.GetComponent<BuildingResources>().energyProductionPerDay = 55;

                            if (seasonManager.isColdWinter)
                                building.GetComponent<BuildingResources>().energyProductionPerDay += 5;
                        }
                    }

                    energyProduction += building.GetComponent<BuildingResources>().energyProductionPerDay;
                    totalEnerggGenerate += building.GetComponent<BuildingResources>().energyProductionPerDay;

                    totalEmissionsGenerate += building.GetComponent<BuildingResources>().emissionsPerDay;                   
                    //totalEmissionsGenerate -= carbonReductions;

                    if (building.GetComponent<BuildingResources>().powerType == BuildingResources.buidingType.hydrogen)
                    {
                        if (building.GetComponent<BuildingResources>().hydrogenStorage - building.GetComponent<BuildingResources>().energyProductionPerDay <= 0)
                        {
                            building.GetComponent<BuildingResources>().hydrogenStorage = 0;
                            building.GetComponent<BuildingResources>().turnBuidingOnOrOff();
                        }
                        else
                            building.GetComponent<BuildingResources>().hydrogenStorage -= building.GetComponent<BuildingResources>().energyProductionPerDay;
                    }
                }
            }

            //carbonReductions = 5;
            if (!SeasonManager.popup && !ObjectPlacer.OnMenu)
                globalEmissions -= carbonReductions;

            //Debug.Log(globalEmissions);

            //Subtract energy consumed from total energy produced
            /*if (!seasonManager.popup)
                energyProduction -= EnergyConsumingDaily;*/
            EnergyProducedDaily = totalEnerggGenerate;
            if (!ObjectPlacer.OnMenu && !SeasonManager.popup)
            {
                pastEnergyProducedDaily.Dequeue();
                pastEnergyProducedDaily.Enqueue(energyProduction);
                UpdatePowerGraph();
                UpdateWindGraph();
                UpdateSolarGraph();
                UpdateTimePeriod();
            }

            totalEmissionsGenerate -= carbonReductions;
            //Debug.Log(totalEmissionsGenerate);
            if (totalEmissionsGenerate >= 150)
                baseLine = 1000;
            else if (totalEmissionsGenerate > 100)
                baseLine = 900;
            else if (totalEmissionsGenerate > 80)
                baseLine = 800;
            else if (totalEmissionsGenerate > 55)
                baseLine = 750;
            else if (totalEmissionsGenerate < 40)
                baseLine = 650;
            else if (totalEmissionsGenerate < 25)
                baseLine = 600;
            else if (totalEmissionsGenerate < 10)
                baseLine = 550;

            else if (totalEmissionsGenerate == 0)
                baseLine = 500;

            else if (totalEmissionsGenerate < 0)
                baseLine = 450;
            else if (totalEmissionsGenerate < -50)
                baseLine = 300;
            else if (totalEmissionsGenerate < -150)
                baseLine = 200;
            else if (totalEmissionsGenerate < -250)
                baseLine = 50;
            else if (totalEmissionsGenerate < -300)
                baseLine = 0;

            if (globalEmissions > 50000)
                globalEmissions = 50000;
            else if (globalEmissions < 0)
                globalEmissions = 0;
            //print(totalEnerggGenerate);
            if (totalEnerggGenerate > energyNeedDaily)
            {
                totalEnerggGenerate -= energyNeedDaily;

                foreach (GameObject building in buildingsList)
                {
                    if (building.GetComponent<BuildingResources>().powerType == BuildingResources.buidingType.hydrogen)
                    {
                        if (building.GetComponent<BuildingResources>().isActive == false)
                        {
                            if (totalEnerggGenerate * hydrogenEfficiency > building.GetComponent<BuildingResources>().maxHydrogenStorage - building.GetComponent<BuildingResources>().hydrogenStorage)
                            {
                                totalEnerggGenerate -= building.GetComponent<BuildingResources>().maxHydrogenStorage - building.GetComponent<BuildingResources>().hydrogenStorage / hydrogenEfficiency;
                                building.GetComponent<BuildingResources>().hydrogenStorage = building.GetComponent<BuildingResources>().maxHydrogenStorage;
                            }
                            else
                            {
                                building.GetComponent<BuildingResources>().hydrogenStorage += totalEnerggGenerate * hydrogenEfficiency;
                                break;
                            }
                        }
                    }
                }
            }
            totalHydrogenStorage = 0;
            totalHydrogenFill = 0;
            foreach (GameObject building in buildingsList)
            {
                if (building.GetComponent<BuildingResources>().powerType == BuildingResources.buidingType.hydrogen)
                {
                    totalHydrogenStorage += building.GetComponent<BuildingResources>().maxHydrogenStorage;
                    totalHydrogenFill += building.GetComponent<BuildingResources>().hydrogenStorage;
                }
            }
            yield return new WaitForSeconds(1.5f);
        }
    }

    void UpdatePowerGraph()
    {
        foreach(LineDataSet dataSet in powerGraph.GetComponent<LineChart>().data.dataSets)
        {
            if (dataSet.title == "CONSUMED")
            {
                foreach (LineEntry lineEntry in dataSet.entries)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        if (lineEntry.Position == i)
                        {
                            float t = startTime + (i - 4) * 1.5f;
                            if (t < 0)
                            {
                                lineEntry.Value = 0;
                            }
                            else
                            {
                                lineEntry.Value = EnergyNeedGraphPredictionDailyCalculation(t);
                            }
                        }
                    }
                }
            }
            else if (dataSet.title == "PRODUCED")
            {
                float[] tmp = pastEnergyProducedDaily.ToArray();
                foreach (LineEntry lineEntry in dataSet.entries)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (lineEntry.Position == i)
                        {
                            lineEntry.Value = tmp[i];
                        }
                    }
                }
            }
        }
        powerGraph.GetComponent<LineChart>().SetDirty();
    }
    void UpdateWindGraph()
    {
        foreach (LineDataSet dataSet in windGraph.GetComponent<LineChart>().data.dataSets)
        {
            float[] tmp = pastWindEnergy.ToArray();
            foreach (LineEntry lineEntry in dataSet.entries)
            {
                for (int i = 0; i < 25; i++)
                {
                    if (lineEntry.Position == i)
                    {
                        if (i >= 0 && i < 4)
                        {
                            lineEntry.Value = tmp[i];
                        }
                        else
                        {
                            lineEntry.Value = WindCalculation((seasonManager.springTime - seasonManager.seasonTime) + (i - 4) * 1.5f, seasonManager.springTime, seasonManager.currentSeason);
                            if (i == 4)
                            {
                                pastWindEnergy.Dequeue();
                                pastWindEnergy.Enqueue(lineEntry.Value);
                            }
                        }
                    }
                }
            }
        }
        windGraph.GetComponent<LineChart>().SetDirty();
    }

    float WindCalculation(float t, float seansonTotalTime, SeasonManager.Season thisSeanson)
    {
        float theValue = 0;
        float currentSeansonObjective = 0;
        float nextSeansonObjective = 0;
        float lastSeasonObjective = 0;
        SeasonManager.Season nextSeanson = new SeasonManager.Season();

        if (thisSeanson == SeasonManager.Season.SPRING)
        {
            currentSeansonObjective = 20;
            nextSeansonObjective = 10;
            lastSeasonObjective = 30;
            nextSeanson = SeasonManager.Season.SUMMER;
        }
        else if (thisSeanson == SeasonManager.Season.SUMMER)
        {
            currentSeansonObjective = 10;
            nextSeansonObjective = 25;
            lastSeasonObjective = 20;
            nextSeanson = SeasonManager.Season.FALL;
        }
        else if (thisSeanson == SeasonManager.Season.FALL)
        {
            currentSeansonObjective = 25;
            nextSeansonObjective = 30;
            lastSeasonObjective = 10;
            if (seasonManager.isColdFall)
                currentSeansonObjective += 5;
            nextSeanson = SeasonManager.Season.WINTER;
        }
        else if (thisSeanson == SeasonManager.Season.WINTER)
        {
            currentSeansonObjective = 30;
            nextSeansonObjective = 20;
            lastSeasonObjective = 25;
            if (seasonManager.isColdWinter)
                currentSeansonObjective += 5;
            nextSeanson = SeasonManager.Season.SPRING;
        }

        if (t > seansonTotalTime)
        {
            theValue = WindCalculation(t - seansonTotalTime, seansonTotalTime, nextSeanson);
        }
        else
        {
            if (t == seansonTotalTime / 2)
            {
                theValue = currentSeansonObjective;
            }
            else if (t > seansonTotalTime / 2)
            {
                theValue = currentSeansonObjective - ((t - seansonTotalTime / 2) * ((currentSeansonObjective - nextSeansonObjective) / 2 / (seansonTotalTime / 2)));
            }
            else
            {
                theValue = currentSeansonObjective + ((seansonTotalTime / 2 - t) * ((lastSeasonObjective - currentSeansonObjective) / 2 / (seansonTotalTime / 2)));
            }
        }

        return theValue;
    }


    void UpdateSolarGraph()
    {
        foreach (LineDataSet dataSet in solarGraph.GetComponent<LineChart>().data.dataSets)
        {
            float[] tmp = pastSolarEnergy.ToArray();
            foreach (LineEntry lineEntry in dataSet.entries)
            {
                for (int i = 0; i < 25; i++)
                {
                    if (lineEntry.Position == i)
                    {
                        if (i >= 0 && i < 4)
                        {
                            lineEntry.Value = tmp[i];
                        }
                        else
                        {
                            lineEntry.Value = Solaralculation((seasonManager.springTime - seasonManager.seasonTime) + (i - 4) * 1.5f, seasonManager.springTime, seasonManager.currentSeason);
                            if (i == 4)
                            {
                                pastSolarEnergy.Dequeue();
                                pastSolarEnergy.Enqueue(lineEntry.Value);
                            }
                        }
                    }
                }
            }
        }
        solarGraph.GetComponent<LineChart>().SetDirty();
    }

    float Solaralculation(float t, float seansonTotalTime, SeasonManager.Season thisSeanson)
    {
        float theValue = 0;
        float currentSeansonObjective = 0;
        float nextSeansonObjective = 0;
        float lastSeasonObjective = 0;
        SeasonManager.Season nextSeanson = new SeasonManager.Season();

        if (thisSeanson == SeasonManager.Season.SPRING)
        {
            currentSeansonObjective = 30;
            nextSeansonObjective = 40;
            lastSeasonObjective = 10;
            if (seasonManager.isHotSpring)
                currentSeansonObjective += 5;
            if (dayNight.nightTime)
                currentSeansonObjective = 0;
            nextSeanson = SeasonManager.Season.SUMMER;
        }
        else if (thisSeanson == SeasonManager.Season.SUMMER)
        {
            currentSeansonObjective = 40;
            nextSeansonObjective = 20;
            lastSeasonObjective = 30;
            if (seasonManager.isHotSummer)
                currentSeansonObjective += 5;
            if (dayNight.nightTime)
                currentSeansonObjective = 0;
            nextSeanson = SeasonManager.Season.FALL;
        }
        else if (thisSeanson == SeasonManager.Season.FALL)
        {
            currentSeansonObjective = 20;
            nextSeansonObjective = 10;
            lastSeasonObjective = 40;
            if (dayNight.nightTime)
                currentSeansonObjective = 0;
            nextSeanson = SeasonManager.Season.WINTER;
        }
        else if (thisSeanson == SeasonManager.Season.WINTER)
        {
            currentSeansonObjective = 10;
            nextSeansonObjective = 30;
            lastSeasonObjective = 20;
            if (dayNight.nightTime)
                currentSeansonObjective = 0;
            nextSeanson = SeasonManager.Season.SPRING;
        }

        if (t > seansonTotalTime)
        {
            theValue = WindCalculation(t - seansonTotalTime, seansonTotalTime, nextSeanson);
        }
        else
        {
            if (t == seansonTotalTime / 2)
            {
                theValue = currentSeansonObjective;
            }
            else if (t > seansonTotalTime / 2)
            {
                theValue = currentSeansonObjective - ((t - seansonTotalTime / 2) * ((currentSeansonObjective - nextSeansonObjective) / 2 / (seansonTotalTime / 2)));
            }
            else
            {
                theValue = currentSeansonObjective + ((seansonTotalTime / 2 - t) * ((lastSeasonObjective - currentSeansonObjective) / 2 / (seansonTotalTime / 2)));
            }
        }

        return theValue;
    }

    void UpdateTimePeriod()
    {
        string tmp;
        tmp = timePeriod.Dequeue();
        timePeriod.Enqueue(tmp);

        powerGraph.GetComponent<LineChart>().axisConfig.horizontalAxisConfig.valueFormatterConfig.customValues = timePeriod.ToList();
    }


    private IEnumerator Flashing()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            theLight.enabled =!theLight.enabled;
        }
    }
}
 