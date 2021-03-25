using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    public bool paused = false;
    public bool pauseLast = true;

    public void setPause(bool p)
    {
        paused = p;
    }

    public void OnPause()
    {
        PausePanel.SetActive(true);
    }


    public void OnResume()
    {
        PausePanel.SetActive(false);
    }

    public GameObject PausePanel;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!paused && Input.GetButton("Cancel") == true)
        {
            paused = true;
        }


        if(paused != pauseLast)
        {
            if(paused)
            {
                OnPause();
            }
            else
            {
                OnResume();
            }
            pauseLast = paused;
        }
    }
}
