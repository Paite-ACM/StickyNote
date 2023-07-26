using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Zipping : MonoBehaviour
{

    //assignables
    public Transform cameraTrans, player;
    public LineRenderer line;
    private SpringJoint grappleJoint;
    //variables for where the end of the grapple is and what the distance can be
    private Vector3 anchorPoint;
    private float grappleDistance = 50;
    //variables to check for grappling when its available
    public LayerMask grappleLayers;
    public bool isGrappling;

    private PlayerMovement plrMovement;

    void Awake()
    {
        plrMovement = GetComponent<PlayerMovement>();
        //line = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        line.enabled = false;
        isGrappling = false;
    }
    //input so the grapple can start. also needs to check if grappling since when the player dies the grapple still exists is its not checking for this bool

    // input code for the funny unity input system
    public void ZipStart(InputAction.CallbackContext context)
    {
        // can only start if cam is aiming
        if (plrMovement.aimingState == AimState.AIMING)
        {
            if (!isGrappling)
            {
                StartGrapple();
            }
        }


        // release key
        if (context.canceled)
        {
            StopGrapple();
        }
    }

    private void LateUpdate()//this is in late update as to not lag behind since it is only applied after all
    { //the calculations and not during so it looks a lot smoother then in the update function
        GrappleLine();
    }

    private void StartGrapple()
    {
        Debug.Log("StartGrapple called!");
        RaycastHit hitInfo;
        if (Physics.Raycast(cameraTrans.position, cameraTrans.forward, out hitInfo, grappleDistance, grappleLayers))
        {
            // change camera fov
            plrMovement.aimingState = AimState.ZIPPING;

            Debug.Log("Raycast hit!");
            isGrappling = true;
            //sets the anchor point to be the raycasthit point to give the grapple a way to determine where it should be connected to
            anchorPoint = hitInfo.point;
            grappleJoint = player.gameObject.AddComponent<SpringJoint>();
            //stops unity from auto configuring the anchor point so that I can set it to the anchorpoint from the hitInfo
            grappleJoint.autoConfigureConnectedAnchor = false;
            grappleJoint.connectedAnchor = anchorPoint;
            //finds the distance from the player to the anchor point to help calculate how close or far the player can be to the grapple point before it no longer works
            float distanceFromAnchor = Vector3.Distance(player.transform.position, anchorPoint);

            //the distacne that the grapple will try to keep from the anchorpoint
            grappleJoint.maxDistance = distanceFromAnchor * 0.4f;
            grappleJoint.minDistance = distanceFromAnchor * 0.01f;

            //changes the different effects of a spring joint to give different effects
            grappleJoint.spring = 4.5f;
            grappleJoint.massScale = 2.5f;
            grappleJoint.damper = 4.5f;

            line.enabled = true;
            line.positionCount = 2;
        }
    }
    //destroys the spring joint that was added to the player and also makes sure that the line render is no longer shown
    public void StopGrapple()
    {
        Debug.Log("StopGrapple() called!");
        line.positionCount = 0;
        plrMovement.aimingState = AimState.NEUTRAL;
        Destroy(grappleJoint);
        isGrappling = false;
    }
    private void GrappleLine()
    {
        //if not grappling then don't do anything but when there is then create a render of the line from player to anchor
        if (!grappleJoint)
        {
            return;
        }
        line.SetPosition(0, player.transform.position);
        line.SetPosition(1, anchorPoint);
    }
}
