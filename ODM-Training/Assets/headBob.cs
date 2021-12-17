using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headBob : MonoBehaviour
{
    public Camera       self;
    public Rigidbody    player;

    public Vector3      offset;
    public float        strength    = 0.1f;
    public float        lerp        = 0.95f;
    public float        max         = 0.5f;
    private Vector3     lastVel;
    public Vector3      accel;
    // Start is called before the first frame update
    void Start()
    {
        lastVel = player.velocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        accel = (player.velocity - lastVel)/Time.fixedDeltaTime;
        if(accel.magnitude > max/Mathf.Abs(strength))
        {
            accel = accel/accel.magnitude*max/Mathf.Abs(strength);
        }
        lastVel = player.velocity;
        self.transform.localPosition = Vector3.Lerp(offset + self.transform.InverseTransformVector(accel*strength), self.transform.localPosition, lerp);
    }
}
