using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;

    public float JumpForce = 2f;
    public float MoveSpeed = 10f;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var movement = new Vector3(1, 0, 0) * Time.deltaTime * MoveSpeed;

        if (movement != new Vector3(0, 0, 0))
        {
            transform.position += movement;
        }
    }

    public void DoAction(string actionText)
    {      
        switch (actionText)
        {
            case string k when (k == "jump" || k == "Jump"):
                Jump();
                break;
            case string k when (k == "slide" || k == "Slide"):
                Slide();
                break;
            case string k when (k == "walljump" || k == "Walljump"):
                WallJump();
                break;
        }

    }

    private void Jump()
    {
        rigidbody2D.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
    }

    private void Slide()
    {
        Debug.Log("Player Slide");
    }

    private void WallJump()
    {
        Debug.Log("Player Walljump");
    }
}
