using Unity.VisualScripting;
using UnityEngine;

public class TowerCore : MonoBehaviour
{
    public TowerScriptableObject towerScriptableObjectReference;
    private TowerScriptableObject towerScriptableObject;
    private TowerUI towerUI;
    SpriteRenderer spriteRenderer;
    Material material;
    private void Awake() {
        towerUI = gameObject.GetComponent<TowerUI>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        // Create a new instance of the scriptable object
        towerScriptableObject = Instantiate(towerScriptableObjectReference);
        towerUI.SetScriptableObject(towerScriptableObject);
        // Create copy of material to change the properties
        material = new Material(spriteRenderer.material);
        spriteRenderer.material = material;
        spriteRenderer.sprite = towerScriptableObject.towerSprite;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    // public void InstantiateTower(TowerScriptableObject towerScriptableObject) {
    //     this.towerScriptableObject = Instantiate(towerScriptableObject); // Create a new instance of the scriptable object
    //     // Add a sprite renderer
    //     spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    //     spriteRenderer.sprite = towerScriptableObject.towerSprite;
    //     spriteRenderer.sortingOrder = 1;
    //     this.towerUI = gameObject.AddComponent<TowerUI>();
    //     this.towerUI.SetScriptableObject(this.towerScriptableObject);
    //     // Add a collider (TODO: Add a collider to the prefab)
    // }

    private void OnMouseEnter() {
        // If mouse is over the tower then we want to change the renderer material property
        spriteRenderer.material.SetInt("_EnablePixelOutline", 1);
    }

    private void OnMouseExit() {
        spriteRenderer.material.SetInt("_EnablePixelOutline", 0);
    }
}
