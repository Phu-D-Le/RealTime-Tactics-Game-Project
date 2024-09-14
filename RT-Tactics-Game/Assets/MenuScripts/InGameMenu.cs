using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Test menu");
    }

    
    void Update()
    {
        
    }
}
