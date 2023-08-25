using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int lives;
    [SerializeField] private int score;

    public int Lives
    {
        get { return lives; }
        set { lives = value; }
    }

    public int Score
    {
        get { return  score; }
        set { score = value; }
    }
}
