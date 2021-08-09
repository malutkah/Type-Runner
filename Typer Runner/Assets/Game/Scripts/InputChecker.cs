using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputChecker : MonoBehaviour
{
    public GameObject Player;
    public InputField actionInput;
    public bool DemoMode = false;

    public GameManager manager;

    private string actionText;
    private PlayerController controller;
    private PlayerController_Demo demo_controller;

    //private bool canInput;

    void Start()
    {
        controller = Player.GetComponent<PlayerController>();
        demo_controller = Player.GetComponent<PlayerController_Demo>();

        actionInput = GetComponent<InputField>();
        actionInput.Select();

        actionText = "";

        //canInput = controller.canInput;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //canInput = controller.canInput;
            ExecuteAction();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            actionInput.text = actionText;
        }
    }

    private void ExecuteAction()
    {
        if (actionInput.text != "") //&& canInput)
        {
            actionText = actionInput.text.Remove(actionInput.text.Length - 1);

            if (DemoMode)
            {
                demo_controller.timeManager.slowdownFactor = 1;
                demo_controller.timeManager.DoSlowdown();
                demo_controller.DoAction(actionText);

            }
            else
            {
                controller.timeManager.slowdownFactor = 1;
                controller.timeManager.DoSlowdown();
                controller.DoAction(actionText);

                manager.Score += GetActionLength() * 10;
            }

            actionInput.text = string.Empty;
        }
    }

    public int GetActionLength()
    {
        return actionText.Length;
    }

    public string GetAction()
    {
        return actionText;
    }
}
