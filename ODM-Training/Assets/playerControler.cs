using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerControler : MonoBehaviour
{
    public gameController   pauseControl;
    public Rigidbody        rig;

    public float            walkForce;

    [Tooltip("The capsule collider for the player collisions")]
    public CapsuleCollider playerCollider;

    public bool pauseLast = true;
    public bool onGround = true;

    public Transform Head;

    [System.Serializable]
    public class movementClass {
        [Tooltip("The power output of the players legs: this determines top running speed")]
        public float power;

        [Tooltip("The max force that the player can use to walk")]
        public float strength;

        [Tooltip("The launch speed in meters a second when the player jumps")]
        public float jump;

        [Tooltip("The height of the player")]
        public float height;

        [Tooltip("The material for walking friction, should be no friction")]
        public PhysicMaterial walk;

        [Tooltip("The material for ducking this has some friction for sliding")]
        public PhysicMaterial duck;
    }

    public movementClass movementStats;
    public Vector2 lookSpeed;

    public Vector2 look;

    void Look()
    {
        look.x -= Input.GetAxis("Mouse X")*lookSpeed.x*Time.deltaTime;
        look.y -= Input.GetAxis("Mouse Y")*lookSpeed.y*Time.deltaTime;

        look.y = Mathf.Clamp(look.y, -90, 90);

        transform.rotation = Quaternion.Euler(0,-look.x,0);
        Head.localRotation = Quaternion.Euler(look.y,0,0);
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump") == true)
        {
            rig.AddForce(Vector3.up*movementStats.jump, ForceMode.VelocityChange);
        }
    }

    void Duck()
    {
        if(Input.GetButton("Duck"))
        {
            playerCollider.height = Mathf.Lerp(playerCollider.height, 0.5f*movementStats.height,8f*Time.deltaTime);
            playerCollider.material = movementStats.duck;
        }
        else
        {
            playerCollider.height = Mathf.Lerp(playerCollider.height, movementStats.height,8f*Time.deltaTime);
            playerCollider.material = movementStats.walk;
        }
    }

    void Walk()
    {
        //Local velocity
        Vector3 lv = transform.InverseTransformVector(rig.velocity);

        //Walk Force axis space
        Vector3 wf = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        wf = wf.normalized;

        if(lv.magnitude > 1f)
        {
            wf = wf*2 - lv.normalized;
            if(wf.magnitude > 1f)
            {
                wf = wf.normalized;
            }

            wf *= Mathf.Clamp(movementStats.power/(lv.magnitude)/(Input.GetButton("Duck") ? 4f : 1f), 0f, movementStats.strength);
        }
        else
        {
            wf *= movementStats.strength;
        }

        wf *= 100f;

        //wf world space
        wf = transform.TransformVector(wf*Time.deltaTime);

        walkForce = wf.magnitude;

        rig.AddForce(wf, ForceMode.Force);
    }

    void TestGround()
    {
        Vector3 p1 = playerCollider.center + Vector3.up*(playerCollider.height/2-playerCollider.radius);
        Vector3 p2 = playerCollider.center*2 - p1;
        p1 = transform.TransformPoint(p1) + new Vector3(0f,0.1f,0f);
        p2 = transform.TransformPoint(p2) + new Vector3(0f,0.1f,0f);
        if(Physics.CapsuleCast(p2, p1, playerCollider.radius, -Vector3.up, 0.25f))
        {
            if(!onGround)
            {
                onGround = true;
            }
        }
        else
        {
            if(onGround)
            {
                onGround = false;
            }
        }
    }

    void OnPause()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void OnResume()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnPauseChange()
    {

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if(pauseLast != pauseControl.paused) //Lock mouse so it can't leave the screen while we capture motion
        {
            pauseLast = pauseControl.paused;
            OnPauseChange();
            if(pauseControl.paused)
            {
                OnPause();
            }
            else
            {
                OnResume();
            }
        }


        if(pauseControl.paused == false) //Run script only when game is running
        {
            TestGround();
            Look();

            if(onGround)
            {
                Jump();
                Walk();
                Duck();
            }
        }
    }
}
