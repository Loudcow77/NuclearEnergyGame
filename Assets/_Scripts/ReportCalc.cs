using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportCalc : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject dataStorage;
    public Text CarbonGrading; 
    public Text MoneyGrading;
    public Text WarningGrading;
    public Text finalGrading;
    
    public Text CarbonResponse;
    public Text MoneyResponse;
    public Text WarningResponse;
    public Text FinalResponse;


    private int carbonScore = 0;
    private int moneyScore = 0;
    private int warningScore = 0;
    private float finalScore = 0f;
    void Start()
    {
        dataStorage = GameObject.FindGameObjectWithTag("SaveMe");
        
        CarbonGrading.text = dataStorage.GetComponent<DataStorage>().carbGrade;
        MoneyGrading.text = dataStorage.GetComponent<DataStorage>().cashGrade;
        WarningGrading.text = dataStorage.GetComponent<DataStorage>().warningGrade;
    }

    // Update is called once per frame
    void Update()
    {
        //carbon calculations
        if (CarbonGrading.text == "A")
        {
            carbonScore = 100;
            CarbonResponse.text = "";
        }

        if (CarbonGrading.text == "B")
        {
            carbonScore = 80;
            CarbonResponse.text = "";
        }

        if (CarbonGrading.text == "C")
        {
            carbonScore = 60 ;
            CarbonResponse.text = "";
        }

        if (CarbonGrading.text == "D")
        {
            carbonScore = 40;
            CarbonResponse.text = "";
        }

        if (CarbonGrading.text == "F")
        {
            carbonScore = 20;
            CarbonResponse.text = "";
        }

        // money calculations 
        if (MoneyGrading.text == "A")
        {
            moneyScore = 100;
            MoneyResponse.text = "Your money has been nicely managed! Plants were placed accordingly and the impact of cost was taken into consideration";
        }

        if (MoneyGrading.text == "B")
        {
            moneyScore = 80;
            MoneyResponse.text = "";
        }

        if (MoneyGrading.text == "C")
        {
            moneyScore = 60;
            MoneyResponse.text = "";
        }

        if (MoneyGrading.text == "D")
        {
            moneyScore = 40;
            MoneyResponse.text = "";
        }

        if (MoneyGrading.text == "F")
        {
            moneyScore = 20;
            MoneyResponse.text = "";
        }

        //warning grading
        if (WarningGrading.text == "A")
        {
            warningScore = 100;
            WarningResponse.text = "You were able to consistently keep up with the amount of energy being consumed. The population was happy, power never went out!";
        }

        if (WarningGrading.text == "B")
        {
            warningScore = 80;
            WarningResponse.text = "";
        }

        if (WarningGrading.text == "C")
        {
            warningScore = 60;
            WarningResponse.text = "";
        }

        if (WarningGrading.text == "D")
        {
            warningScore = 40;
            WarningResponse.text = "";
        }

        if (WarningGrading.text == "F")
        {
            warningScore = 20;
            WarningResponse.text = "A large penalty is received since you were  unable to produce enough energy to sustain the amount of energy you were consumed throughout the game. ";
        }

        //final grade
        if(calculateFinalGrade() >= 90 || calculateFinalGrade() <= 100)
        {
            finalGrading.text = "A";
            FinalResponse.text = "";
        }

        if (calculateFinalGrade() >= 80 || calculateFinalGrade() <= 89)
        {
            finalGrading.text = "B";
            FinalResponse.text = "";
        }

        if (calculateFinalGrade() >= 70 || calculateFinalGrade() <= 79)
        {
            finalGrading.text = "C";
            FinalResponse.text = "";
        }

        if (calculateFinalGrade() >= 60 || calculateFinalGrade() <= 69)
        {
            finalGrading.text = "D";
            FinalResponse.text = "";
        }

        if (calculateFinalGrade() <= 59)
        {
            finalGrading.text = "F";
            FinalResponse.text = "";
        }
    }

    private float calculateFinalGrade()
    {
        finalScore = carbonScore + moneyScore + warningScore / 3;
        return finalScore;
    }
}
