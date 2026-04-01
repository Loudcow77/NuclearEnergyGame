using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public GameObject Tutpanel;
    public GameObject Tutpane2;
    public GameObject Tutpane3;
    public GameObject Tutpane4;
    public GameObject Tutpane5;
    public GameObject Tutpane6;
    public GameObject Tutpane7;
    public GameObject Tutpane8;
    public GameObject arrow;
    public GameObject arrow2;
    public GameObject arrow3;
    public GameObject arrow4;
    public GameObject arrow5;

    public GameObject buildTut;

    public GameObject moneyTut;
    public GameObject powerConsTut;
    public GameObject onOffTut;

    public GameObject build1;
    //public GameObject onOffMenuTut;
    //public GameObject arrow4;
    public static bool firstNight= false;
    public static bool isFirstBuild= false;
    public static bool firstOnOff= false;
    public static bool firstOnOffMenu = false;
    public static bool isFirstDestroy = false;

    private void Start()
    {
        TutEvent1();
        //Tutpane2 = GameObject.Find("Tutorial Panel 2");
        Tutpane2.gameObject.SetActive(false);
        Tutpane3.gameObject.SetActive(false);
    }

    public void TutEvent1()
    {
        SeasonManager.popup = true;
        Tutpanel.SetActive(true);
        Tutpanel.transform.GetChild(1).gameObject.GetComponent<Text>().text = "WELCOME TO NET ZERO!";
    }
    public void TutEvent2()
    {
        SeasonManager.popup = true;
        Tutpanel.SetActive(true);
        Tutpanel.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Your goal is to supply power to Canada and reduce our Carbon Footprint!";
    }
    public void TutEvent3()
    {
        SeasonManager.popup = true;
        Tutpanel.SetActive(true);
        Tutpanel.transform.GetChild(1).gameObject.GetComponent<Text>().text = "To start, build a Power Plant to begin supplying power!";
    }

    public void TutEvent3Disable()
    {
        build1.GetComponentInChildren<ObjectPlacer>().enabled = false;
    }
    public void TutEvent4()
    {
        SeasonManager.popup = true;
        Tutpane2.SetActive(true);
        powerConsTut.SetActive(true);
        Tutpane2.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Be sure to keep your Power Production above your Power Consumption, to avoid a grid failure!";

    }
  
    public void TutEvent6()
    {
        ResourceManager.totalMoney += 700;

        SeasonManager.popup = true;
        Tutpane7.SetActive(true);
        Tutpane7.transform.GetChild(1).gameObject.GetComponent<Text>().text = "You now have the ability to DESTROY Power Plants and BUILD new ones! This will cost $700";
    }
    public void TutEvent7()
    {
        SeasonManager.popup = true;
        Tutpane2.SetActive(true);
        Tutpane2.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Use green energy alternatives to reduce your carbon emissions. Try to get your carbon emissions to 0 by 2050.";
    }
    public void TutEvent8()
    {
        SeasonManager.popup = true;
        Tutpane8.transform.GetChild(1).gameObject.GetComponent<Text>().text = "This is your total Hydrogen storage ";
        Tutpane8.SetActive(true);

    }
    public void TutEvent9()
    {
        SeasonManager.popup = true;
        moneyTut.gameObject.SetActive(true);
        //buildTut.gameObject.SetActive(false);
        Tutpane2.gameObject.SetActive(true);
        Tutpane2.transform.GetChild(1).gameObject.GetComponent<Text>().text = "As you collect more cash from powering residents, you can build more powerful and eco friendly power plants!";
        
    }

    public void TutEvent10()
    {
        
    }
    public void Update()
    {
        if (ObjectPlacer.firstBuild == true && isFirstBuild == false)
        {
            Debug.Log("TEST");
            buildTut.gameObject.SetActive(false);
            isFirstBuild = true;
        }
        if(BuildingResources.firstDestroy == true && isFirstDestroy == false)
        {
            buildTut.SetActive(false);
            SeasonManager.popup = false;

        }
        if(BuildingResources.firstOnOffClick == true && firstOnOffMenu == false)
        {
            firstOnOffMenu = true;
            //onOffTut.gameObject.SetActive(false);
            buildTut.gameObject.SetActive(false);
        }

       //
        if (BuildingResources.firstOnOff == true && firstOnOff == false)
        {
            Debug.Log("ON OFF");
            firstOnOff = true;
            onOffTut.SetActive(false);
            //onOffMenuTut.gameObject.SetActive(false);
        }
        if (DayNight.isNight == true && firstNight == false)
        {
            firstNight = true; 
            SeasonManager.popup = true;
            Tutpane5.SetActive(true);
            Tutpane5.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Turn Power Plants ON and OFF to balance production with energey need";
        }
        //if (SeasonManager.firstSeason == true && isFirstSeason == false)
        //{
        //    SeasonManager.popup = true;
        //    Tutpane3.gameObject.SetActive(true);
        //    Tutpane3.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Seasons will also affect power consumption and power production!";
        //    isFirstSeason = true;
        //}
        //if (SeasonManager.firstTemp == true && isFirstTemp == false)
        //{
        //    SeasonManager.popup = true;
        //    arrow4.gameObject.SetActive(true);
        //    Tutpane6.gameObject.SetActive(true);
        //    Tutpane6.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Weather spikes will also affect your power consumption and production!";
        //    isFirstTemp = true;
        //}

        //if (ObjectPlacer.isDoneBuilding == true && doneBuild == false)
        //{
        //    //arrow4.gameObject.SetActive(true);
        //    doneBuild = true;
        //    TutEvent5();
        //}

       //if(SeasonManager.destroySeason == true && DayNight.isNight == true && is2035 == false)
       //{
       //    StartCoroutine(Destroy());
       //    is2035 = true;
       //
       //    if (is2035==true)
       //    {
       //        Debug.Log("IS2035");
       //        stopCo = true;
       //    }
       //}
    }

    //public IEnumerator Destroy()
    //{
    //    while (stopCo == false)
    //    {
    //        Debug.Log("HI");
    //        is2035 = true;
    //        seasonManager.popup = true;
    //        Tutpane3.gameObject.SetActive(true);
    //        Tutpane3.transform.GetChild(1).gameObject.GetComponent<Text>().text = "You now have the ability to DESTROY Power Plants and BUILD new ones!\n(CLICK ON BUILDING - NEED TO ADD)";
    //        yield return new WaitForSeconds(5f);
    //    }
    //}
}
