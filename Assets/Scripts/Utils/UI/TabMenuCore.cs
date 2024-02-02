using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabMenuCore : MonoBehaviour
{
    GameObject tabMenu;
    TabMenuTowers tabMenuTowers;
    void Awake()
    {
        // Get the only child of the gameobject
        tabMenu = gameObject.transform.GetChild(0).gameObject;
        // Deactivate the gameobject
        tabMenu.SetActive(false);
        tabMenuTowers = gameObject.GetComponent<TabMenuTowers>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeWindowVisiblity();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            tabMenuTowers.StopTowerPlace();
        }

        if (Input.GetMouseButtonDown(0))
        {
            tabMenuTowers.PlaceTower();
        }
    }

    private void ChangeWindowVisiblity()
    {
        // If the gameobject is active
        if (tabMenu.activeSelf)
        {
            // Deactivate the gameobject
            tabMenu.SetActive(false);
        }
        // If the gameobject is inactive
        else
        {
            // Activate the gameobject
            tabMenu.SetActive(true);
        }
    }

    public void PrePlaceTowerCore(string towerName)
    {
        // Place the tower
        if (tabMenuTowers.TowerPrePlace(towerName)) {
            ChangeWindowVisiblity();
            // Send a message to the player that the tower is being placed
        }
        else {
            // Send a message to the player that the tower cannot be placed
            Debug.Log("Tower can't be placed");
        }
    }
}
