using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointData : MonoBehaviour
{
    private Transform checkpointLocation;
    private GameObject player;

    [SerializeField]
    private GameObject[] arrayOfEnemies;
    [SerializeField]
    private GameObject[] arrayOfTriggers;
    [SerializeField]
    private GameObject[] arrayOfCollectibles;
    [SerializeField]
    private GameObject[] arrayOfDestructibles;
    [SerializeField]
    private Dictionary<GameObject, Vector2> activeEnemies;
    [SerializeField]
    private List<GameObject> activeTriggers;
    [SerializeField]
    private List<GameObject> activeCollectibles;
    [SerializeField]
    private List<GameObject> activeDestructibles;

    public int playerMaxHealth;
    public int playerCurrentHealth;
    public int playerGold;
    public int playerKuldo;

    private void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
        arrayOfTriggers = GameObject.FindGameObjectsWithTag("Trigger");
        arrayOfCollectibles = GameObject.FindGameObjectsWithTag("Collectible");
        arrayOfDestructibles = GameObject.FindGameObjectsWithTag("Destructible");

        activeEnemies = new Dictionary<GameObject, Vector2>();
        activeTriggers = new List<GameObject>();
        activeDestructibles = new List<GameObject>();
        activeCollectibles = new List<GameObject>();


        playerMaxHealth = PlayerData.playerMaxHealth;
        playerCurrentHealth = PlayerData.playerCurrentHealth;
        playerGold = PlayerData.playerGold;
        playerKuldo = PlayerData.kuldoCount;
    }

    public void SetCheckpoint(Transform currentCheckpoint)
    {
        Debug.Log("Checkpoint set!");
        // Set respawn location to current checkpoint's location
        checkpointLocation = currentCheckpoint;
        // Since enemies start off inactive the start method call doesn't work so we need to do it here
        arrayOfEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        // Clear any old data from the dict and lists so they don't conflict with current game state
        if (activeEnemies != null) activeEnemies.Clear();
        if (activeTriggers != null) activeTriggers.Clear();
        if (activeCollectibles != null) activeCollectibles.Clear();
        if (activeDestructibles != null) activeDestructibles.Clear();

        // Go through the arrays of assets created at the start of the scene
        // and check which ones are active, adding them to the active list if yes
        foreach (GameObject go in arrayOfEnemies)
        {
            if(go.activeInHierarchy == true)
            {
                activeEnemies.Add(go, new Vector2(go.transform.position.x, go.transform.position.y));
            }
        }

        foreach (GameObject go in arrayOfTriggers)
        {
            if (go.activeInHierarchy == true)
            {
                activeTriggers.Add(go);
            }
        }

        foreach(GameObject go in arrayOfCollectibles)
        {
            if (go.activeInHierarchy == true)
            {
                activeCollectibles.Add(go);
            }
        }

        foreach (GameObject go in arrayOfDestructibles)
        {
            if (go.activeInHierarchy == true)
            {
                activeDestructibles.Add(go);
            }
        }

        // Store player current player stats
        playerMaxHealth = player.GetComponent<Unit>().maxHealth;
        playerCurrentHealth = player.GetComponent<Unit>().currentHealth;
        playerGold = player.GetComponent<Inventory>().GoldCount();
        playerKuldo = player.GetComponent<Inventory>().KuldoCrystalCount();
    }

    public void LoadCheckpoint()
    {
        player.transform.position = new Vector2(checkpointLocation.position.x, checkpointLocation.position.y);
        // Fade back in after death
        //FadeInOut.Instance.gameObject.SetActive(true);
        FadeInOut.Instance.Fade(false, 3.0f);
        // Restore player stats to last checkpoint values, except for deaths
        player.GetComponent<Unit>().maxHealth = playerMaxHealth;
        player.GetComponent<Unit>().currentHealth = playerCurrentHealth;
        player.GetComponent<Unit>().Heal(0);
        int desiredLayer = 9;
        int desiredMask = 1 << desiredLayer;
        player.GetComponent<RaycastController>().collisionMask = desiredMask;
        PlayerData.playerGold = playerGold;
        PlayerData.kuldoCount = playerKuldo;
        player.GetComponent<Inventory>().UpdateInventory();
        player.SetActive(true);

        foreach (KeyValuePair<GameObject, Vector2> enemy in activeEnemies)
        {
            enemy.Key.transform.position = enemy.Value;
            if (enemy.Key.GetComponent<Unit>() == null) Debug.Log("Something without a Unit script is tagged as Enemy!");
            enemy.Key.GetComponent<Unit>().currentHealth = enemy.Key.GetComponent<Unit>().maxHealth;
            if (enemy.Key.GetComponent<Thrall>() != null)
            {
                enemy.Key.GetComponent<Thrall>().ResetWaypoints();
            }
            enemy.Key.SetActive(true);
        }

        foreach (GameObject trigger in activeTriggers)
        {
            trigger.SetActive(true);
        }

        foreach (GameObject collectible in activeCollectibles)
        {
            collectible.SetActive(true);
        }

        foreach (GameObject destructible in activeDestructibles)
        {
            if (destructible.GetComponent<DestructibleTerrain>() != null)
            {
                destructible.GetComponent<DestructibleTerrain>().currentHealth = destructible.GetComponent<DestructibleTerrain>().maxHealth;
            }
            destructible.SetActive(true);
        }
    }
}
