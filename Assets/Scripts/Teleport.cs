using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Locking : MonoBehaviour
{
    [SerializeField]
    GameObject cameraOffset;

    float y = 0;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
       

        //Button A
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            cameraOffset.transform.position = new   Vector3(0, 0.15f, 2.5f);

        }

        //Button B
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            cameraOffset.transform.position = new  Vector3(0, 0.15f, 22.3f);

        }

    }
}
