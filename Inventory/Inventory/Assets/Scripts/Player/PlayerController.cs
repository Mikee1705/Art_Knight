using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft { get { return facingLeft; } }
    

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;


    public FixedJoystick joystick;
    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;

    private bool facingLeft = false;
    private bool isDashing = false;
    private float startingMoveSpeed;

    private Vector2 lastinput;
    public bool isFlip
    {
        get
        {
            return mySpriteRender.flipX;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();

        startingMoveSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Update()
    {
        lastinput = (joystick != null) ? joystick.Direction : movement;
        PlayerInput();
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }

    private void PlayerInput()
    {

        myAnimator.SetFloat("MoveX", lastinput.x);
        myAnimator.SetFloat("MoveY", lastinput.y);
    }

    private void Move()
    {
        rb.MovePosition(rb.position + lastinput * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        if (lastinput.magnitude > 0.1f)
        {
            facingLeft = lastinput.x <= 0;
            mySpriteRender.flipX = facingLeft;
        }
    }

    public Vector2 GetLastInput()
    {
        return lastinput;
    }

    private void Dash()
    {
        if (!isDashing)
        {
            isDashing = true;
            moveSpeed *= dashSpeed;
            myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }

    private IEnumerator EndDashRoutine()
    {
        float dashTime = 0.2f;
        float dashCD = 0.25f;
        yield return new WaitForSeconds(dashTime);
        moveSpeed = startingMoveSpeed;
        myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }

}
