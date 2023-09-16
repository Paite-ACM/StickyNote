using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum MovementState
{
    WALKING,
    SPRINTING,
    AIR,
    ZIP,
    WALL,
    DEATH
}

public enum AimState
{
    NEUTRAL,
    AIMING
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform playerObj;
    [SerializeField] private float speed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    public Transform orientation;
    public Transform aimingPoint;

    [SerializeField] private float groundDrag;

    public float jumpForce;
    [SerializeField] private float jumpForceDefault;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;

    // modifier for jumping off of a wall
    [SerializeField] private float wallJumpModifier;

    public MovementState state;

    private bool readyToJump = true;


    private bool groundedOnIce;

    [SerializeField] private bool grounded;
    [SerializeField] private bool stuckToWall;
    [SerializeField] private float height;
    public LayerMask ground;
    public LayerMask ice;
    [SerializeField] private LayerMask Hazard;

    [SerializeField] float maxSlopeAngle;
    private RaycastHit slopeHit;

    private float horizontal;
    private float vertical;

    private Vector3 moveDir;

    [SerializeField] private Rigidbody rb;

    private bool exitingSlope;

    public AimState aimingState;

    // zip related stuff
    [SerializeField] private Transform maxZipDistance;
    [SerializeField] LineRenderer lineRenderer;
    private Ray zipRay;
    private RaycastHit zipHit;
    Camera cam;
    private Vector3 pathway;

    private float zipTimer;

    private bool onJumpBoost;

    private float jumpBoostModifier;

    private void Start()
    {
        lineRenderer.enabled = false;
        rb.freezeRotation = true;
        cam = Camera.main;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Update()
    {

        // display/disable aiming line
        if (aimingState == AimState.NEUTRAL)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, aimingPoint.position);
            lineRenderer.SetPosition(1, maxZipDistance.position);
        }

        // check ground
        grounded = Physics.Raycast(transform.position, Vector3.down, height * 0.5f + 0.2f, ground);

        // ice ground
        groundedOnIce = Physics.Raycast(transform.position, Vector3.down, height * 0.5f + 0.2f, ice);
       
        SpeedControl();
        StateHandler();
        // handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
            
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void StateHandler()
    {
        // primarily just to check if airborne

        if (!grounded && state != MovementState.ZIP && state != MovementState.WALL)
        {
            state = MovementState.AIR;
        }

        if (grounded && state == MovementState.AIR)
        {
            state = MovementState.WALKING;
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (grounded)
        {
            if (context.canceled)
            {
                state = MovementState.WALKING;
                speed = walkSpeed;
            }
            else
            {
                state = MovementState.SPRINTING;
                speed = sprintSpeed;
            }
        }        
    }


    private void MovePlayer()
    {
        if (state != MovementState.ZIP && state != MovementState.WALL && state != MovementState.DEATH)
        {
            moveDir = orientation.forward * vertical + orientation.right * horizontal;

            // slope handling
            if (OnSlope())
            {
                rb.AddForce(GetSlopeMoveDirection() * speed * 20f, ForceMode.Force);

                if (rb.velocity.y > 0)
                {
                    // keep player on slope
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
                }
            }

            // on the ground
            if (grounded)
            {
                rb.AddForce(moveDir.normalized * speed * 10f, ForceMode.Force);
            }
            else
            {
                rb.AddForce(moveDir.normalized * speed * 10f * airMultiplier, ForceMode.Force);
            }


            // no more gravity while sloped
            rb.useGravity = !OnSlope();
        }
        else
        {
            return;
        }
        
    }


    public void UserInput()
    {
        // get inputs
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }


    // if this doesn't work then you probably didn't set the layer on the ground object
    public void Jump()
    {
        if (state != MovementState.ZIP && state != MovementState.WALL && state != MovementState.DEATH)
        {
            exitingSlope = true;
            if (readyToJump && grounded)
            {
                if (onJumpBoost)
                {
                    jumpForce *= jumpBoostModifier;
                }

                readyToJump = false;
                // reset y velocity
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

                Invoke(nameof(ResetJump), jumpCooldown);
            }
            
            // wow copying everything again i'm disappointed in you, me.
            if (readyToJump && groundedOnIce)
            {
                if (onJumpBoost)
                {
                    jumpForce *= jumpBoostModifier;
                }

                readyToJump = false;
                // reset y velocity
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
        
        if (state == MovementState.WALL)
        {
            readyToJump = false;
            // reset y velocity
            rb.velocity = new Vector3(rb.velocity.x, playerObj.rotation.y, rb.velocity.z);

            // using transform.right because there's something off with the object's rotation i think
            rb.AddForce(transform.up * (jumpForce * wallJumpModifier), ForceMode.Impulse);

            state = MovementState.AIR;
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // break out of zip
        if (state == MovementState.ZIP)
        {
             state = MovementState.AIR;
        }

    }

    // allows player to jump
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private void SpeedControl()
    {
        
        if (OnSlope() && !exitingSlope)
        {
            // slope speed limit
            if (rb.velocity.magnitude > speed)
            {
                rb.velocity = rb.velocity.normalized * speed;
            }
        }
        else
        {
            // speed limit on ground and air
            // flat velocity
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity
            if (flatVel.magnitude > speed)
            {
                Vector3 limitedVel = flatVel.normalized * speed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

    }

    private bool OnSlope()
    {
        // check if on a slope
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, height * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    // aiming
    public void AimIn(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            aimingState = AimState.NEUTRAL;
        }
        else
        {
            aimingState = AimState.AIMING; 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {


        switch (collision.gameObject.layer)
        {
            // hazard layer
            case 8:
                collision.gameObject.GetComponent<BoxCollider>().enabled = false;
                state = MovementState.DEATH;
                Debug.Log("Collision with hazard");
                GetComponent<Player>().PlayerDeath();
                break;
            // respawn layer
            case 10:
                // placeholder thing to reset if you fall through the ground
                SceneManager.LoadScene("MainScene");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter()");
        switch (other.gameObject.tag)
        {
            case "Collectable":
                Debug.Log("Collision!");
                switch (other.gameObject.GetComponent<Collectables>().colletableType)
                {
                    case CollectableType.COIN:
                        GetComponent<Player>().Score += other.gameObject.GetComponent<Collectables>().scoreToGive;
                        Destroy(other.gameObject);
                        break;
                }
                break;
            case "Checkpoint":
                // checkpoint animation
                other.gameObject.GetComponent<Checkpoint>().ActivateCheckpoint();
                // set saving location
                GetComponent<Player>().CurrentCheckPointX = other.gameObject.GetComponent<Checkpoint>().RespawnPosition.x;
                GetComponent<Player>().CurrentCheckPointY = other.gameObject.GetComponent<Checkpoint>().RespawnPosition.y;
                GetComponent<Player>().CurrentCheckPointZ = other.gameObject.GetComponent<Checkpoint>().RespawnPosition.z;
                GetComponent<Player>().HasReachedCheckpoint = true;
                // save
                GetComponent<Player>().SavePlayer();
                break;
            case "JumpBoost":
                Debug.Log("On jump boost");
                onJumpBoost = true;
                jumpBoostModifier = other.gameObject.GetComponent<JumpPad>().BoostMultiplier;
                break;
            case "Goal":
                GameObject uiObj = GameObject.Find("Canvas");
                uiObj.GetComponent<UIScript>().PlayerWinScreen();
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        switch (other.gameObject.tag)
        {
            case "JumpBoost":
                Debug.Log("Jelp");
                jumpBoostModifier = 0f;
                jumpForce = jumpForceDefault;
                onJumpBoost = false;
                break;
        }
    }
}
