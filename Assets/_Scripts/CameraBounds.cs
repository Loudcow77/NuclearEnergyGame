using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Source: https://forum.unity.com/threads/click-drag-camera-movement.39513/
public class CameraBounds : MonoBehaviour
{
    public Camera linkedCamera;
    private BoxCollider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = this.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float vertExtent = linkedCamera.orthographicSize;
        float horizExtent = vertExtent * Screen.width / Screen.height;

        Vector3 linkedCameraPos = linkedCamera.transform.position;
        Bounds areaBounds = boxCollider.bounds;

        linkedCamera.transform.position = new Vector3(
            Mathf.Clamp(linkedCameraPos.x, areaBounds.min.x, areaBounds.max.x),
            linkedCameraPos.y,
            Mathf.Clamp(linkedCameraPos.z, areaBounds.min.z - horizExtent, areaBounds.max.z - horizExtent));
    }
}
