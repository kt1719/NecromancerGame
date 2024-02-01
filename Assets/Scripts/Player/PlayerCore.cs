using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    // public PlayerScriptableObject playerScriptableObjectReference;
    // private PlayerScriptableObject playerScriptableObject;
    PlayerMovement playerMovement;
    public float speed = 5f;
    void Awake()
    {
        // unitMovement = GetComponent<UnitMovement>();
        // unitCombat = GetComponent<UnitCombat>();
        // unitScriptableObject = Instantiate(unitScriptableObjectReference);
        // unitMovement.SetScriptableObject(unitScriptableObject);
        // unitCombat.SetScriptableObject(unitScriptableObject);
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void FixedUpdate() {
        playerMovement.Move(speed);
    }
}
