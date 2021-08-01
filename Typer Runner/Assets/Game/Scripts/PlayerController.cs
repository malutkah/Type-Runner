using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float JumpForce = 2f;
    public float MoveSpeed = 10f;
    public float slopeCheckDistance;

    public int life = 3;

    public TimeManager timeManager;

    public bool canInput = false;

    public Transform GroundCheck;

    public LayerMask WhatIsGround;

    private Rigidbody2D rb;

    private bool isDead = false;
    private bool walljumpOn;
    private bool startGame;
    private bool grounded;
    private bool respawned = false;
    private bool canDoFrontFlip = false;
    private bool isOnSlope;
    private bool canWalkOnSlope;

    private float maxSlopeAngle;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;
    private float groundedRadius = 1.5f;

    private Vector2 newVelocity;
    private Vector2 newForce;
    private Vector2 colliderSize;
    private Vector2 slopeNormalPerp;

    private Vector3 spawn;
    private Vector3 movement;

    private Animator animator;

    private CapsuleCollider2D capsuleCollider;

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
        rb = GetComponent<Rigidbody2D>();
        timeManager = GetComponent<TimeManager>();
        animator = GetComponent<Animator>();

        walljumpOn = false;
        startGame = false;
    }

    private void FixedUpdate()
    {
        CheckGround();
        SlopeCheck();
    }
    private void Update()
    {
        //if (startGame)

        if (transform.position == spawn)
        {
            respawned = false;
        }

        Run();

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
        if (collision.tag == "Reset" && !respawned)
        {
            life--;

            if (life <= 0)
                life = 0;

            isDead = life == 0 ? true : false;

            startGame = false;

            if (isDead)
            {
                Debug.Log("You're dead!");
            } else
            {
                transform.position = spawn;
                walljumpOn = false;
                respawned = true;
            }

        }

        if (collision.tag == "ResetSpeed")
        {
            MoveSpeed = 5;
        }

        if (collision.tag == "SlowDown")
        {
            canInput = true;
            timeManager.slowdownFactor = 0.03f;
            timeManager.DoSlowdown();
        }

        if (collision.tag == "SlowDonwFF")
        {
            canDoFrontFlip = true;
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
            case string k when ( k == "frontflip" ):
            FrontFlip();
            break;

        }

    }

    private void FrontFlip()
    {
        if (grounded)
        {
            grounded = false;

            //newVelocity.Set(0.0f, 0.0f);
            //rb.velocity = newVelocity;
            //newForce.Set(0.0f, JumpForce * 1.25f);
            rb.AddForce(new Vector2(0, JumpForce * 1.25f), ForceMode2D.Impulse);
            
            animator.SetBool("onGround", false);
            animator.SetBool("doFrontFlip", true);
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

            //newVelocity.Set(0.0f, 0.0f);
            //rb.velocity = newVelocity;
            //newForce.Set(0.0f, JumpForce);
            rb.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);

            animator.SetBool("onGround", false);
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
        animator.SetBool("onGround", true);
        animator.SetBool("doFrontFlip", false);
        animator.SetBool("doJump", false);
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;

        scale.x *= -1;

        transform.localScale = scale;
    }

    private void CheckGround()
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

    private void Run()
    {
        if (life > 0 && !isDead && !walljumpOn && !isOnSlope)
        {
            //movement = new Vector3(1, 0, 0) * Time.deltaTime * MoveSpeed;

            //if (movement != new Vector3(0, 0, 0))
            //{
            //    transform.position += movement;
            //}

            newVelocity.Set(MoveSpeed * 1, 0f);
            rb.velocity = newVelocity;

        } else if (!isDead && !walljumpOn && isOnSlope)
        {
            newVelocity.Set(MoveSpeed * slopeNormalPerp.x * -1, MoveSpeed * slopeNormalPerp.y * -1);
            rb.velocity = newVelocity;
        }

        if (life > 0 && !isDead && walljumpOn)
        {
            //movement = new Vector3(-1, 0, 0) * Time.deltaTime * MoveSpeed;

            //if (movement != new Vector3(0, 0, 0))
            //{
            //    transform.position += movement;
            //}

            newVelocity.Set(MoveSpeed * 1, 0f);
            rb.velocity = newVelocity;

        } else if (!isDead && !walljumpOn && isOnSlope)
        {
            newVelocity.Set(MoveSpeed * slopeNormalPerp.x * -1, MoveSpeed * slopeNormalPerp.y * -1);
            rb.velocity = newVelocity;
        }
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3) ( new Vector2(0.0f, colliderSize.y / 2) );

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, WhatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, WhatIsGround);

        if (slopeHitFront)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);

        } else if (slopeHitBack)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        } else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }

    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, WhatIsGround);

        if (hit)
        {

            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }

            lastSlopeAngle = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        } else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && canWalkOnSlope)
        {
            //rb.sharedMaterial = fullFriction;
        } else
        {
            //rb.sharedMaterial = noFriction;
        }
    }

}
