using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorage : MonoBehaviour
{
    // Start is called before the first frame update
    public ResourceManager resourceManager;
    public string carbGrade;
    public string cashGrade;
    public string warningGrade;

    // Update is called once per frame
    void Update()
    {
        carbGrade = resourceManager.carbonGrade;
        cashGrade = resourceManager.moneyGrade;
        warningGrade = resourceManager.warningGrade;
    }
}
