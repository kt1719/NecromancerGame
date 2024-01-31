using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCore : MonoBehaviour
{
    public bool selected = false;
    UnitMovement unitMovement;
    UnitCombat unitCombat;
    // Start is called before the first frame update
    public UnitScriptableObject unitScriptableObjectReference;
    private UnitScriptableObject unitScriptableObject;
    private bool attacking = false;
    void Awake()
    {
        unitMovement = GetComponent<UnitMovement>();
        unitCombat = GetComponent<UnitCombat>();
        // Set scriptable object reference to all components
        // Create a new instance of the scriptable object
        unitScriptableObject = Instantiate(unitScriptableObjectReference);
        // Copy the values from the scriptable object to the components
        unitMovement.SetScriptableObject(unitScriptableObject);
        unitCombat.SetScriptableObject(unitScriptableObject);
    }

    private void FixedUpdate() {
        unitMovement.Move(attacking);
    }
    public void Select()
    {
        selected = true;
        // place pointer above player
        ShowPointer();
    }

    public void Deselect()
    {
        selected = false;
        // remove pointer
        UnshowPointer();
    }

    public void SetTargetPosition(Vector3 position) {
        attacking = false;
        unitMovement.SetTargetPosition(position);
        unitCombat.CancelUnitsAttack();
    }

    public void CommandUnitsAttack(EnemyCore enemyCore) {
        if (attacking) return;
        attacking = true;
        unitMovement.SetTargetPosition(enemyCore.gameObject.transform.position);
        unitCombat.AttackEnemy(enemyCore);
    }

    private void ShowPointer()
    {
        GameObject pointer = new GameObject();
        pointer.transform.position = transform.position + new Vector3(0, unitScriptableObject.pointerAbovePlayer, 0);
        pointer.AddComponent<SpriteRenderer>();
        pointer.GetComponent<SpriteRenderer>().sprite = unitScriptableObject.pointerSprite;
        pointer.GetComponent<SpriteRenderer>().sortingOrder = 1;
        pointer.transform.parent = transform;
    }

    private void UnshowPointer()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
}
