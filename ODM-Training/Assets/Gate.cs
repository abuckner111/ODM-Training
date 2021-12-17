using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{

    public selector sel;
    public bool open;
    private bool toggle;
    public Transform gate;
    public Transform lever;
    public Vector3 opened;
    public Vector3 closed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if(sel.selected)
        {
            if(sel.use != toggle)
            {
                toggle = sel.use;
                if(toggle)
                {
                    open = !open;
                }
            }

        }
        if(open)
        {
            lever.eulerAngles = new Vector3(0f,0f,-45f);
            gate.localPosition = Vector3.Lerp(gate.localPosition, opened, 0.5f*Time.deltaTime);
        }
        else
        {
            lever.eulerAngles = new Vector3(0f,0f,45f);
            gate.localPosition = Vector3.Lerp(gate.localPosition, closed, 0.5f*Time.deltaTime);
        }

    }
}
