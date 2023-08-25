using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType
{
    COIN
}

public class Collectables : MonoBehaviour
{
    public CollectableType colletableType;

    public int scoreToGive;

}
