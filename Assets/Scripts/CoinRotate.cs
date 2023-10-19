using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The entire purpose of this script is to make the coin object spin since doing it in the animator is lame
public class CoinRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}
