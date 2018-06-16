using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float PanBorderThickness = 10f;

    private void Update()
    {
        Vector3 pos = transform.position;


        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - PanBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= PanBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - PanBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= PanBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }



        transform.position = pos;
    }

}
