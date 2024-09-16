using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuV2 : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private bool isPaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }
    }

    void ActivateMenu()
    {
        pauseMenuUI.SetActive(true);
    }

    void DeactivateMenu()
    {
        pauseMenuUI.SetActive(false);
    }
}
