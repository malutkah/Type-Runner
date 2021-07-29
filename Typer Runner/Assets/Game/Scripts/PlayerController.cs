using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float JumpForce = 2f;
    public float MoveSpeed = 10f;
    public int life = 3;
    public TimeManager timeManager;

    private Rigidbody2D rigidbody2D;

    private bool isDead = false;

    private Vector3 spawn;
    private Vector3 movement;

    private void Awake()
    {
        spawn = transform.position;
    }

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timeManager = GetComponent<TimeManager>();
    }

    private void Update()
    {
        if (life > 0 && !isDead)
        {
            movement = new Vector3(1, 0, 0) * Time.deltaTime * MoveSpeed;

            if (movement != new Vector3(0, 0, 0))
            {
                transform.position += movement;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                timeManager.DoSlowdown();
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Reset")
        {
            transform.position = spawn;
            life--;
            if (life <= 0)
                life = 0;

            isDead = life == 0 ? true : false;

            if (isDead)
                Debug.Log("You're dead!");
        }

        if (collision.tag == "SlowDown")
        {
            timeManager.slowdownFactor = 0.03f;
            timeManager.DoSlowdown();
        }
        
        if (collision.tag == "SlowDownWallRight")
        {
            movement = movement * -1;
            timeManager.slowdownFactor = 0.03f;
            timeManager.DoSlowdown();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "SlowDown")
        {
            timeManager.slowdownFactor = 1f;
            timeManager.DoSlowdown();
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
