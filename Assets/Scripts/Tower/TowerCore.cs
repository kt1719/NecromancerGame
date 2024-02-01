using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCore : MonoBehaviour
{
    private TowerScriptableObject towerScriptableObject;
    SpriteRenderer spriteRenderer;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateTower(TowerScriptableObject towerScriptableObject) {
        this.towerScriptableObject = Instantiate(towerScriptableObject); // Create a new instance of the scriptable object
        // Add a sprite renderer
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = towerScriptableObject.towerSprite;
        spriteRenderer.sortingOrder = 1;
    }
}
