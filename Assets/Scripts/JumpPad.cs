using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    // the multiplier in which your jump will be boosted by
    [SerializeField] private float boostMultiplier;

    public float BoostMultiplier
    {
        get { return boostMultiplier; }
    }
}
