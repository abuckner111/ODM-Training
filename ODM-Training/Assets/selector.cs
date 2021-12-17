using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selector : MonoBehaviour
{
    public bool selected = false;
    private bool _selected = true;
    public bool use    = false;

    public float    range = 1f;
    public string   type;

    public Material highlight;

    public Renderer[] ren;
    public Material[] mats;
    // Start is called before the first frame update

    void Deselect()
    {
        if(ren.Length > 0)
        {
            int i = 0;
            while (i < ren.Length)
            {
                ren[i].material = mats[i];
                i++;
            }
        }
    }

    void Select()
    {
        if(ren.Length > 0 && mats.Length == ren.Length)
        {
            int i = 0;
            while(i < ren.Length)
            {
                ren[i].material = highlight;
                i++;
            }
        }
    }

    void AssignMats()
    {
        if(ren.Length > 0 && mats.Length == ren.Length)
        {
            int i = 0;
            while(i < ren.Length)
            {
                mats[i] = ren[i].material;
                i++;
            }
        }
    }

    void Start()
    {
        AssignMats();
        _selected = !selected;
    }

    // Update is called once per frame
    void Update()
    {
        if(selected != _selected)
        {
            _selected = selected;
            if(selected)
            {
                Select();
            }
            else
            {
                Deselect();
            }
        }

        if(selected == false && use == true)
        {
            use = false;
        }

    }
}
