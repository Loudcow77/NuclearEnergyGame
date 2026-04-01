using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastForward : MonoBehaviour
{
  

    public void SpeedUp()
    {
            Debug.Log("FAST");
            Time.timeScale = 6f;
    }

    public void SpeedDown()
    {
            Time.timeScale = 1f;
    }
}
