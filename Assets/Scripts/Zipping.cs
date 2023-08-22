using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Zipping : MonoBehaviour
{

    // get distance between player obj and maxDistance obj
    // get distance between current zip target and player
    // if the distance between the zip target and the player is too large then cancel the zip


    [SerializeField] private PlayerMovement plrMovement;
    [SerializeField] private Transform maxZipDistance;
    //[SerializeField] LineRenderer lineRenderer;
    [SerializeField] private float zipSpeed;
    private Ray zipRay;
    private RaycastHit zipHit;
    Camera cam;
    private Vector3 pathway;

    private float zipTimer;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ZipMovement();
    }

    // time to zip
    // gets called by input system!!)
    public void ZipToPosition()
    {
        // aiming raycast
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out zipHit) && plrMovement.aimingState == AimState.AIMING)
        {
            Debug.Log(zipHit.point);
            Debug.Log(maxZipDistance.position);
            StartCoroutine(ActivateZip());
        }
    }

    private IEnumerator ActivateZip()
    {
        // d1 is the distance between the player object and the object representing the max zip range
        // d2 is the distance between the player and the current zip target
        var d1 = Vector3.Distance(transform.position, maxZipDistance.position);
        var d2 = Vector3.Distance(transform.position, zipHit.point);
        // if the object is further away from the maxZipDistance obj then it won't let you do anything
        if (d1 < d2)
        {
            Debug.LogWarning("too far");
            Debug.Log("ziphit = " + zipHit.point.x);
            zipHit = new RaycastHit();
            plrMovement.state = MovementState.WALKING;
            yield break;
            
        }
        else
        {
            // get off the ground if not already off the ground
            if (plrMovement.state != MovementState.AIR)
            {
                plrMovement.state = MovementState.AIR;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                rb.AddForce(transform.up * plrMovement.jumpForce, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(0.1f);

            // get path between the player's position and the raycasts target
            float x = zipHit.point.x - transform.position.x;
            float y = zipHit.point.y - transform.position.y;
            float z = zipHit.point.z - transform.position.z;
            pathway = new Vector3(x, y, z);

            pathway = pathway.normalized;

            // zip to the desired position
            if (plrMovement.state != MovementState.WALKING)
            {
                plrMovement.aimingState = AimState.NEUTRAL;
                plrMovement.state = MovementState.ZIP;
            }
            else
            {
                pathway = new Vector3(0, 0, 0);
            }

        }
    }

    // what actually moves the player during a zip
    private void ZipMovement()
    {
        if (plrMovement.state != MovementState.ZIP)
        {
            return;
        }

        if (plrMovement.state == MovementState.ZIP)
        {
            zipTimer += Time.deltaTime;
        }

        // moved from Update() for organisation
        if (!Physics.Raycast(transform.position, plrMovement.orientation.forward, 1f, plrMovement.ground))
        {
            if (pathway.x == 0f && pathway.y == 0f && pathway.z == 0f)
            {
                Debug.Log("Non-existent path");
                plrMovement.state = MovementState.WALKING;
                return;
            }

           /* 
            var d1 = Vector3.Distance(transform.position, maxZipDistance.position);
            var d2 = Vector3.Distance(transform.position, zipHit.point);

            // if the object is further away from the maxZipDistance obj then it won't let you do anything
            if (d1 > d2)
            {
                Debug.Log("too far");
                Debug.Log("ziphit = " + zipHit.point.x);
                plrMovement.state = MovementState.WALKING;
                return;
            }
            else
            {
                // move object
                transform.position += pathway * zipSpeed * Time.deltaTime;
                //rb.AddForce(pathway * zipSpeed * Time.deltaTime, ForceMode.Force);
            }  */

            transform.position += pathway * zipSpeed * Time.deltaTime;

        }
        else
        {
            // check to make sure the player actually is still zipping
            if (plrMovement.state == MovementState.ZIP)
            {
                rb.velocity = new Vector3(0, 0, 0);
                rb.useGravity = false;
                plrMovement.state = MovementState.WALL;
                zipHit = new RaycastHit();
            }
        }
    }
}
