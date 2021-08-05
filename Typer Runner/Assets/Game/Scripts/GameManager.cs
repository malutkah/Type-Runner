using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public InputField commandField;

    public TextMeshProUGUI ScoreText;

    public int Score;

    private InputChecker inputChecker;

    #region Unity
    private void Awake()
    {
        Score = 0;
        ScoreText.text = Score.ToString();

        inputChecker = commandField.GetComponent<InputChecker>();
    }

    private void Update()
    {
        ScoreText.text = Score.ToString();
    }

    #endregion

}
