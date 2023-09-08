using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Text scoreText;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject menuObj;
    [SerializeField] private GameObject defMenuObj;
    [SerializeField] private Text livesText;

    // Death screen
    [SerializeField] private GameObject diedObj;
    [SerializeField] private GameObject defDeathObj;

    // win screen
    [SerializeField] private GameObject winObj;
    [SerializeField] private GameObject defWinObj;

    private bool menuActive;

    // turning on/off the menu by pressing escape
    public void ToggleMenu()
    {
        if (!menuActive)
        {
            menuActive = true;
            menuObj.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            eventSystem.SetSelectedGameObject(defMenuObj);
        }
        else
        {
            menuActive = false;
            menuObj.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        scoreText.text = "Score: " + player.Score;
        livesText.text = "Lives: " + player.Lives;
    }

    // pressing unpause button
    public void ResumeButton()
    {
        menuActive = false;
        menuObj.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // quit to menu button
    public void QuitButton()
    {
        player.SavePlayer();
        SceneManager.LoadScene("MainMenu");
    }

    // display death screen
    public void PlayerDeathScreen()
    {
        diedObj.SetActive(true);
        eventSystem.SetSelectedGameObject(defDeathObj);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // display win screen
    public void PlayerWinScreen()
    {
        winObj.SetActive(true);
        eventSystem.SetSelectedGameObject(defWinObj);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // death screen retry button
    public void RetryButton()
    {
        player.SavePlayer();
        SceneManager.LoadScene("MainScene");
    }
}
