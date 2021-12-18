using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class selectionController : MonoBehaviour
{
    public Camera  main;
    public Vector3 center;
    private Ray ray;
    public  RaycastHit hit;
    private string     sel = "Selectable";
    public Transform selection;
    public selector  select;
    public TMP_Text  actText;
    public RectTransform Canvas;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale != 0f)
        {

        if(selection != null)
        {
            Canvas.position = selection.TransformPoint(select.offset);
            Canvas.LookAt((main.GetComponent(typeof(Transform)) as Transform).position);
            Canvas.position += Canvas.forward*select.hover;
            select.selected = false;
            selection = null;
            select    = null;
        }

        center = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        ray = main.ScreenPointToRay(center);
        if(Physics.Raycast(ray, out hit))
        {
            selection = hit.transform;
            if(selection.CompareTag(sel))
            {
                select = selection.GetComponent<selector>();
                if(select != null && hit.distance <= select.range)
                {
                    select.selected = true;
                }
                else
                {
                    selection = null;
                    select    = null;
                }
            }
            else
            {
                selection = null;
                select    = null;
            }
            if(select != null)
            {
                actText.text = select.type;
                select.use = Input.GetButton("Use");
            }
            else
            {
                actText.text = "";
            }

        }
    }
    }
}
