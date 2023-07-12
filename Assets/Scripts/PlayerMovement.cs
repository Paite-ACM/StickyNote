using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum MovementState
{
    WALKING,
    SPRINTING,
    AIR,
    ZIP,
    WALL
}

public enum AimState
{
    NEUTRAL,
    AIMING
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float zipSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private Transform orientation;
    public Transform aimingPoint;

    [SerializeField] private float groundDrag;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;


    public MovementState state;

    private bool readyToJump = true;

    

    [SerializeField] private bool grounded;
    [SerializeField] private bool stuckToWall;
    [SerializeField] private float height;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask zippableWall;
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
        ZipMovement();

        if (state == MovementState.ZIP)
        {
            zipTimer += Time.deltaTime;
        }

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
        if (state != MovementState.ZIP && state != MovementState.WALL)
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
        if (state != MovementState.ZIP && state != MovementState.WALL)
        {
            exitingSlope = true;
            if (readyToJump && grounded)
            {
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
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // using transform.right because there's something off with the object's rotation i think
            rb.AddForce(transform.right * jumpForce, ForceMode.Impulse);

            state = MovementState.AIR;
            Invoke(nameof(ResetJump), jumpCooldown);
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

    // time to zip
    public void ZipToPosition()
    {
        // aiming raycast
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out zipHit) && aimingState == AimState.AIMING)
        {
            Debug.Log(zipHit.point);
            Debug.Log(maxZipDistance.position);
            StartCoroutine(ActivateZip());
        }
    }

    private IEnumerator ActivateZip()
    {
        // if the object is further away from the maxZipDistance obj then it won't let you do anything
        if (zipHit.point.x > maxZipDistance.position.x && zipHit.point.y > maxZipDistance.position.y && zipHit.point.z > maxZipDistance.position.z)
        {
            Debug.Log("too far");
            Debug.Log("ziphit = " + zipHit.point.x);
            state = MovementState.WALKING;
            yield return null;
        }

        // get off the ground if not already off the ground
        if (state != MovementState.AIR)
        {
            state = MovementState.AIR;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(0.1f);

        // get path between the player's position and the raycasts target
        float x = zipHit.point.x - transform.position.x;
        float y = zipHit.point.y - transform.position.y;
        float z = zipHit.point.z - transform.position.z;
        pathway = new Vector3(x, y, z);

        pathway = pathway.normalized;

        // zip to the desired position
        aimingState = AimState.NEUTRAL;
        state = MovementState.ZIP;
    }

    // what actually moves the player during a zip
    private void ZipMovement()
    {
        if (state != MovementState.ZIP)
        {
            return;
        }

        // moved from Update() for organisation
        if (!Physics.Raycast(transform.position, orientation.forward, 1f, ground))
        {
            // if the object is further away from the maxZipDistance obj then it won't let you do anything
            /*if (zipHit.point.x > maxZipDistance.position.x && zipHit.point.y > maxZipDistance.position.y && zipHit.point.z > maxZipDistance.position.z)
            {
                Debug.Log("too far");
                Debug.Log("ziphit = " + zipHit.point.x);
                state = MovementState.WALKING;
                return;
            }
            else
            {
                // move object
                transform.position += pathway * zipSpeed * Time.deltaTime;
                //rb.AddForce(pathway * zipSpeed * Time.deltaTime, ForceMode.Force);
            } */

            transform.position += pathway * zipSpeed * Time.deltaTime;
        }
        else
        {
            // check to make sure the player actually is still zipping
            if (state == MovementState.ZIP)
            {
                rb.velocity = new Vector3(0, 0, 0);
                rb.useGravity = false;
                state = MovementState.WALL;
                zipHit = new RaycastHit();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            Debug.Log("Collision with hazard");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Respawner")
        {
            // placeholder thing to reset if you fall through the ground
            SceneManager.LoadScene("SampleScene");
        }
    }
}
