using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // lazy
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            SaveSystem.DeletePlayerFile();
            SceneManager.LoadScene("MainMenu");
        }

        // gaaah
        if (Gamepad.current != null)
        {
            // of course there's no controller equivalent to anyKey
            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                SaveSystem.DeletePlayerFile();
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
