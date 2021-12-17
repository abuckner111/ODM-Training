using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticule : MonoBehaviour
{
    public Camera cam;
    public RectTransform left;
    public RectTransform right;
    public odmController odm;

    public Vector3 leftHook;
    public Vector3 rightHook;
    public float   grav;
    public Vector3 gr;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void OnGUI()
    {
        leftHook = checkHit(odm.odm.hookLeft);
        grav = grav/odm.odm.launchSpeed;
        gr = Physics.gravity*grav*grav/2f;
        leftHook += gr;
        leftHook = cam.WorldToScreenPoint(leftHook);
        rightHook = checkHit(odm.odm.hookRight);
        grav = grav/odm.odm.launchSpeed;
        gr = Physics.gravity*grav*grav/2f;
        rightHook += gr;
        rightHook = cam.WorldToScreenPoint(rightHook);

        if(leftHook != Vector3.positiveInfinity && Mathf.Abs(leftHook.z) != Mathf.Infinity)
        {
            left.position = leftHook;
        }
        if(rightHook != Vector3.positiveInfinity && Mathf.Abs(rightHook.z) != Mathf.Infinity)
        {
            right.position = rightHook;
        }

        if(odm.odm.hookLeft.state == odmController.hookState.hooked)
        {
            left.position = cam.WorldToScreenPoint(odm.odm.hookLeft.pos.position);
        }
        if(odm.odm.hookRight.state == odmController.hookState.hooked)
        {
            right.position = cam.WorldToScreenPoint(odm.odm.hookRight.pos.position);
        }
    }

    Vector3 checkHit(odmController.hookClass hook)
    {
        RaycastHit hit;
        if(Physics.Raycast(hook.pos.position, (cam.transform.forward*odm.odm.launchSpeed+odm.odm.body.velocity).normalized, out hit, odm.odm.maxLength))
        {
            grav = hit.distance;
            return hit.point;
        }
        return Vector3.positiveInfinity;
    }
}
