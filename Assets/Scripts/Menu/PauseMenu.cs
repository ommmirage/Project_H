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
        HexMap hexMap = Object.FindObjectOfType<HexMap>();
        SaveSystem.SaveGeo(hexMap.Hexes);
    }

    public void Load()
    {
        SaveSystem.LoadGeo();
    }

    public void Quit()
    {

    }
}
