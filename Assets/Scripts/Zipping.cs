using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Zipping : MonoBehaviour
{

    [SerializeField] private PlayerMovement plrMovement;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform zipAimer;
    [SerializeField] private LayerMask zippable;
    [SerializeField] private LineRenderer lr;

    [SerializeField] private float maxZipDistance;
    [SerializeField] private float zipDelay;
    [SerializeField] private float overshootYAxis;

    private Vector3 zipPoint;

    [SerializeField] private float zipCooldown;
    [SerializeField] private float zipCooldownTimer;

    [SerializeField] private bool zipping;

    private void Update()
    {
        if (zipCooldownTimer > 0)
        {
            zipCooldownTimer -= Time.deltaTime;
        }
    }

    private void Start()
    {
        lr.enabled = false;
    }

    private void LateUpdate()
    {
        if (zipping)
        {
            // keep line position on the aimer object
            lr.SetPosition(0, zipAimer.position);
        }
    }

    // initiate zip
    // public so that the input system can access it
    public void StartZip(InputAction.CallbackContext context)
    {
        // only work if aiming in
        if (plrMovement.aimingState == AimState.AIMING)
        {
            if (context.started)
            {
                // check if cooldown is already active
                if (zipCooldownTimer > 0)
                {
                    return;
                }

                zipping = true;
                plrMovement.freeze = true;

                // funky little raycasting
                RaycastHit hit;
                if (Physics.Raycast(cam.position, cam.forward, out hit, maxZipDistance, zippable))
                {
                    zipPoint = hit.point;

                    // call the execution of the zip on the specified delay
                    Invoke(nameof(ExecuteZip), zipDelay);
                }
                else
                {
                    zipPoint = cam.position + cam.forward * maxZipDistance;

                    Invoke(nameof(StopZip), zipDelay);
                }

                lr.enabled = true;
                lr.SetPosition(1, zipPoint);
            }
            
        }
    }

    private void ExecuteZip()
    {
        plrMovement.freeze = false;


        // lowest point of the player
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        // difference between player's lowest point and where the zip target is & the overshoot
        float zipPointRelativeYPos = zipPoint.y - lowestPoint.y;
        float highestPointOnArc = zipPointRelativeYPos + overshootYAxis;

        // use overshoot if point is below the player
        if (zipPointRelativeYPos < 0)
        {
            highestPointOnArc = overshootYAxis;
        }

        plrMovement.ZipToPosition(zipPoint, highestPointOnArc);

        // le delay
        Invoke(nameof(StopZip), 1.0f);
    }

    public void StopZip()
    {
        plrMovement.freeze = false;
        zipping = false;
        zipCooldownTimer = zipCooldown;
        lr.enabled = false;
    }
}
