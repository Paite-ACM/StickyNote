using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zipping : MonoBehaviour
{
    // important references 
    [SerializeField] private PlayerMovement plrMovement;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform zipAimer;
    [SerializeField] private LayerMask zippable;

    // zip stuff
    [SerializeField] private float maxZipDistance;

}
