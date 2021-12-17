using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class playerControl : MonoBehaviour
{
    public int countColliders = 0;

    public bool OnGround() {
        return countColliders>0;
    }

    public collideControl   foot;

    public gameController   control;
    public configClass      config;
    public linkClass        link;
    public movementClass    movement;
    public bool             lastPause;

    // Start is called before the first frame update
    void Start()
    {
        movement.LockMouse();
    }

    // Update is called once per frame
    void Update()
    {
        if(foot.countColliders != countColliders)
        {
            if(foot.countColliders-countColliders > 0)
            {
                movement.PlayLanding();
            }
            countColliders = foot.countColliders;

        }

        if(control.pauseControl.paused != lastPause)
        {
            lastPause = control.pauseControl.paused;
            if(lastPause == true)
            {
                movement.FreeMouse();
            }
            else
            {
                movement.LockMouse();
            }
        }

        if(control.pauseControl.paused == false)
        {
            movement.ViewControl(
                link.bodyTransform,
                link.headTransform,
                config.viewSensitivity,
                config.senX,
                config.senY);

            if(OnGround())
            {
                if(Input.GetButtonDown("Jump"))
                {
                    movement.Jump(link.bodyRigidbody);
                }
            }
        }

    }

    void FixedUpdate()
    {
        movement.calcPhys(link.bodyRigidbody);
        if(control.pauseControl.paused == false)
        {
            if(OnGround())
            {

                movement.WalkControl(link.bodyRigidbody, link.bodyTransform, foot.main);
            }

            if(link.odmGear != null)
            {
                link.odmGear.odm.canStrafe = !OnGround();
            }
        }
    }

    [System.Serializable]
    public class movementClass
    {
        public AudioSource step;
        public float       stepRatio = 1f;
        public AudioClip[] steps;

        public AudioSource jump;
        public AudioClip[] jumps;

        public float legFriction = 1f;

        [Tooltip("How long you can sustain legStrength")]
        public float legPower;

        [Tooltip("The peak force the legs produce for movement")]
        public float legStrength;

        public Vector3 accel;
        public Vector3 jerk;
        private Vector3 _accel;
        private Vector3 _jerk;

        public void calcPhys(Rigidbody bod)
        {
            this.accel = (bod.velocity-this._accel)/Time.fixedDeltaTime;
            this.jerk  = (this.accel-this._jerk)/Time.fixedDeltaTime;
            this._jerk = this._accel;
            this._accel = bod.velocity;
        }

        public void LockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void FreeMouse()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        public void ViewControl(Transform body, Transform head, float sen, float x, float y)
        {
            body.rotation = Quaternion.Euler(0f,body.rotation.eulerAngles.y + Input.GetAxis("Mouse X")*x*sen,0f);

            head.localRotation = Quaternion.Euler(head.localRotation.eulerAngles.x - Input.GetAxis("Mouse Y")*y*sen, 0f, 0f);
        }

        public void Jump(Rigidbody rig)
        {
            rig.AddForce(0f,6.3f,0f, ForceMode.VelocityChange);
        }

        public void PlayLanding()
        {
            if(!jump.isPlaying)
            {
                int selected = Mathf.FloorToInt(Random.value*(jumps.Length));
                jump.clip = jumps[selected];
                jump.Play();
            }

        }

        public void WalkControl(Rigidbody bodyR, Transform bodyT, Rigidbody foot)
        {

            Vector3 walk;
            Vector3 vel;
            Vector3 dir;
            Vector3 fric;
            float   dot;
            float   speed;

            void PlayStep()
            {
                if(!step.isPlaying)
                {
                    int selected = Mathf.FloorToInt(Random.value*(steps.Length));
                    step.clip = steps[selected];
                    step.PlayDelayed(stepRatio/speed);
                }
            }

            vel  = bodyR.velocity.normalized;
            speed= bodyR.velocity.magnitude;
            walk = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
            walk = bodyT.TransformDirection(walk);
            dir  = Vector3.Project(walk,vel);
            dot  = Mathf.Max(Vector3.Dot(vel,walk),0);
            /*if(walk.magnitude == 0f)
            {
                walk = -vel;
            }*/
            fric = -bodyR.velocity*bodyR.mass/Time.fixedDeltaTime;
            var max = this.legFriction*bodyR.mass;
            if(fric.magnitude > max)
            {
                fric = fric.normalized*max;
            }
            if(speed == 0f)
            {
                vel = walk*this.legStrength;
                step.Stop();
            }
            else
            {
                if(speed > 0.5f)
                {
                    PlayStep();
                }
                if(Input.GetButton("Sprint"))
                {
                    vel = (walk)*Mathf.Min(this.legStrength,this.legPower/speed);// + (1-dot)*(walk-dir)*this.legStrength;
                }
                else
                {
                    vel = (walk)*Mathf.Min(this.legStrength,this.legPower/speed/4);// + (1-dot)*(walk-dir)*this.legStrength;
                }

            }


            bodyR.AddForce(vel+fric);
            if(foot != null)
            {
                foot.AddForce(-vel-fric);
            }

        }
    }

    [System.Serializable]
    public class configClass
    {
        [Tooltip("The overall strength of view speed")]
        public float viewSensitivity = 1f;
        [Tooltip("The view look strength for the X axis")]
        public float senX = 1f;
        [Tooltip("The view look strength for the Y axis")]
        public float senY = 1f;
    }

    [System.Serializable]
    public class linkClass
    {
        public Transform headTransform;
        public Transform bodyTransform;
        public Rigidbody bodyRigidbody;
        public odmController odmGear;
    }
}
