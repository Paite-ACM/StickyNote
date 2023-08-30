using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int lives;
    public int score;
    public float currentCheckPointX;
    public float currentCheckPointY;
    public float currentCheckPointZ;
    public bool hasReachedCheckpoint;

    public PlayerData(Player player)
    {
        lives = player.Lives;
        score = player.Score;
        currentCheckPointX = player.CurrentCheckPointX;
        currentCheckPointY = player.CurrentCheckPointY;
        currentCheckPointZ = player.CurrentCheckPointZ;
        hasReachedCheckpoint = player.HasReachedCheckpoint;
    }
}
