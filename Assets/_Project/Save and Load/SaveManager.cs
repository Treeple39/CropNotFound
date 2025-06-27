using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    private GameData gameData;
    public static SaveManager instance;
    private List<ISaveManager> saveManagers;
    private FileDataHandler fileDataHandler;

    [SerializeField] private string fileName;

    [ContextMenu("DELETE SAVE FILE")]
    private void DeleteSavedData()
    {
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        fileDataHandler.Delete();
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        saveManagers = FindAllSaveManager();
        fileDataHandler = new FileDataHandler(Application.persistentDataPath,fileName);
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }
    public void LoadGame()
    {
        gameData = fileDataHandler.Load();

        if (this.gameData == null)
        {
            NewGame();
        }
        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    public void SaveGame() {
        foreach (ISaveManager saveManager in saveManagers) {
            saveManager.SaveData(ref gameData);
        }
        fileDataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManager()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }
}
