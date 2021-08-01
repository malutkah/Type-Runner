using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController_Demo : MonoBehaviour
{
    public float JumpForce = 2f;
    public float MoveSpeed = 10f;
    public int life = 3;
    public TimeManager timeManager;
    //public bool canInput = false;
    public Transform GroundCheck;
    public LayerMask WhatIsGround;

    private Rigidbody2D rigidbody2D;

    private bool isDead = false;
    private bool walljumpOn;
    private bool startGame;
    private bool grounded;
    private bool respawned = false;

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

        if (OnLandEvent == null)
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
        if (transform.position == spawn)
        {
            respawned = false;
        }

    }

    #endregion

    #region Action Methods
    public void DoAction(string actionText)
    {
        switch (actionText)
        {
            case string k when ( k == "jump" || k == "Jump" ):
            Jump();
            break;
            case string k when ( k == "slide" || k == "Slide" ):
            Slide();
            break;
            case string k when ( k == "walljump" || k == "Walljump" ):
            WallJump();
            break;
            case string k when ( k == "reset" || k == "reload" || k == "restart" ):
            Restart();
            break;
            case string k when ( k == "go" || k == "start" ):
            StartGame();
            break;
        }

    }

    private void Restart()
    {
        SceneManager.LoadScene("Test");
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
            animator.SetBool("onGround", false);
        }
    }

    private void Slide()
    {
        animator.SetBool("doSlide", true);
    }

    private void WallJump()
    {
        Debug.Log("Player Walljump");
    }
    #endregion

    public void OnLanding()
    {
        animator.SetBool("onGround", true);
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;

        scale.x *= -1;

        transform.localScale = scale;
    }
}
