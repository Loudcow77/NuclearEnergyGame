using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 2;
    public float dragSpeed = 2;

    public Texture2D _cursor, _hand;
    private Vector3 dragOrigin;

    float delay = 0.2f;
    float clickTime;

    public float zoomMin = 18;
    public float zoomMax = 60;
    public float zoomSpeed = 10;
    public float zoomScale; //starting field of view value

    // Start is called before the first frame update
    void Start()
    {
        zoomScale = 39.4f;
    }

    // Update is called once per frame
    void Update()
    {
        // Screen only move when menu is off
        if (!ObjectPlacer.OnMenu && !SeasonManager.popup)
        {
            //Zoom in or out camera buy mouse Scroll Wheel (comment this out, mouses are meant for clicking)
            //zoomCamera(Input.GetAxis("Mouse ScrollWheel") * zoomSpeed);

            //Zoom in or out camera buy fingers in mobile


            //Get the X and Y position of any input (Keyboard Movement)
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            //Set the move direction (Keyboard Movement)
            Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput);

            //Set camera to new position (Keyboard Movement)
            transform.position = transform.position + moveDirection * moveSpeed * Time.deltaTime;

            if (zoomScale <= 37)
            {
                //Check if left mouse button down and set the click time and drag origin
                if (Input.GetMouseButtonDown(0))
                {
                    clickTime = Time.time;
                    dragOrigin = Input.mousePosition;
                }

                //Set the cursor image back to default
                //if (Input.GetMouseButtonUp(0))
                //    Cursor.SetCursor(_cursor, Vector2.zero, CursorMode.Auto);

                if (!Input.GetMouseButton(0))
                    return;
                //else
                //{
                //    //check if if left mouse button is pressed large than the delay time, set cursor to hand
                //    if (Time.time - clickTime >= delay)
                //        Cursor.SetCursor(_hand, new Vector2(_hand.width / 2, _hand.height / 2), CursorMode.Auto);
                //}

                //Set camera position when player is use mouse to drag the screen
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                Vector3 move = new Vector3(-pos.x * dragSpeed, 0, -pos.y * dragSpeed);
                dragOrigin = Input.mousePosition;
                transform.Translate(move, Space.World);
            }
        }
    }

    void zoomCamera(float value)
    {
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - value, zoomMin, zoomMax);
    }

    public void zoomCameraFixedIn()
    {
        if (zoomScale > 18 && zoomScale < 40)
        {
            zoomScale -= 3;
            Camera.main.fieldOfView = zoomScale;
        }
        
        else
            zoomScale = 39.4f;
    }

    public void zoomCameraFixedOut()
    {
        if (zoomScale > 18 && zoomScale < 40)
        {
            zoomScale += 3;
            Camera.main.fieldOfView = zoomScale;
        }

        else
            zoomScale = 39f;
    }
        public void zoomCameraSlider(float value)
    {
        Camera.main.fieldOfView = 30 + value * 30;
    }
}
