using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterSelection : MonoBehaviour
{
    public Button lightPawnButton;
    public Button mediumPawnButton;
    public Button heavyPawnButton;
    public Button confirmButton;
    public Button resetButton;

    public TMP_Text lightCountText;
    public TMP_Text mediumCountText;
    public TMP_Text heavyCountText;

    public GameObject lightPawnPrefab;
    public GameObject mediumPawnPrefab;
    public GameObject heavyPawnPrefab;

    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    private int maxSelections = 3;
    private int currentSelectionCount = 0;
    private int lightCount = 0;
    private int mediumCount = 0;
    private int heavyCount = 0;

    private bool isPlayerSelecting = true; // Track if it's player's turn

    private List<GameObject> playerPawns = new List<GameObject>();
    private List<GameObject> enemyPawns = new List<GameObject>();

    void Start()
    {
        lightPawnButton.onClick.AddListener(() => AddPawnSelection("Light"));
        mediumPawnButton.onClick.AddListener(() => AddPawnSelection("Medium"));
        heavyPawnButton.onClick.AddListener(() => AddPawnSelection("Heavy"));
        confirmButton.onClick.AddListener(ConfirmSelection);
        resetButton.onClick.AddListener(() => ResetSelection());

        UpdateUI();
    }

    void AddPawnSelection(string pawnType)
    {
        if (currentSelectionCount < maxSelections)
        {
            switch (pawnType)
            {
                case "Light":
                    lightCount++;
                    break;
                case "Medium":
                    mediumCount++;
                    break;
                case "Heavy":
                    heavyCount++;
                    break;
            }
            currentSelectionCount++;
            UpdateUI();
        }
    }

    void ConfirmSelection()
    {
        if (currentSelectionCount != maxSelections)
        {
            // Optional: Display a message or feedback to the player
            Debug.Log("You must select exactly 3 pawns before confirming.");
            return;
        }
        if (currentSelectionCount > 0)
        {
            if (isPlayerSelecting)
            {
                // Spawn pawns for the player
                SpawnPawns(playerSpawnPoint, playerPawns);

                // Make player persistent across scenes
                DontDestroyOnLoad(playerSpawnPoint.gameObject);

                // Switch to enemy selection
                isPlayerSelecting = false;
                ResetSelection(false); // Reset selection for enemy without resetting the UI
                UpdateUI();
            }
            else
            {
                // Spawn pawns for the enemy
                SpawnPawns(enemySpawnPoint, enemyPawns);

                // Make player persistent across scenes
                DontDestroyOnLoad(enemySpawnPoint.gameObject);

                // Both selections done, proceed to next scene
                LoadGameScene();
            }
        }
    }

    void SpawnPawns(Transform spawnPoint, List<GameObject> pawnList)
    {
        // Spawn Light Pawns
        for (int i = 0; i < lightCount; i++)
        {
            GameObject newPawn = Instantiate(lightPawnPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            pawnList.Add(newPawn);
        }

        // Spawn Medium Pawns
        for (int i = 0; i < mediumCount; i++)
        {
            GameObject newPawn = Instantiate(mediumPawnPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            pawnList.Add(newPawn);
        }

        // Spawn Heavy Pawns
        for (int i = 0; i < heavyCount; i++)
        {
            GameObject newPawn = Instantiate(heavyPawnPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            pawnList.Add(newPawn);
        }
    }

    void ResetSelection(bool resetUI = true)
    {
        lightCount = 0;
        mediumCount = 0;
        heavyCount = 0;
        currentSelectionCount = 0;

        if (resetUI)
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        lightCountText.text = lightCount.ToString();
        mediumCountText.text = mediumCount.ToString();
        heavyCountText.text = heavyCount.ToString();
    }

    void LoadGameScene()
    {
        // Load the next scene, passing the player and enemy pawns if needed.
        Debug.Log("Loading next scene with Player and Enemy pawns.");

        // Use Unity's SceneManager to load the next scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameDemo");
    }
}
