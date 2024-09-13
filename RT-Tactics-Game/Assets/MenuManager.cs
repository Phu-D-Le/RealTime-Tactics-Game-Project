using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; set; }

    public GameObject menuCanvas;
    public GameObject uiCanvas;
    public GameObject saveMenu;
    public GameObject settingsMenu;
    public GameObject inGameenu;


    public bool isMenuOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        // Check if the "M" key is pressed and the menu is not open
        if (Input.GetKeyDown(KeyCode.M) && !isMenuOpen)
        {
            uiCanvas.SetActive(false); // Hide the UI canvas
            menuCanvas.SetActive(true); // Show the menu canvas
            isMenuOpen = true; // Set the menu state to open
        }
        // Check if the "M" key is pressed and the menu is already open
        else if (Input.GetKeyDown(KeyCode.M) && isMenuOpen)
        {
            uiCanvas.SetActive(true); // Show the UI canvas
            menuCanvas.SetActive(false); // Hide the menu canvas
            isMenuOpen = false; // Set the menu state to closed
        }
    }
}
