using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    MouseController mouseController;
    public GameObject pauseMenuUI;

    void Start()
	{
		mouseController = Object.FindObjectOfType<MouseController>();
        pauseMenuUI.SetActive(true);
        mouseController.OnPause = true;
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

    public void NewGame()
    {
        HexMapContinents hexMapContinents = Object.FindObjectOfType<HexMapContinents>();
        hexMapContinents.GenerateMap();

        pauseMenuUI.SetActive(false);
        mouseController.OnPause = false;
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
        SaveSystem.SaveGeo(hexMap);
    }

    public void Load()
    {
        SaveSystem.LoadGeo();
        pauseMenuUI.SetActive(false);
        // mouseController.OnPause = false;
    }

    public void Quit()
    {
    }
}
