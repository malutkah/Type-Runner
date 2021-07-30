using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float JumpForce = 2f;
    public float MoveSpeed = 10f;
    public int life = 3;
    public TimeManager timeManager;
    public bool canInput = false;
    public Transform GroundCheck;
    public LayerMask WhatIsGround;

    private Rigidbody2D rigidbody2D;

    private bool isDead = false;
    private bool walljumpOn;
    private bool startGame;
    private bool grounded;

    private float groundedRadius = 1.5f;

    private Vector3 spawn;
    private Vector3 movement;

    private Animator animator;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    #region Unity
    private void Awake()
    {
        spawn = transform.position;
        OnLandEvent = new UnityEvent();
    }

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timeManager = GetComponent<TimeManager>();
        animator = GetComponent<Animator>();

        walljumpOn = false;
        startGame = false;
    }

    private void FixedUpdate()
    {
        bool wasGrounded = grounded;
        grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, groundedRadius, WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    private void Update()
    {
        //if (startGame)

        if (life > 0 && !isDead && !walljumpOn)
        {
            movement = new Vector3(1, 0, 0) * Time.deltaTime * MoveSpeed;

            if (movement != new Vector3(0, 0, 0))
            {
                transform.position += movement;
            }
        }

        if (life > 0 && !isDead && walljumpOn)
        {
            movement = new Vector3(-1, 0, 0) * Time.deltaTime * MoveSpeed;

            if (movement != new Vector3(0, 0, 0))
            {
                transform.position += movement;
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "SlowDownWallRight")
        {
            canInput = true;
            timeManager.slowdownFactor = 0.01f;
            timeManager.DoSlowdown();
            walljumpOn = true;
        }

        if (collision.tag == "SlowDownWallLeft")
        {
            canInput = true;
            timeManager.slowdownFactor = 0.01f;
            timeManager.DoSlowdown();
            walljumpOn = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Reset")
        {
            life--;

            if (life <= 0)
                life = 0;

            isDead = life == 0 ? true : false;

            startGame = false;

            if (isDead)
            {
                Debug.Log("You're dead!");
            }
            else
            {
                transform.position = spawn;
                walljumpOn = false;
            }

        }

        if (collision.tag == "SlowDown")
        {
            canInput = true;
            timeManager.slowdownFactor = 0.03f;
            timeManager.DoSlowdown();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Contains("SlowDown"))
        {
            canInput = false;
            timeManager.slowdownFactor = 1f;
            timeManager.DoSlowdown();
        }
    }

    #endregion

    #region Action Methods
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
            case string k when (k == "reset" || k == "reload" || k == "restart"):
                Restart();
                break;
            case string k when (k == "go" || k == "start"):
                StartGame();
                break;
        }

    }

    private void Restart()
    {
        SceneManager.LoadScene("Main");
    }

    private void StartGame()
    {
        startGame = true;
    }

    private void Jump()
    {
        if (grounded)
        {
            grounded = false;
            rigidbody2D.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
            animator.SetBool("doJump", true);
        }
    }

    private void Slide()
    {
        Debug.Log("Player Slide");
    }

    private void WallJump()
    {
        Debug.Log("Player Walljump");
    }
    #endregion

    public void OnLanding()
    {
        Debug.Log("Landed");

        animator.SetBool("doJump", false);
        animator.SetBool("isFalling", false);
        animator.SetBool("hasLanded", true);
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;

        scale.x *= -1;

        transform.localScale = scale;
    }
}
