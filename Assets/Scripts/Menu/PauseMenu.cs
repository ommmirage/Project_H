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

        hexMapContinents.ClearMap();
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
        SaveSystem saveSystem = new SaveSystem();
        saveSystem.SaveGame(hexMap);
    }

    public void Load()
    {
        HexMap hexMap = Object.FindObjectOfType<HexMap>();
        hexMap.ClearMap();
        
        SaveSystem saveSystem = new SaveSystem();
        saveSystem.LoadGame();
        pauseMenuUI.SetActive(false);
        mouseController.OnPause = false;
    }

    public void Quit()
    {

    }
}
