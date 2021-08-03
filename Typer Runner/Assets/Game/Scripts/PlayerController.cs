using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float JumpForce = 2f;
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
    private bool startGame;
    private bool grounded;
    private bool respawned = false;
    private bool canDoFrontFlip = false;
    private bool isFalling = false;
    private bool canPickup = false;
    private bool pickedUp = false;

    [SerializeField]
    private int onWall = 0; // 0: not on wall; 1: in air; 2: right wall; 3: left wall

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
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timeManager = GetComponent<TimeManager>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        walljumpOn = false;
        startGame = false;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }
    private void Update()
    {
        Run();

        if (transform.position == spawn)
        {
            respawned = false;
        }

        if (rb.velocity.y < -3)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }

        if (isFalling)
        {
            Debug.Log("Player is Falling");
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
            //onWall = 2;

            transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, .0f));
        }

        if (collision.tag == "SlowDownWallLeft")
        {
            canInput = true;
            timeManager.slowdownFactor = 0.01f;
            timeManager.DoSlowdown();

            transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, -180.0f));

            walljumpOn = false;
            //onWall = 3;
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
                walljumpOn = false;
                respawned = true;
            }

        }

        if (collision.tag == "SlowDownWallRight")
        {
            animator.SetBool("onWall", true);
            onWall = 2;
        }

        if (collision.tag == "SlowDownWallLeft")
        {
            animator.SetBool("onWall", true);
            onWall = 3;
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

        if (collision.tag == "Pickup")
        {
            canPickup = true;
            PickedUpItem = collision.gameObject;
            timeManager.slowdownFactor = 0.03f;
            timeManager.DoSlowdown();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Contains("SlowDown"))
        {
            canInput = false;

            if (!grounded)
                onWall = 1;
            else
                onWall = 0;

            timeManager.slowdownFactor = 1f;
            timeManager.DoSlowdown();
        }

        if (collision.tag == "Pickup")
        {
            canPickup = false;

            if (!pickedUp)
                PickedUpItem = null;

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
                StartCoroutine(Slide());
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
            case string k when (k == "frontflip"):
                FrontFlip();
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
            yield return new WaitForSeconds(1.3f);
            animator.SetBool("doDash", false);

            MoveSpeed = tmpSpeed;
        }
    }

    private void FrontFlip()
    {
        if (grounded)
        {
            grounded = false;

            rb.velocity = Vector2.up * JumpForce * 1.25f;

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
        if (grounded || walljumpOn)
        {
            grounded = false;

            rb.velocity = Vector2.up * JumpForce;

            animator.SetBool("onGround", false);
            animator.SetBool("doJump", true);
        }
    }

    private IEnumerator Slide()
    {
        animator.SetBool("doSlide", true);

        Vector2 oldSize, oldOffset;
        SetNewColliderValues(out oldSize, out oldOffset);

        yield return new WaitForSeconds(SlideDuration);

        ResetColliderValues(oldSize, oldOffset);

        animator.SetBool("doSlide", false);
    }

    private void ResetColliderValues(Vector2 oldSize, Vector2 oldOffset)
    {
        boxCollider.size = oldSize;
        boxCollider.offset = oldOffset;

        circleCollider.offset = new Vector2(.2f, -.38f);
        circleCollider.radius = .04f;
    }

    private void SetNewColliderValues(out Vector2 oldSize, out Vector2 oldOffset)
    {
        oldSize = boxCollider.size;
        oldOffset = boxCollider.offset;
        Vector2 oldCCOffset = circleCollider.offset;
        float oldRadius = circleCollider.radius;

        boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y / 2);
        boxCollider.offset = new Vector2(boxCollider.offset.x, -0.3f);

        circleCollider.offset = new Vector2(.2f, -.38f);
        circleCollider.radius = .04f;
    }

    private void WallJump()
    {
        Debug.Log("Player Walljump");
    }
    #endregion

    #region Methods
    public void OnLanding()
    {
        animator.SetBool("onGround", true);
        animator.SetBool("doFrontFlip", false);
        animator.SetBool("doJump", false);
        animator.SetBool("onWall", false);
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
    #endregion

}
