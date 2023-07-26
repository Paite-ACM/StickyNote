using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zipping : MonoBehaviour
{
    [SerializeField] private PlayerMovement plrMovement;
    [SerializeField] private Transform maxZipDistance;
    [SerializeField] LineRenderer lineRenderer;
    private Ray zipRay;
    private RaycastHit zipHit;
    Camera cam;
    private Vector3 pathway;

    private float zipTimer;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
