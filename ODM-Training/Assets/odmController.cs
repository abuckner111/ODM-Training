using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class odmController : MonoBehaviour
{
    public selectionController select;

    public static class hookState
    {
        public static int loaded       = 1;
        public static int fired        = 2;
        public static int hooked       = 3;
        public static int retracting   = 4;
        public static int mask         = 1 << 8;
    }
    public odmClass odm;

    public Transform head;
    public Transform gear;

    private bool paused;
    private bool willStrafe;

    // Start is called before the first frame update
    void Start()
    {
        odm.Reload(odm.hookRight, gear);

        odm.Reload(odm.hookLeft, gear);
    }

    // Update is called once per frame
    void Update()
    {
        if(!paused)
        {
            if(odm.canStrafe)
            {
                if(Input.GetButtonDown("Jump"))
                {
                    willStrafe = true;
                }
            }
            if(Input.GetButton("Jump") && willStrafe)
            {
                odm.TickStrafe(head, odm.body.velocity.magnitude);
            }
            else if(willStrafe)
            {
                willStrafe = false;
                odm.StopStrafe();
            }

        }

    }

    void FixedUpdate()
    {
        if(Time.timeScale == 0f)
        {
            paused = true;
        }
        else
        {
            paused = false;
        }

        if(!paused)
        {
            if(select.selection != null)
            {
                if(select.select != null)
                {
                    if(select.select.type == "Refill" && Input.GetButton("Use"))
                    {
                        odm.refill = true;
                    }

                }
            }
            if(select.selection == null || select.select == null)
            {
                odm.refill = false;
            }
            if(odm.refill == true && !Input.GetButton("Use"))
            {
                odm.refill = false;
            }

            if(odm.hookLeft.state == hookState.loaded)
            {
                if(Input.GetButton("FireLeft"))
                {
                    odm.Fire(odm.hookLeft, head.forward, odm.launchSpeed);
                }
            }
            else
            {
                if(!Input.GetButton("FireLeft"))
                {
                    odm.Reload(odm.hookLeft, gear);
                }
            }

            if(odm.hookRight.state == hookState.loaded)
            {
                if(Input.GetButton("FireRight"))
                {
                    odm.Fire(odm.hookRight, head.forward, odm.launchSpeed);
                }
            }
            else
            {
                if(!Input.GetButton("FireRight"))
                {
                    odm.Reload(odm.hookRight, gear);
                }
            }

            if(odm.hookLeft.state == hookState.hooked)
            {
                if(Input.GetButton("FireLeft"))
                {
                    var leftSpeed = Mathf.Clamp(1f-2f*Input.GetAxis("Horizontal"), -1f, 2f);
                    odm.Reel(odm.hookLeft, Input.GetAxis("Reel")*leftSpeed);
                }
            }
            if(odm.hookRight.state == hookState.hooked)
            {
                if(Input.GetButton("FireRight"))
                {
                    var rightSpeed = Mathf.Clamp(1f+2f*Input.GetAxis("Horizontal"), -1f, 2f);
                    odm.Reel(odm.hookRight, Input.GetAxis("Reel")*rightSpeed);
                }
            }

            if(Input.GetAxis("Reel") == 0f || (!Input.GetButton("FireRight") && !Input.GetButton("FireLeft")))
            {
                odm.reel.Stop();
            }

            odm.RefillTick();
            odm.TickHook(odm.hookRight);
            odm.CableTick(odm.hookRight);
            odm.TickHook(odm.hookLeft);
            odm.CableTick(odm.hookLeft);


        }

    }


    [System.Serializable]
    public class hookClass
    {
        public GameObject   obj;
        public Transform    pos;
        public Rigidbody    rig;
        public SpringJoint  spring;
        public int          state;
        public Vector3      offset;
        public Vector3      offsetCable;
        public RaycastHit   hit;
        public Transform    cable;
        public Material     cableMat;
        public float        cableScale;


    }


    [System.Serializable]
    public class odmClass
    {
        [Tooltip("The spring stiffness of the simulated cable")]
        public float spring;

        [Tooltip("The velocity dampening when all the slack is gone in the cable")]
        public float damper;

        [Tooltip("This is the mass constant in the spring constraint")]
        public float massCon;

        [Tooltip("The initial launch speed of the hooks")]
        public float launchSpeed;

        [Tooltip("The maximum length of the cables")]
        public float maxLength;

        [Tooltip("The max speed at which the hooks are reeled in")]
        public float reelSpeed;

        [Tooltip("The maximum stretch distance before the cable unreels")]
        public float maxStretch;

        [Tooltip("The minimum operating pressure of the odm gear")]
        public float opPressure;

        [Tooltip("The current pressure stored in the system")]
        public float pressure;

        [Tooltip("The scale of gas consumption")]
        public float consumptionRate;

        [Tooltip("The rate at which strafing consumes gas")]
        public float strafeRate;

        [Tooltip("The strength of the force of strafing")]
        public float strafeForce;

        [Tooltip("Is the gear refilling on gas?")]
        public bool filling;

        public bool refill;

        [Tooltip("The player's Rigidbody")]
        public Rigidbody body;

        public bool canStrafe;

        public AudioSource  hit;
        public AudioSource  fire;
        public AudioSource  load;
        public float        fireTime;
        public bool         fireStop;
        public AudioSource  reel;
        public AudioSource  strafe;

        public AudioClip[]  hits;
        public AudioClip[]  fires;
        public AudioClip[]  reels;
        public AudioClip[]  loads;
        public AudioClip[]  fills;
        public AudioClip  strafes;
        public AudioClip  click;

        public Transform    needle;

        public hookClass hookLeft;

        public hookClass hookRight;



        public void CableTick(hookClass hook)
        {
            if(hook.state != hookState.loaded)
            {
                Vector3 p1 = this.body.transform.TransformPoint(hook.offset) + this.body.velocity*Time.fixedDeltaTime;
                Vector3 p2 = hook.pos.TransformPoint(hook.offsetCable) + hook.rig.velocity*Time.fixedDeltaTime;
                float length = (p2-p1).magnitude;
                hook.cable.localScale = new Vector3(50, 50, length*100);
                hook.cableMat.mainTextureScale = new Vector2(0.333333f,length)*hook.cableScale;
                hook.cable.position = p1;
                hook.cable.LookAt(p2);
            }
            else
            {
                hook.cable.localScale = Vector3.zero;
            }

        }

        public void Strafe(Vector2 dir, float curSpeed, float maxSpeed)
        {
            if(dir.magnitude > 0.1f)
            {
                var dirt = new Vector3(dir.x, 0f, dir.y);
                dirt = dirt*Mathf.Clamp(1f-(curSpeed/maxSpeed-1f),0f,1f);
                //dirt = body.transform.TransformVector(dirt);
                if(pressure > 0f)
                {
                    this.pressure = Mathf.Max(pressure-this.strafeRate*dir.magnitude*Time.deltaTime, 0f);
                    this.body.AddForce(dirt*this.strafeForce*Time.deltaTime);
                }
            }
        }
        public void PlayStrafe()
        {
            if(this.pressure > 0f && this.strafe.loop == false && this.strafe.isPlaying == false)
            {
                this.strafe.pitch = 1.5f;
                this.strafe.clip = strafes;
                this.strafe.loop = true;
                this.strafe.Play();
            }
        }
        public void StopStrafe()
        {
            if(this.strafe.loop == true && this.strafe.isPlaying == true && this.strafe.clip == strafes)
            this.strafe.pitch = 1f;
            this.strafe.loop = false;
            this.strafe.Stop();
        }

        public void TickStrafe(Transform view, float sped)
        {
            if(this.canStrafe)
            {
                /*var dire = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                */
                Vector3 forw = view.forward;
                var dire = (new Vector2(forw.x, forw.z)).normalized;

                if(dire.magnitude > 0f)
                {
                    this.Strafe(dire, sped, 10f);
                    this.PlayStrafe();
                }
                else
                {
                    this.StopStrafe();
                }
            }
            else
            {
                this.StopStrafe();
            }
        }

        public void TickHook(hookClass hook) //Only call in fixedUpdate();
        {

            needle.localRotation = Quaternion.Euler(0f,0f,pressure*270-(45+90));
            if(fireStop)
            {
                if(fire.time >= fireTime)
                {
                    fire.Stop();
                }
            }
            if(hook.state == hookState.fired)
            {
                checkHit(hook);
                hook.pos.LookAt(hook.pos.position-hook.rig.velocity);
            }

            /*if(hook.state != hookState.loaded)
            {
                hook.line.SetPosition(0, this.body.transform.TransformPoint(hook.offset)+this.body.velocity*Time.fixedDeltaTime);
                hook.line.SetPosition(1, hook.pos.TransformPoint(hook.offsetCable) + hook.rig.velocity*Time.fixedDeltaTime);
            }*/
            if(hook.state == hookState.hooked)
            {
                float stretch = (hook.pos.position-this.body.transform.position).magnitude-this.maxStretch;
                if(hook.spring.maxDistance < stretch)
                {
                    hook.spring.maxDistance = stretch;
                }
            }
            if(hook.state == hookState.retracting)
            {

            }
        }

        public AudioClip RandomClip(AudioClip[] clips)
        {
            if(clips.Length > 1)
            {
                int sel = Mathf.RoundToInt(Random.Range(0f, clips.Length-1));
                return clips[sel];
            }
            else
            {
                return clips[0];
            }
        }

        public void RefillTick()
        {
            if(this.refill == true && this.filling == false && this.pressure < 1f)
            {
                this.filling = true;
                if(load.isPlaying == false)
                {
                    this.load.loop = false;
                    this.load.clip = loads[0];
                    this.load.Play();
                }
            }

            if(this.refill && this.filling && this.pressure < 1f)
            {
                if(this.load.isPlaying == false && this.load.loop == false)
                {
                    this.load.clip = fills[0];
                    this.load.loop = true;
                    this.load.Play();
                }

                if(this.load.isPlaying == true && this.load.loop == true)
                {
                    this.pressure = Mathf.Min(1f, this.pressure + (1.5f - this.pressure)/10f*Time.fixedDeltaTime);
                }
            }
            if((this.refill == false || this.pressure == 1f) && this.filling == true)
            {
                this.filling = false;
                if(this.load.isPlaying && this.load.loop == true)
                {
                    this.load.Stop();
                    this.load.loop = false;
                }
                if(this.load.loop == false)
                {
                    if(this.loads.Length > 1)
                    {
                        this.load.clip = this.loads[1];
                        this.load.loop = false;
                        this.load.Play();
                    }
                }
            }

        }

        public void Reload(hookClass hook, Transform parent)
        {
            /*hook.line.SetPosition(0, Vector3.zero);
            hook.line.SetPosition(1, Vector3.zero);
            */
            hook.spring.spring      = 0f;
            hook.spring.damper      = 0f;
            hook.spring.minDistance = 0f;
            hook.spring.connectedMassScale   = this.massCon;
            hook.spring.connectedBody = null;
            hook.pos.SetParent(parent);
            hook.rig.constraints    = RigidbodyConstraints.FreezeAll;
            hook.rig.velocity       = Vector3.zero;
            hook.pos.localPosition  = hook.offset;
            hook.state              = hookState.loaded;
        }

        public void checkHit(hookClass hook)
        {
            if(Physics.Raycast(hook.pos.position, hook.rig.velocity.normalized, out hook.hit, hook.rig.velocity.magnitude*Time.fixedDeltaTime))
            {
                if(hook.hit.transform == null)
                {
                    Hit(hook, hook.hit.point, hook.hit.normal, null);
                }
                else
                {
                    Hit(hook, hook.hit.point, hook.hit.normal, hook.hit.transform);
                }

                if(hits.Length > 0)
                {
                    hit.clip = RandomClip(hits);
                    hit.Play();
                    if(fire.isPlaying == true)
                    {
                        if(fire.time >= fireTime)
                        {
                            fireStop = false;
                            fire.Stop();
                        }
                        else
                        {
                            fireStop = true;
                        }

                    }
                }
            }
        }

        public void Fire(hookClass hook, Vector3 direction, float lSpeed)
        {
            if(hook.state == hookState.loaded && this.pressure > 0f)
            {
                if(fires.Length > 0)
                {
                    fire.clip = RandomClip(fires);
                    fire.pitch = 1f;
                    fire.Play();
                    fireStop = false;
                }

                this.pressure = Mathf.Max(this.pressure - 0.005f, 0);

                checkHit(hook);
                hook.pos.SetParent(null);
                hook.state = hookState.fired;
                hook.rig.constraints = RigidbodyConstraints.None;
                hook.rig.velocity = direction.normalized*Mathf.Min(this.pressure/this.opPressure, 1)*lSpeed + body.velocity;
            }
            else if(pressure == 0f)
            {
                /*if(fire.isPlaying == false)
                {
                    fire.clip = click;
                    fire.pitch = 1.5f;
                    fire.Play();
                }*/

            }
        }

        public void Hit(hookClass hook, Vector3 pos, Vector3 normal, Transform parent)
        {
            Vector3 vela = hook.rig.velocity;

            if(parent != null)
            {
                hook.spring.connectedMassScale = this.massCon;
                hook.spring.massScale = 1f/this.massCon;
                hook.pos.SetParent(parent);
            }
            else
            {
                hook.spring.connectedMassScale = this.massCon;
                hook.spring.massScale = 1f;
                hook.pos.SetParent(null);
            }

            hook.state = hookState.hooked;
            hook.rig.constraints = RigidbodyConstraints.FreezeAll;
            hook.rig.velocity = Vector3.zero;
            hook.pos.position = pos;
            hook.pos.LookAt(hook.pos.position-vela);
            hook.spring.maxDistance = (pos-this.body.worldCenterOfMass).magnitude;
            hook.spring.connectedBody = this.body;
            hook.spring.spring = this.spring;
            hook.spring.damper = this.damper;
        }

        public void Reel(hookClass hook, float speed)
        {
            if(speed != 0f && pressure > 0.005f*this.consumptionRate*Time.fixedDeltaTime)
            {
                if(reel.isPlaying == false)
                {
                    reel.clip = reels[0];
                    reel.loop = true;
                    reel.Play();
                }
                float dis = (hook.pos.position-this.body.transform.position).magnitude;
                if(hook.spring.maxDistance > dis)
                {
                    hook.spring.maxDistance = dis;
                }
                pressure -= 0.005f*this.consumptionRate*Mathf.Max(0f,speed)*Time.fixedDeltaTime;
                hook.spring.maxDistance = Mathf.Max(hook.spring.maxDistance - this.reelSpeed*Time.fixedDeltaTime*speed*Mathf.Min(this.pressure/this.opPressure, 1),0);
            }
        }

        public void Retract(hookClass hook)
        {

        }
    }
}
