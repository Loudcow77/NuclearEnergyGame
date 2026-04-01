using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    private ObjectPlacer premade1;
    private ObjectPlacer premade2;
    private ObjectPlacer premade3;

    // Start is called before the first frame update
    void Start()
    {
        premade1 = GameObject.Find("Build Area 1").GetComponentInChildren<ObjectPlacer>();
        premade2 = GameObject.Find("Build Area 4").GetComponentInChildren<ObjectPlacer>();
        premade3 = GameObject.Find("Build Area 9").GetComponentInChildren<ObjectPlacer>();

        FindObjectOfType<AudioManager>().Play("SummerSound");
        FindObjectOfType<AudioManager>().Play("GameMusic");

        premade1.PlaceBuildingWhenGameStart(0);
        premade2.PlaceBuildingWhenGameStart(2);
        premade3.PlaceBuildingWhenGameStart(3);
    }
}
