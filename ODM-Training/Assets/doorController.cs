using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour
{
    public selector open;
    public selector close;
    public Transform door;
    public float openAngle;
    public float closeAngle;

    public float doorPos;
    public float doorSpeed;

    public float friction;
    public float force;
    public float bounce;
    public float maxSpeed;

    public Vector3 offset;

    public bool x;
    public bool y;
    public bool z;

    public void DoorTick()
    {
        doorPos += doorSpeed*Time.deltaTime;

        doorSpeed += Mathf.Clamp(-doorSpeed, -friction*Time.deltaTime, friction*Time.deltaTime);

        if(open.use == true)
        {
            doorSpeed = Mathf.Clamp(doorSpeed + force*Time.deltaTime*Mathf.Sign(openAngle), -maxSpeed, maxSpeed);
        }
        if(close.use == true)
        {
            doorSpeed = Mathf.Clamp(doorSpeed-force*Time.deltaTime*Mathf.Sign(openAngle), -maxSpeed, maxSpeed);
        }

        if(openAngle > closeAngle)
        {
            if(doorPos > openAngle || doorPos < closeAngle)
            {
                doorSpeed = -doorSpeed*bounce;
            }
            doorPos = Mathf.Clamp(doorPos, closeAngle, openAngle);
        }
        else
        {
            if(doorPos > closeAngle || doorPos < openAngle)
            {
                doorSpeed = -doorSpeed*bounce;
            }
            doorPos = Mathf.Clamp(doorPos, openAngle, closeAngle);
        }

        var oo = new Vector3(0f,0f,0f);
        if(x)
        {
            oo.x = 1f;
        }
        if(y)
        {
            oo.y = 1f;
        }
        if(z)
        {
            oo.z = 1f;
        }

        door.eulerAngles = oo*doorPos+offset;

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DoorTick();
    }
}
