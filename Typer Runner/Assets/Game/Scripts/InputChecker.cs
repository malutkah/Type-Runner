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

    private bool canInput;

    void Start()
    {
        controller = Player.GetComponent<PlayerController>();
        
        actionInput = GetComponent<InputField>();
        actionInput.Select();
        
        actionText = "";

        canInput = controller.canInput;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            canInput = controller.canInput;
            ExecuteAction();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            actionInput.text = actionText;
        }
    }

    private void ExecuteAction()
    {
        if (actionInput.text != "" && canInput)
        {         
            actionText = actionInput.text.Remove(actionInput.text.Length - 1);

            controller.timeManager.slowdownFactor = 1;
            controller.timeManager.DoSlowdown();
            controller.DoAction(actionText);


            actionInput.text = string.Empty;
        }
    }
}
