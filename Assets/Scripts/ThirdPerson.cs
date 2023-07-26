using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public enum CameraStyle
{
    BASIC,
    COMBAT
}

// have the player rotate based on direction walking
public class ThirdPerson : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float rotateSpeed;

    [SerializeField] private CameraStyle style;
    [SerializeField] Transform combatLookAt;
    [SerializeField] private CinemachineFreeLook mainCamera;

    [SerializeField] private PlayerMovement plrMovement;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

       
    }

    private void Update()
    {
        // rotate orientation
        Vector3 dir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);

        orientation.forward = dir.normalized;

        // cam styles
        switch (style)
        {
            case CameraStyle.BASIC:
                // rotate player object
                float hoz = Input.GetAxis("Horizontal");
                float ver = Input.GetAxis("Vertical");
                Vector3 inDir = orientation.forward * ver + orientation.right * hoz;
                        
                // face the direction of the camera
                if (inDir != Vector3.zero)
                {
                    playerObj.forward = Vector3.Slerp(playerObj.forward, inDir.normalized, Time.deltaTime * rotateSpeed);
                }
                break;
            case CameraStyle.COMBAT:
                // slightly different camera
                Vector3 c_dir = player.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);

                orientation.forward = c_dir.normalized;
                playerObj.forward = c_dir.normalized;
                break;
        }

        // camera fov based on if aiming
        switch (plrMovement.aimingState)
        {
            case AimState.NEUTRAL:
                mainCamera.m_Lens.FieldOfView = 50;
                break;
            case AimState.AIMING:
                mainCamera.m_Lens.FieldOfView = 30;
                break;
            case AimState.ZIPPING:
                // test value
                mainCamera.m_Lens.FieldOfView = 90;
                break;
            case AimState.RUNNING:
                mainCamera.m_Lens.FieldOfView = 65;
                break;
        } 

    }
}
