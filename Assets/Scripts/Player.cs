using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private int lives;
    [SerializeField] private int score;

    private float currentCheckPointX;
    private float currentCheckPointY;
    private float currentCheckPointZ;

    private bool hasReachedCheckpoint;

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

    public float CurrentCheckPointX
    {
        get { return currentCheckPointX; }
        set { currentCheckPointX = value; }
    }

    public float CurrentCheckPointY
    {
        get { return currentCheckPointY; }
        set { currentCheckPointY = value; }
    }

    public float CurrentCheckPointZ
    {
        get { return currentCheckPointZ; }
        set { currentCheckPointZ = value; }
    }

    public bool HasReachedCheckpoint
    {
        get { return hasReachedCheckpoint; }
        set { hasReachedCheckpoint = value; }
    }

    public void SavePlayer()
    {
        Debug.Log("Saving player data");
        SaveSystem.SaveGame(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        Debug.Log("DATA: \nlives = " + data.lives + " \nscore = " + data.score + "\nhasReachedCheckpoint = " + data.hasReachedCheckpoint);

        // loading checkpoint position data
        if (data.hasReachedCheckpoint)
        {
            hasReachedCheckpoint = data.hasReachedCheckpoint;
            transform.position = new Vector3(data.currentCheckPointX, data.currentCheckPointY, data.currentCheckPointZ);
        }

        lives = data.lives;
        score = data.score;
    }

    public void PlayerDeath()
    {
        GetComponentInChildren<BoxCollider>().enabled = false;
        float livesPrev = lives;

        if (lives == livesPrev)
        {
            lives--;
        }
        
        if (lives < 1)
        {
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            GameObject uiObj = GameObject.Find("Canvas");
            uiObj.GetComponent<UIScript>().PlayerDeathScreen();
        }
    }

    private void Awake()
    {
        // check save
        if (!SaveSystem.DoesPlayerFileExist())
        {
            SaveSystem.CreatePlayerFile(this);
            SavePlayer();
        }
        else
        {
            LoadPlayer();
        }
    }
}
