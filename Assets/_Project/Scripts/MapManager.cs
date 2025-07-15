using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : Singleton<MapManager>
{
    [Header("��ͼԤ����")]
    public GameObject livingroom;
    public GameObject bathroom;
    private GameObject _currentRoom;
    private string _mainSceneName = "MainScene";


    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != _mainSceneName) {
            DestoryCurrentRoom();
        }
    }

    //�ⲿ���ýӿ�
    public void ShowRoom(string roomName)
    {
        if (SceneManager.GetActiveScene().name != _mainSceneName) {
            Debug.LogWarning("ֻ�����������������ɵ�ͼ���ߣ�");
            return;
        }
        DestoryCurrentRoom();
        switch (roomName.ToLower()) {
            case "livingroom":
                if (livingroom != null) { 
                    _currentRoom = Instantiate(livingroom);
                }
                break;
            case "bathroom":
                if (livingroom != null)
                {
                    _currentRoom = Instantiate(bathroom);
                }
                break;
            default:
                Debug.LogError("�㴫�˸�ʲô���ɣ�");
                break;
        }
    }

    private void DestoryCurrentRoom()
    {
        if (_currentRoom != null)
        {
            Destroy(_currentRoom);
            _currentRoom = null;
        }
    }

    public bool IsInMainScene()
    {
        return SceneManager.GetActiveScene().name == _mainSceneName;
    }

    public void Init()
    {

    }
}
