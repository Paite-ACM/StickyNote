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

    private bool menuActive;

    // turning on/off the menu by pressing escape
    public void ToggleMenu()
    {
        if (!menuActive)
        {
            menuActive = true;
            menuObj.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            eventSystem.SetSelectedGameObject(defMenuObj);
        }
        else
        {
            menuActive = false;
            menuObj.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Update()
    {
        scoreText.text = "Score: " + player.Score;
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
        SceneManager.LoadScene("MainMenu");
    }
}
