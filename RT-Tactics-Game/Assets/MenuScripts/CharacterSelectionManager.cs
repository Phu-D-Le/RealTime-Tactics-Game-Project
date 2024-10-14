using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterSelectionManager : MonoBehaviour
{
    public Button lightPawnButton; // Button to select Light Pawn
    public Button mediumPawnButton; // Button to select Medium Pawn
    public Button heavyPawnButton; // Button to select Heavy Pawn
    public Button confirmButton; // Button to confirm selection
    public Button resetButton; // Button to reset selections

    public TMP_Text lightCountText; // TextMeshPro text to display Light Pawn count
    public TMP_Text mediumCountText; // TextMeshPro text to display Medium Pawn count
    public TMP_Text heavyCountText; // TextMeshPro text to display Heavy Pawn count

    private int maxSelections = 3; // Maximum number of selections allowed
    private int currentSelectionCount = 0; // Counter to keep track of selections

    public GameObject lightPawnPrefab; // Prefab for Light Pawn
    public GameObject mediumPawnPrefab; // Prefab for Medium Pawn
    public GameObject heavyPawnPrefab; // Prefab for Heavy Pawn

    // List of all spawn points shared among all pawn types
    public List<Transform> spawnPoints; // List of spawn points to use in order

    // Index to keep track of the next available spawn point
    private int nextSpawnIndex = 0;

    // Lists to track spawned pawn instances
    private List<GameObject> lightPawns = new List<GameObject>(); // Track spawned Light Pawns
    private List<GameObject> mediumPawns = new List<GameObject>(); // Track spawned Medium Pawns
    private List<GameObject> heavyPawns = new List<GameObject>(); // Track spawned Heavy Pawns

    void Start()
    {
        // Remove any previously attached listeners to prevent duplicate calls
        lightPawnButton.onClick.RemoveAllListeners();
        mediumPawnButton.onClick.RemoveAllListeners();
        heavyPawnButton.onClick.RemoveAllListeners();
        confirmButton.onClick.RemoveAllListeners();
        resetButton.onClick.RemoveAllListeners(); // Add listener removal for reset button

        // Assign button click listeners
        lightPawnButton.onClick.AddListener(() => OnPawnClick(lightPawnPrefab, "Light"));
        mediumPawnButton.onClick.AddListener(() => OnPawnClick(mediumPawnPrefab, "Medium"));
        heavyPawnButton.onClick.AddListener(() => OnPawnClick(heavyPawnPrefab, "Heavy"));
        confirmButton.onClick.AddListener(ConfirmSelection);
        resetButton.onClick.AddListener(ResetSelection); // Add listener for reset button

        // Initialize count texts
        UpdatePawnCount("Light", 0);
        UpdatePawnCount("Medium", 0);
        UpdatePawnCount("Heavy", 0);
    }

    // General function triggered when any pawn button is clicked
    public void OnPawnClick(GameObject pawnPrefab, string pawnType)
    {
        // Check if there's room to spawn another pawn and a spawn point is available
        if (currentSelectionCount < maxSelections && nextSpawnIndex < spawnPoints.Count)
        {
            // Spawn a new pawn at the next available spawn point
            SpawnPawn(pawnPrefab, spawnPoints[nextSpawnIndex], pawnType);
            nextSpawnIndex++; // Move to the next spawn point for the next click
        }
    }

    // Function to spawn a pawn at a specified spawn point
    void SpawnPawn(GameObject pawnPrefab, Transform spawnPoint, string pawnType)
    {
        GameObject newPawn = Instantiate(pawnPrefab, spawnPoint.position, spawnPoint.rotation);
        currentSelectionCount++; // Increment the selection count

        // Track the spawned pawn based on its type
        switch (pawnType)
        {
            case "Light":
                lightPawns.Add(newPawn);
                UpdatePawnCount("Light", lightPawns.Count); // Update the count for Light Pawns
                break;
            case "Medium":
                mediumPawns.Add(newPawn);
                UpdatePawnCount("Medium", mediumPawns.Count); // Update the count for Medium Pawns
                break;
            case "Heavy":
                heavyPawns.Add(newPawn);
                UpdatePawnCount("Heavy", heavyPawns.Count); // Update the count for Heavy Pawns
                break;
        }
    }

    // Function to update the count of selected pawns displayed on the screen
    void UpdatePawnCount(string type, int count)
    {
        switch (type)
        {
            case "Light":
                lightCountText.text = count.ToString();
                break;
            case "Medium":
                mediumCountText.text = count.ToString();
                break;
            case "Heavy":
                heavyCountText.text = count.ToString();
                break;
        }
    }

    // Function to handle confirming the selection and proceeding
    void ConfirmSelection()
    {
        // Only proceed if at least one pawn is selected
        if (currentSelectionCount > 0)
        {
            LoadGameScene();
        }
    }

    // Function to load the game scene (update with your actual scene name)
    void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameDemo");
    }

    // Function to reset all selections and undo everything
    void ResetSelection()
    {
        // Destroy all spawned pawns and clear the lists
        foreach (GameObject pawn in lightPawns)
        {
            Destroy(pawn);
        }
        lightPawns.Clear();

        foreach (GameObject pawn in mediumPawns)
        {
            Destroy(pawn);
        }
        mediumPawns.Clear();

        foreach (GameObject pawn in heavyPawns)
        {
            Destroy(pawn);
        }
        heavyPawns.Clear();

        // Reset counts and spawn index
        currentSelectionCount = 0;
        nextSpawnIndex = 0;

        // Reset UI texts to default
        UpdatePawnCount("Light", 0);
        UpdatePawnCount("Medium", 0);
        UpdatePawnCount("Heavy", 0);
    }
}
