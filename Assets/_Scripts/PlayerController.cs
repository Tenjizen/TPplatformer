using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Movements")]
    [SerializeField] float walkspeed;
    [SerializeField] float acceleration;
    [Space]
    [Header("GroundCheck")]
    [SerializeField] float groundOffset;
    [SerializeField] float groundRadius;
    [SerializeField] LayerMask groundLayer;
    [Space]
    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float timeMinBetweenJump;
    [SerializeField] float velocityFallMin;
    [SerializeField] [Tooltip("Gravity otherwise")] float gravity;
    [SerializeField] [Tooltip("Gravity when the player goes up and press jump")] float gravityUpJump;
    [SerializeField] float jumpInputTimer = 0.1f;
    [Space]
    private Vector2 inputs;
    private bool inputJump;
    private Rigidbody2D rb;
    private float timerNoJump;
    private bool isGrounded;
    private Collider2D[] colliderGround = new Collider2D[1];
    private float timeSinceJumpPressed;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();


    }

    private void FixedUpdate()
    {
        HandleGrounded();
        HandleMovements();
        HandleJump();
    }
    void HandleInputs()
    {
        inputs.x = Input.GetAxisRaw("Horizontal");
        inputs.y = Input.GetAxisRaw("Vertical");

        inputJump = Input.GetKey(KeyCode.UpArrow);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            timeSinceJumpPressed = 0;
    }
    void HandleMovements()
    {
        var velocity = rb.velocity;
        Vector2 wantVelocity = new Vector2(inputs.x * walkspeed, velocity.y);
        rb.velocity = Vector2.MoveTowards(velocity, wantVelocity, acceleration * Time.deltaTime);
    }
    void HandleJump()
    {
        timerNoJump -= Time.deltaTime;
        if (inputJump && rb.velocity.y <= 0 && isGrounded && timerNoJump <= 0 && timeSinceJumpPressed < jumpInputTimer)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            timerNoJump = timeMinBetweenJump;
        }

        if (!isGrounded)
        {
            if (rb.velocity.y < 0)
                rb.gravityScale = gravity;
            else
                rb.gravityScale = inputJump ? gravityUpJump : gravity;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if (rb.velocity.y < velocityFallMin)
            rb.velocity = new Vector2(rb.velocity.x, velocityFallMin);



        timeSinceJumpPressed += Time.deltaTime;
    }
    void HandleGrounded()
    {
        Vector2 point = transform.position + Vector3.up * groundOffset;
        bool currentGrounded = Physics2D.OverlapCircleNonAlloc(point, groundRadius, colliderGround, groundLayer) > 0;
        isGrounded = currentGrounded;
    }
}
