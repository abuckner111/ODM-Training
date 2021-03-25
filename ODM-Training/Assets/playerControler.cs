﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerControler : MonoBehaviour
{
    public gameController pauseControl;
    public bool pauseLast = true;

    public Transform Head;

    [System.Serializable]
    public class movementClass {
        public float power;
        public float strength;
    }

    public movementClass movementStats;
    public Vector2 lookSpeed;

    public Vector2 look;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(pauseLast != pauseControl.paused)
        {
            pauseLast = pauseControl.paused;

            if(pauseControl.paused)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        if(pauseControl.paused == false)
        {
            look.x -= Input.GetAxis("Mouse X")*lookSpeed.x;
            look.y -= Input.GetAxis("Mouse Y")*lookSpeed.y;

            look.y = Mathf.Clamp(look.y, -90, 90);

            transform.rotation = Quaternion.Euler(0,-look.x,0);
            Head.localRotation = Quaternion.Euler(look.y,0,0);
        }
    }
}
