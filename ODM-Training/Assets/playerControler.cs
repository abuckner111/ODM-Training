﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [System.Serializable]
    public class movementClass {
        public float power;
        public float strength;
    }

    public movementClass movementStats;
}