using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControler : MonoBehaviour
{
    public Transform Head;

    [System.Serializable]
    public class movementClass {
        public float power;
        public float strength;
    }

    public movementClass movementStats;
    public Vector2 lookSpeed;

    private Vector2 look;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        look.x -= Input.GetAxis("Mouse X");
        look.y -= Input.GetAxis("Mouse Y");
        transform.rotation = Quaternion.Euler(0,-look.x*lookSpeed.x,0);
        Head.localRotation = Quaternion.Euler(look.y*lookSpeed.y,0,0);
    }
}
