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
    public float slideSpeed = 1;
    public float wallJumpLerp = 5;
    public float wallJumpMultiplier = 1.25f;
    public float dashSpeed = 8;
    public float defaultGravity = 2;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
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

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        godMode = Input.GetKey(KeyCode.G);

        Walk(dir);
        //anim.SetHorizontalMovement(x, y, rb.velocity.y);

        if (coll.onWall && Input.GetButton("Fire3") && canMove && !coll.onGround)
        {
            //if (side != coll.wallSide)
            //anim.Flip(side * -1);
            wallGrab = true;
            wallSlide = false;
        }

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }

        // MARK: Wall climbing
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if (x > .2f || x < -.2f)
                rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .6f : 1;

            // NOTE: Making x param = 0 fixed the weird wall climb bug
            rb.velocity = new Vector2(0, y * (speed * speedModifier)); // Add platform velocity 
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }

        // MARK: Wall sliding
        if (coll.onWall && !coll.onGround)
        {
            // TODO: Put somewhere else
            // Reset double jump
            this.hasDoubleJumped = false;

            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            wallSlide = false;

        // MARK: Jumping, Wall jumping
        if (Input.GetButtonDown("Jump"))
        {
            ////anim.SetTrigger("jump");

            if (coll.onGround) // Normal jump
                Jump(Vector2.up, false);
            if (coll.onWall && !coll.onGround) // Wall jump
                WallJump();
            if (!coll.onWall && !coll.onGround && !hasDoubleJumped) // Double jump
            {
                Jump(Vector2.up, false);
                hasDoubleJumped = true;
            }

            if (!godMode) return;
            GodModeJump();
        }

        // Reset atrributes on groundtouch
        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        WallParticle(y);

        // Don't flip sprites if conditions:
        if (wallGrab || wallSlide || !canMove)
            return;

        if (x > 0)
        {
            side = 1;
            //anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            //anim.Flip(side);
        }
    }

    void GroundTouch()
    {
        hasDoubleJumped = false;

        //side = anim.sr.flipX ? -1 : 1;

        //jumpParticle.Play();
    }

    // MARK: Wall jump
    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            //anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Vector2 jumpDirection = new Vector2(wallDir.x + .5f, .5f).normalized * wallJumpMultiplier;
        Jump(jumpDirection, true);

        wallJumped = true;
    }

    private void WallSlide()
    {
        if (coll.wallSide != side)
            //anim.Flip(side * -1);

            if (!canMove)
                return;

        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
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

        if (wallSlide || (wallGrab && vertical < 0))
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
