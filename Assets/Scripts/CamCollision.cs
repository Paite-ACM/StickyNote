using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;


// despite the name this is for a raycast detecting if the player is too close to the camera 
// if it is then the player will become transparent
public class CamCollision : MonoBehaviour
{
    [SerializeField] private Material defPlayerMat;
    [SerializeField] private Material transPlayerMat;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Renderer playerRenderer;
    public Transform camTransform;

    private Ray ray;
    private RaycastHit hit;

    [SerializeField] private Vector3 offset;

    public float radius;


    // Update is called once per frame
    void Update()
    {
        /*if (Physics.Raycast(playerTransform.position, transform.position + offset, range, playerMask))
        {
            Debug.Log("WOWOWOWOOWOW");
        } */
        // get collisions in a sphere around the player
        

        if (Physics.SphereCast(transform.position, radius, transform.forward, out hit, 0, playerMask))
        {
            // check if camera is target
            
            Debug.Log(hit.collider.name); 
            Debug.Log("AWOOOGA");
        }
    }


    // set transparent material 
    private void MakePlayerTrans()
    {
        playerRenderer.material = transPlayerMat;
    }

    // return to original material
    private void ResetPlayerMat()
    {
        playerRenderer.material = defPlayerMat;
    }
}
