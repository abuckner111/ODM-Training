using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collideControl : MonoBehaviour
{
    public Collider body;
    public Collider self;
    public int countColliders = 0;
    public Rigidbody main;
    public void OnCollisionEnter(Collision collision) {
        countColliders++;
        if(collision.rigidbody != null)
        {
            main = collision.rigidbody;
        }
    }

    public void OnCollisionExit(Collision collision) {
        if(collision.rigidbody == main)
        {
            main = null;
        }
        countColliders--;
    }


    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreCollision(self, body, true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
