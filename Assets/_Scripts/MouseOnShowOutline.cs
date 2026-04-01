using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOnShowOutline : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;

    public GameObject metarialObject;

    public GameObject popUP;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.name == this.gameObject.name)
            {
                popUP.SetActive(true);
                popUP.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y - 30, 0);
                metarialObject.GetComponent<Outline>().enabled = true;
            }
            else
            {
                popUP.SetActive(false);
                metarialObject.GetComponent<Outline>().enabled = false;
            }
        }
        else
        {
            popUP.SetActive(false);
            metarialObject.GetComponent<Outline>().enabled = false;
        }
    }
}
