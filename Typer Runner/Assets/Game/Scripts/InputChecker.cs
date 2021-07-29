using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputChecker : MonoBehaviour
{
    public GameObject Player;
    public InputField actionInput;

    private string actionText;
    private PlayerController controller;

    void Start()
    {
        controller = Player.GetComponent<PlayerController>();
        actionInput = GetComponent<InputField>();
        actionInput.Select();
        actionText = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (actionInput.text != "")
            {
                actionText = actionInput.text.Remove(actionInput.text.Length - 1);

                controller.timeManager.slowdownFactor = 1;
                controller.timeManager.DoSlowdown();
                controller.DoAction(actionText);


                actionInput.text = string.Empty;


            }

        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            actionInput.text = actionText;
        }
    }
}
