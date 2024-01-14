using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    //private AnimationScript anim;

    [Space]
    [Header("Stats")]
    public float speed = 4;
    public float jumpForce = 7;
    public float slideSpeed = 2f;
    public float wallJumpLerp = 5;
    public float wallJumpMultiplier = 1.25f;
    public float dashSpeed = 8;
    public float defaultGravity = 2;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    [Space]
    private bool groundTouch;
    private bool hasDoubleJumped;

    public int side = 1;

    [Space]
    [Header("Playtest settings")]
    public bool godMode;
    public float godJumpForce = 15;

    // Wall Jumping
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(9f, 4f);

    private float horizontal;
    private float vertical;

    //[Space]
    //[Header("Polish")]
    // public ParticleSystem dashParticle;
    // public ParticleSystem wallJumpParticle;
    // public ParticleSystem jumpParticle;
    // public ParticleSystem slideParticle;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponentInChildren<AnimationScript>();
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        godMode = Input.GetKey(KeyCode.G);

        //Walk(dir);

        // MARK: Wall sliding
        WallSlide();
        WallJump();

        // MARK: Jumping, Wall jumping
        if (Input.GetButtonDown("Jump"))
        {

            if (coll.onGround) // Normal jump
                Jump(Vector2.up, false);
            // if (!coll.onWall && !coll.onGround && !hasDoubleJumped) // Double jump
            // {
            //     Jump(Vector2.up, false);
            //     hasDoubleJumped = true;
            // }

            if (!godMode) return;
            GodModeJump();
        }


        // Reset atrributes on groundtouch
        // if (coll.onGround && !groundTouch)
        // {
        //     GroundTouch();
        //     groundTouch = true;
        // }

        // if (!coll.onGround && groundTouch)
        // {
        //     groundTouch = false;
        // }

        // WallParticle(vertical);

        // // Don't flip sprites if conditions:
        // if (wallSlide || !canMove)
        //     return;

        // if (horizontal > 0)
        // {
        //     side = 1;
        //     //anim.Flip(side);
        // }
        // if (horizontal < 0)
        // {
        //     side = -1;
        //     //anim.Flip(side);
        // }
    }

    void GroundTouch()
    {
        hasDoubleJumped = false;

        //side = anim.sr.flipX ? -1 : 1;

        //jumpParticle.Play();
    }

    private void WallSlide()
    {
        if (coll.onWall && !coll.onGround && horizontal != 0f)
        {
            this.hasDoubleJumped = false;
            wallSlide = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -slideSpeed, float.MaxValue));
            Debug.Log("Wall Sliding");

            // if (coll.wallSide != side)
            //     //anim.Flip(side * -1);

            //     if (!canMove)
            //         return;

            // bool pushingWall = false;
            // if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
            // {
            //     pushingWall = true;
            // }
            // float push = pushingWall ? 0 : rb.velocity.x;

            // rb.velocity = new Vector2(push, -slideSpeed);
        } 
        else
        {
            wallSlide = false;
        }
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove || isWallJumping)
            return;

        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        //slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        //ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        //particle.Play();
    }

    private void WallJump()
    {
        if (wallSlide) 
        {
            isWallJumping = false;
            wallJumpingDirection = coll.onRightWall ? Vector2.left.x : Vector2.right.x;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f) 
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            // TODO: Flip Player
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
        // if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        // {
        //     side *= -1;
        //     //anim.Flip(side);
        // }

        // StopCoroutine(DisableMovement(0));
        // StartCoroutine(DisableMovement(.1f));

        // Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;
        // wallJumpingCounter = wallJumpingTime;    


        // //Vector2 jumpDirection = new Vector2(wallDir.x + .5f, .5f).normalized * wallJumpMultiplier;
        // rb.velocity = new Vector2(wallDir.x * wallJumpingPower.x, wallJumpingPower.y);
        // //Jump(jumpDirection, true);

        // wallJumped = true;
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    // God mode
    private void GodModeJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * (godJumpForce);
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        rb.gravityScale = 0;
        yield return new WaitForSeconds(time);
        canMove = true;
        rb.gravityScale = defaultGravity;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    void WallParticle(float vertical)
    {
        // var main = slideParticle.main;

        if (wallSlide || vertical < 0)
        {
            // slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            // main.startColor = Color.white;
        }
        else
        {
            //main.startColor = Color.clear;
        }
    }

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }
}
