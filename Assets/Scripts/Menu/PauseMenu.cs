using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    MouseController mouseController;
    public GameObject pauseMenuUI;

    void Start()
	{
		mouseController = Object.FindObjectOfType<MouseController>();
    }

    void Update()
    {
        if ( (Input.GetKeyDown(KeyCode.Escape)) && (mouseController.SelectedHex == null) )
        {
            if (mouseController.OnPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        mouseController.OnPause = false;
    }

    void Pause()
    {
        mouseController.OnPause = true;
        pauseMenuUI.SetActive(true);
    }

    public void Save()
    {

    }

    public void Load()
    {

    }

    public void Quit()
    {
        
    }
}
