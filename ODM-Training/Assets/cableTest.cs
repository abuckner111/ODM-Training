using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cableTest : MonoBehaviour
{
    public LineRenderer line;
    public Transform    character;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, character.position);
    }
}
