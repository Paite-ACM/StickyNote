using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject defMenuObj;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Button continueButton;

    // Start is called before the first frame update
    void Start()
    {
        // check for save data
        if (SaveSystem.DoesPlayerFileExist())
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }

        eventSystem.SetSelectedGameObject(defMenuObj);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // pressing new game
    public void StartNewGame()
    {
        SaveSystem.DeletePlayerFile();
        SceneManager.LoadScene("MainScene");
    }

    // continue button
    public void ContinueGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    // quit game
    public void QuitButton()
    {
        Application.Quit();
    }
}
