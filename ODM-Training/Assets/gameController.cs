using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    public pauseClass pauseControl;
    public pauseMenuClass pauseMenu;
    public bool fullscreen;
    private bool _fullscreen;
    public UnityEngine.UI.Toggle tog;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(_fullscreen != fullscreen)
        {
            _fullscreen = fullscreen;
            Screen.fullScreen = fullscreen;
        }
        if(Input.GetButtonDown("Pause") && pauseControl.paused == false)
        {
            pauseControl.pause();
            pauseMenu.pause();
        }
    }

    public void SetFullscreen()
    {
        fullscreen = tog.isOn;
    }

    public void ResumeButton()
    {
        pauseControl.resume();
        pauseMenu.resume();
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    [System.Serializable]
    public class pauseMenuClass
    {
        public GameObject Panel;

        public void pause()
        {
            this.Panel.SetActive(true);
        }

        public void resume()
        {
            this.Panel.SetActive(false);
        }
    }

    [System.Serializable]
    public class pauseClass
    {
        public bool     paused      = true;
        public float    storedSpeed = 1f;

        public void pause()
        {
            if(Time.timeScale != 0f)
            {
                this.storedSpeed    = Time.timeScale;
                Time.timeScale      = 0f;
            }
            this.paused = true;
        }

        public void resume()
        {
            if(this.storedSpeed != 0f)
            {
                Time.timeScale = this.storedSpeed;
            }
            else
            {
                Time.timeScale = 1f;
            }
            this.paused = false;
        }

        public void setPause(bool pauseState)
        {
            if(pauseState != this.paused)
            {
                this.paused = pauseState;
                if(pauseState == true)
                {
                    this.pause();
                }
                else
                {
                    this.resume();
                }
            }
        }
    }
}
