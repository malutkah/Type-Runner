using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject Player, MainCam;

    public Canvas Canvas;

    public InputField commandField;

    public TextMeshProUGUI ScoreText;

    public int Score;

    private InputChecker inputChecker;

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
            playerController.LoadedNewScene();
            Player.transform.position = new Vector3(-119.0618f, 38.53685f, .0f);
        }
    }

    private void KeepInScenes()
    {
        // game manager
        DontDestroyOnLoad(gameObject);

        // player
        DontDestroyOnLoad(Player);

        // canvas
        DontDestroyOnLoad(Canvas);

        // camera
        DontDestroyOnLoad(MainCam);
    }

    #endregion
}
