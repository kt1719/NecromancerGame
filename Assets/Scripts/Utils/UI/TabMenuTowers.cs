using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabMenuTowers : MonoBehaviour
{
    /// <summary>
    /// ScriptableObject references    
    /// </summary>
    public GameObject towerPrefab;
    public GameObject summoningTowerPrefab;
    private string currentScriptableObjectName;
    /// <summary>
    /// Variables Used for Tower Placement
    /// </summary>
    private bool placingTower = false;
    private bool towerCanBePlaced = true;
    private GameObject towerObject; // Used as a way to show the tower before it is placed
    private GameObject tileObject; // Used as a way to show the tile to be placed on
    void Awake()
    {
        InitializeTile();
    }
    // Update is called once per frame
    void Update()
    {
        TowerPrePlaceIndicator();
    }

    private void TowerPrePlaceIndicator()
    {
        if (placingTower)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            // We want only numbers in between whole numbers (e.g., 1.5, 2.5, 3.5, etc.) since this is the midpoint of the grid
            mousePosition.x = Mathf.Round(mousePosition.x) + (mousePosition.x - Mathf.Round(mousePosition.x) > 0 ? 0.5f : -0.5f);
            mousePosition.y = Mathf.Round(mousePosition.y) + (mousePosition.y - Mathf.Round(mousePosition.y) > 0 ? 0.5f : -0.5f);
            towerObject.transform.position = mousePosition;
            tileObject.transform.position = mousePosition;
            // Check if there is a tower already placed on the tile
            if(TowerManager.TileHasTower(mousePosition)) {
                tileObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.5f); // Make the tile transparent red
                towerCanBePlaced = false;
            } else {
                tileObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f); // Make the tile transparent white
                towerCanBePlaced = true;
            }
        }
    }
    private void InitializeTile()
    {
        // Create the tile sprite
        tileObject = new GameObject();
        tileObject.AddComponent<SpriteRenderer>();
        // Create a sprite the same size as a tilemap cell size
        Sprite sprite = Sprite.Create(new Texture2D(48, 48), new Rect(0, 0, 48, 48), new Vector2(0.5f, 0.5f), 16);
        tileObject.GetComponent<SpriteRenderer>().sprite = sprite;
        tileObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f); // Make the tile transparent white
        tileObject.GetComponent<SpriteRenderer>().enabled = false; // Hide the tile sprite until we call TowerPrePlace
        // Create the tower sprite
        towerObject = new GameObject();
        towerObject.AddComponent<SpriteRenderer>();
        towerObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        // Make sure tile is always behind tower
        tileObject.GetComponent<SpriteRenderer>().sortingOrder = towerObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
        // Set parent of towerObject to be the tileObject
        towerObject.transform.parent = this.transform;
        tileObject.transform.parent = this.transform;
        towerObject.GetComponent<SpriteRenderer>().enabled = false; // Hide the tower sprite until we call TowerPrePlace
        // Change name of tileObject
        tileObject.name = "TilePrePlace";
        towerObject.name = "TowerPrePlace";
    }
    // Function called when we want to place down a tower
    public bool TowerPrePlace(string towerString) {
        currentScriptableObjectName = towerString;
        // Get the TowerScriptableObject from the mapping
        GameObject towerPrefab = TowerManager.towerMappings[towerString];
        TowerScriptableObject towerScriptableObject = towerPrefab.GetComponent<TowerCore>().towerScriptableObjectReference;
        if (!TowerManager.towerUnlockMappings[towerString]) {
            Debug.Log("Tower is not unlocked");
            return false;
        }
        // Set placingTower to true
        placingTower = true;
        // Create a tower sprite to follow the mouse
        towerObject.GetComponent<SpriteRenderer>().sprite = towerScriptableObject.towerSprite;
        towerObject.GetComponent<SpriteRenderer>().enabled = true;
        tileObject.GetComponent<SpriteRenderer>().enabled = true;
        return true; // Return true if we can successfully place the tower
    }
    public void PlaceTower() {
        if (!placingTower || !towerCanBePlaced) {
            return;
        }
        // Instantiate the tower Prefab
        GameObject tower = Instantiate(TowerManager.towerMappings[currentScriptableObjectName]);
        // Set the tower's position to mousePosition
        tower.transform.position = towerObject.transform.position;
        // Add the tower's position to the hashset
        TowerManager.AddTowerPosition(tower.transform.position);
        StopTowerPlace();
    }
    public void StopTowerPlace() {
        placingTower = false;
        towerObject.GetComponent<SpriteRenderer>().sprite = null;
        towerObject.GetComponent<SpriteRenderer>().enabled = false;
        tileObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
