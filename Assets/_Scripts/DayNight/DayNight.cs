using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    [Header("Time")]
    [Tooltip("Day Length in Minutes")]

    [SerializeField]
    private float _targetDayLength = 1f; //length of day in minutes
    public float targetDayLength
    {
        get
        {
            return _targetDayLength;
        }
    }

    [SerializeField]
    [RangeAttribute(0f, 1f)] //time of day, fraction of the day thats passed
    private float _timeOfDay;
    public float timeofDay
    {
        get
        {
            return _timeOfDay;
        }

    }

    [SerializeField]
    private int _dayNumber = 0; //tracks the days passed in the year
    public int dayNumber
    {
        get
        {
            return _dayNumber;
        }
    }

    [SerializeField]
    private int _yearNumber = 0; //number of years passed
    public int yearNumber
    {
        get
        {
            return _yearNumber;
        }
    }

    private float _timeScale = 100f; //internal varaible, conversion bw real time (24hrs) an
    [SerializeField]
    private int _yearLength = 100; //number of days in the year
    public float yearLength
    {
        get
        {
            return _yearLength;
        }
    }

    public bool pause = false; //pause day night cycle

    [SerializeField]
    private AnimationCurve timeCurve;
    private float timeCurveNoramlization;

    [Header("Sun Light")]
    [SerializeField]
    private Transform dailyRotation;
    [SerializeField]
    private Light sun;
    private float intensity;
    [SerializeField]
    private float sunBaseIntensity = 1.5f; //intensity at sun rise and sun set
    [SerializeField]
    private float sunVariation =0.5f;
    [SerializeField]
    private Gradient sunColor;

    [Header("Emmissive Materials")]
    public Material building1;
    public Material building2;
    public Material building3;
    public Material building4;
    public Material building5;
    public Material house1;
    public Material house2;
    public Material grid;
    public Material farm;
    public Light light1;
    public Light light2;
    public Light light3;

    public bool nightTime = false;

    public static bool isNight = false;
    public static bool nightDestroy = false;

    private void Update()
    {
        //Stop time from flowing when a popup is there
        if (SeasonManager.popup || ObjectPlacer.OnMenu)
            pause = true;
        else if (!SeasonManager.popup || !ObjectPlacer.OnMenu)
            pause = false;

        if (!pause)
        {
            UpdateTimeScale();
            UpdateTime();
        }
        //Debug.Log(dayNumber);
        AdjustRotation();
        //SunIntenity();
        if (timeofDay >= 0.6f && timeofDay <= 0.7f)
        {
            nightDestroy = true;
        }
        if (timeofDay < 1.0f && timeofDay > 0.5f)
        {
            //building1.EnableKeyword("_EMISSION");
            //building2.EnableKeyword("_EMISSION");
            //building3.EnableKeyword("_EMISSION");
            //building4.EnableKeyword("_EMISSION");
            //building5.EnableKeyword("_EMISSION");
            //house1.EnableKeyword("_EMISSION");
            //house2.EnableKeyword("_EMISSION");
            //grid.EnableKeyword("_EMISSION");
            //farm.EnableKeyword("_EMISSION");
            light1.enabled = true;
            light2.enabled = true;
            light3.enabled = true;

            isNight = true;
            nightTime = true;
            Debug.Log(isNight); 
        }
        else
        {
            //building1.DisableKeyword("_EMISSION");
            //building2.DisableKeyword("_EMISSION");
            //building3.DisableKeyword("_EMISSION");
            //building4.DisableKeyword("_EMISSION");
            //building5.DisableKeyword("_EMISSION");
            //house1.DisableKeyword("_EMISSION");
            //house2.DisableKeyword("_EMISSION");
            //grid.DisableKeyword("_EMISSION");
            //farm.DisableKeyword("_EMISSION");

            light1.enabled = false;
            light2.enabled = false;
            light3.enabled = false;
            nightTime = false;
            //Debug.Log(nightTime);
        }
    }

    private void Start()
    {
        NormalTimeCurve();
    }

    private void UpdateTimeScale()
    {
        //24 hr day = 1, 1 hour = 24, 24 times faster 
        _timeScale = 24 / (_targetDayLength / 60);
        _timeScale *= timeCurve.Evaluate(timeofDay);
        _timeScale /= timeCurveNoramlization;
    }
    private void NormalTimeCurve()
    {
        float stepSize = 0.01f;
        int numberSteps = Mathf.FloorToInt(1f / stepSize);
        float curveTotal = 0;

        for(int i =0; i <numberSteps; i++)
        {
            curveTotal += timeCurve.Evaluate(i * stepSize);
        }

        timeCurveNoramlization = curveTotal / numberSteps;
    }

    private void UpdateTime()
    {
        _timeOfDay += Time.deltaTime * _timeScale / 86400;//seconds in a day
        if (_timeOfDay > 1) //its a new day
        {
            _dayNumber++;
            _timeOfDay -= 1; //restart the day

            if (_dayNumber > _yearLength)//new year 
            {
                _yearNumber++;
                _dayNumber = 0;
            }
        }
    }

    private void AdjustRotation()
    {
        float sunAngle = timeofDay * 360f;
        dailyRotation.transform.localRotation = Quaternion.Euler(new Vector3(sunAngle, 0f, 0f));
    }

    private void SunIntenity()
    {
        intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        //intensity = Mathf.Clamp01(intensity);

        sun.intensity = intensity * sunVariation + sunBaseIntensity;
    }

    private void AdjustSunColor()
    {
        sun.color = sunColor.Evaluate(intensity);
    }

}
