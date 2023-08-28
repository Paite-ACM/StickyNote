using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // the exact position that the player should be respawned at (because just using the transform.position would put the player
    // inside of the checkpoint object)
    [SerializeField] private Vector3 respawnPosition;

    // checking wheteher to display the object's active mode
    [SerializeField] private bool activated;

    [SerializeField] private Material activeMat;

    // the sphere object 
    [SerializeField] private GameObject smellySphere;

    public Vector3 RespawnPosition
    {
        get { return respawnPosition; }
    }

    public bool Activated
    {
        get { return activated; }
        set { activated = value; }
    }


    public void ActivateCheckpoint()
    {
        smellySphere.GetComponent<MeshRenderer>().material = activeMat;
        smellySphere.GetComponent<Animator>().SetBool("Activate", true);
    }
}
