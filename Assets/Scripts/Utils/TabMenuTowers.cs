using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabMenuTowers : MonoBehaviour
{
    /// <summary>
    /// ScriptableObject references    
    /// </summary>
    public TowerScriptableObject cannonScriptableObject;
    public TowerScriptableObject laserScriptableObject;

    // current scriptable object
    private TowerScriptableObject currentScriptableObject;

    /// <summary>
    /// Variables Used for Tower Placement
    /// </summary>
    public bool placingTower = false;
    private GameObject towerObject; // Used as a way to show the tower before it is placed
    private GameObject tileObject; // Used as a way to show the tile to be placed on

    // Define mapping from string to TowerScriptableObject
    Dictionary<string, TowerScriptableObject> towerNameToScriptableObject = new Dictionary<string, TowerScriptableObject>();
    void Awake()
    {
        InitializeTile();
        SetUpMapping();
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
        }
    }
    private void SetUpMapping()
    {
        towerNameToScriptableObject.Add("Cannon", cannonScriptableObject);
        towerNameToScriptableObject.Add("Laser", laserScriptableObject);
    }
    private void InitializeTile()
    {
        // Create the tile sprite
        tileObject = new GameObject();
        tileObject.AddComponent<SpriteRenderer>();
        // Create a sprite the same size as a tilemap cell size
        Sprite sprite = Sprite.Create(new Texture2D(32, 32), new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
        tileObject.GetComponent<SpriteRenderer>().sprite = sprite;
        tileObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f); // Make the tile transparent white
        tileObject.GetComponent<SpriteRenderer>().enabled = false; // Hide the tile sprite until we call TowerPrePlace
        // Create the tower sprite
        towerObject = new GameObject();
        towerObject.AddComponent<SpriteRenderer>();
        towerObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        // Make sure tile is always behind tower
        tileObject.GetComponent<SpriteRenderer>().sortingOrder = towerObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
    }
    // Function called when we want to place down a tower
    public void TowerPrePlace(string towerString) {
        // Get the TowerScriptableObject from the mapping
        TowerScriptableObject towerScriptableObject = towerNameToScriptableObject[towerString];
        currentScriptableObject = towerScriptableObject;
        // Set placingTower to true
        placingTower = true;
        // Create a tower sprite to follow the mouse
        towerObject.GetComponent<SpriteRenderer>().sprite = towerScriptableObject.towerSprite;
        towerObject.GetComponent<SpriteRenderer>().enabled = true;
        tileObject.GetComponent<SpriteRenderer>().enabled = true;
    }
    public void PlaceTower() {
        if (!placingTower) {
            return;
        }
        placingTower = false;
        // Create gameobject with towerObject's sprite
        GameObject tower = new GameObject();
        tower.AddComponent<SpriteRenderer>();
        tower.GetComponent<SpriteRenderer>().sprite = currentScriptableObject.towerSprite;
        tower.GetComponent<SpriteRenderer>().sortingOrder = 1;
        // Create a tower script
        Tower towerScript = tower.AddComponent<Tower>();
        // Set the tower's scriptable object
        towerScript.towerScriptableObject = towerNameToScriptableObject[currentScriptableObject.towerName];
        // Set the tower's position to mousePosition
        tower.transform.position = towerObject.transform.position;
        StopTowerPlace();
    }
    public void StopTowerPlace() {
        // Set placingTower to false
        placingTower = false;
        towerObject.GetComponent<SpriteRenderer>().sprite = null;
        towerObject.GetComponent<SpriteRenderer>().enabled = false;
        tileObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
