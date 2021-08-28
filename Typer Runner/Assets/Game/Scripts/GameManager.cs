using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject Player, MainCam, EventSystem;

    public Canvas Canvas;

    public InputField commandField;


    public TextMeshProUGUI ScoreText;

    public int Score;

    private InputChecker inputChecker;
    private GameObject SpawnMap2;
    private PlayerController playerController;

    #region Unity
    private void Awake()
    {
        Score = 0;
        ScoreText.text = Score.ToString();

        KeepInScenes();

        inputChecker = commandField.GetComponent<InputChecker>();
        playerController = Player.GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void Update()
    {
        ScoreText.text = Score.ToString();
    }

    #endregion

    #region Methods

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"OnSceneLoaded: {scene.name}");

        if (scene.name == "Main 2")
        {
            SpawnMap2 = GameObject.Find("SpawnMap2");
            //Player.transform.localPosition = new Vector3(0f, 0f, 0f);
            Player.transform.localPosition = SpawnMap2.transform.localPosition;
            playerController.LoadedNewScene();
        }
    }

    private void KeepInScenes()
    {
        // game manager
        DontDestroyOnLoad(gameObject);

        // player
        DontDestroyOnLoad(Player);
        DontDestroyOnLoad(EventSystem);

        // canvas
        DontDestroyOnLoad(Canvas);

        // camera
        DontDestroyOnLoad(MainCam);
    }

    #endregion
}
