using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float JumpForce = 2f;
    public float FrontFlipMultiplicator = 1.3f;
    public float MoveSpeed = 10f;
    public float FallMutliplier = 2.5f;
    public float LowJumpMutliplier = 2f;
    
    [Range(.5f, 3f)]
    public float SlideDuration = 1f;

    public int life = 3;

    public GameObject PickedUpItem;

    public TimeManager timeManager;

    public bool canInput = false;

    public Transform GroundCheck;

    public LayerMask WhatIsGround;

    private Rigidbody2D rb;

    private bool isDead = false;
    private bool walljumpOn;
    private bool onWallLeft = false;
    private bool startGame;
    private bool grounded;
    private bool respawned = false;
    private bool isFalling = false;
    private bool canPickup = false;
    private bool pickedUp = false;

    private float groundedRadius = 1.3f;

    private Vector3 spawn;
    private Vector3 movement;

    private Animator animator;

    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;

    #region Unity

    private void Awake()
    {
        spawn = transform.position;

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        startGame = false;
        walljumpOn = false;

        rb = GetComponent<Rigidbody2D>();
        timeManager = GetComponent<TimeManager>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (startGame)
            Run();

        if (transform.position == spawn)
        {
            respawned = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "SlowDownWallRight")
        {
            //canInput = true;
            DoSlowdown(.01f);

            transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, -180f));

            walljumpOn = true;
        }

        if (collision.tag == "SlowDownWallLeft")
        {
            //canInput = true;
            DoSlowdown(.01f);

            transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, .0f));

            walljumpOn = false;
            onWallLeft = true;
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
            }
            else
            {
                transform.position = spawn;
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, .0f));

                walljumpOn = false;
                respawned = true;
            }

        }

        if (collision.tag == "SlowDownWallRight" || collision.tag == "SlowDownWallLeft")
        {
            animator.SetBool("onWall", true);
        }

        if (collision.tag == "ResetSpeed")
        {
            MoveSpeed = 5;
        }

        if (collision.tag.Contains("SlowDown"))
        {
            canInput = true;
            DoSlowdown(.03f);
        }

        if (collision.tag == "Pickup")
        {
            canPickup = true;
            PickedUpItem = collision.gameObject;
            DoSlowdown(.03f);
        }

        if (collision.tag == TagConstants.ChangeLevel)
        {
            startGame = false;
            SceneManager.LoadScene("Main 2");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Contains("SlowDown") && !collision.tag.Contains("SlowDownWall"))
        {
            transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, .0f));
            DoSlowdown(1f);
        }

        if (collision.tag == "Pickup")
        {
            canPickup = false;

            if (!pickedUp)
                PickedUpItem = null;

            DoSlowdown(1f);
        }
    }

    #endregion

    #region Action Methods
    public void DoAction(string actionText)
    {
        switch (actionText)
        {
            case string k when (k.Contains(ActionConstants.jump[0]) || k == ActionConstants.jump[1]):
                Jump();
                break;
            case string k when (k == ActionConstants.slide[0] || k == ActionConstants.slide[1]):
                StartCoroutine(Slide());
                break;
            case string k when (k == ActionConstants.reset[0] || k == ActionConstants.reset[1] || k == ActionConstants.reset[2] || k == ActionConstants.reset[3]):
                Restart();
                break;
            case string k when (k == "go" || k == "start"):
                StartGame();
                break;
            case string k when (k == "frontflip"):
                Jump(FrontFlipMultiplicator);
                break;
            case string k when (k == "dash" || k == "sprint"):
                StartCoroutine(Dash());
                break;
            case string k when (k == "get" || k == "pickup"):
                Pickup();
                break;
            case string k when (k == "yeet" || k == "throw"):
                StartCoroutine(Throw());
                break;
        }
    }

    private IEnumerator Throw()
    {
        Rigidbody2D itemRb = PickedUpItem.GetComponent<Rigidbody2D>();

        itemRb.velocity = Vector2.right * 10f;

        yield return new WaitForSeconds(1f);

        Destroy(PickedUpItem);
    }

    private void Pickup()
    {
        if (canPickup)
        {
            pickedUp = true;

            Debug.Log($"Player picked up {PickedUpItem.name}");

            PickedUpItem.transform.parent = transform;
            PickedUpItem.transform.localPosition = new Vector3(0, 0);
        }
    }

    private IEnumerator Dash()
    {
        if (grounded)
        {
            float tmpSpeed = MoveSpeed;
            MoveSpeed *= 1.3f;

            animator.SetBool("doDash", true);

            yield return new WaitForSeconds(1f);

            animator.SetBool("doDash", false);

            MoveSpeed = tmpSpeed;
        }
    }

    private void Restart()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("Main");
    }

    private void StartGame()
    {
        startGame = true;
    }

    private void Jump(float frontFlipMultiplicator = 1)
    {
        if (grounded || walljumpOn || onWallLeft)
        {
            grounded = false;
            onWallLeft = false;

            rb.velocity = Vector2.up * JumpForce * frontFlipMultiplicator;

            animator.SetBool("onGround", false);

            if (frontFlipMultiplicator > 1)
                animator.SetBool("doFrontFlip", true);
            else
                animator.SetBool("doJump", true);
        }
    }

    private IEnumerator Slide()
    {
        Vector2 oldSize, oldOffset, oldCCOffset;
        float oldRadius;
        SetOldColliderValues(out oldSize, out oldOffset, out oldCCOffset, out oldRadius);

        SetNewColliderValues();

        animator.SetBool("doSlide", true);

        yield return new WaitForSeconds(SlideDuration);

        ResetColliderValues(oldSize, oldOffset, oldCCOffset, oldRadius);

        animator.SetBool("doSlide", false);
    }
    private void WallJump()
    {
        Debug.Log("Player Walljump");
    }
    #endregion

    #region Methods

    private void SetOldColliderValues(out Vector2 oldSize, out Vector2 oldOffset, out Vector2 oldCCOffset, out float oldRadius)
    {
        oldSize = boxCollider.size;
        oldOffset = boxCollider.offset;
        oldCCOffset = circleCollider.offset;
        oldRadius = circleCollider.radius;
    }

    private void ResetColliderValues(Vector2 oldSize, Vector2 oldOffset, Vector2 oldCCOffset, float oldRadius)
    {
        boxCollider.size = oldSize;
        boxCollider.offset = oldOffset;

        circleCollider.offset = oldCCOffset;
        circleCollider.radius = oldRadius;
    }

    private void SetNewColliderValues()
    {
        boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y / 2);
        boxCollider.offset = new Vector2(boxCollider.offset.x, -0.3f);

        circleCollider.offset = new Vector2(.2f, -.38f);
        circleCollider.radius = .04f;
    }

    private void DoSlowdown(float slowdownFactor)
    {
        timeManager.slowdownFactor = slowdownFactor;
        timeManager.DoSlowdown();
    }

    public void OnLanding()
    {
        animator.SetBool("onGround", true);
        animator.SetBool("doFrontFlip", false);
        animator.SetBool("doJump", false);
        animator.SetBool("onWall", false);
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
        if (life > 0 && !isDead && !walljumpOn)
        {
            movement = new Vector3(1, 0, 0) * MoveSpeed * Time.deltaTime;

            if (movement != new Vector3(0, 0, 0))
            {
                transform.position += movement;
            }
        }

        if (life > 0 && !isDead && walljumpOn)
        {
            movement = new Vector3(-1, 0, 0) * MoveSpeed * Time.deltaTime;

            if (movement != new Vector3(0, 0, 0))
            {
                transform.position += movement;
            }
        }
    }
    
    public void LoadedNewScene()
    {
        startGame = false;
        walljumpOn = false;
        grounded = true;
    }
    
    #endregion

}
