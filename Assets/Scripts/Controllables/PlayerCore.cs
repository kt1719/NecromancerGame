using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    public bool selected = false;
    PlayerMovement playerMovement;
    PlayerFarming playerFarming;
    // Start is called before the first frame update
    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerFarming = GetComponent<PlayerFarming>();
    }

    private void Update() {
        playerMovement.Move();
    }

    public void Select() {
        selected = true;
    }

    public void Deselect() {
        selected = false;
    }

    public void SetTargetPosition(Vector3 position) {
        playerMovement.SetTargetPosition(position);
    }

    public void CommandChop(TreeCore treeCore) {
        playerMovement.SetTargetPosition(treeCore.transform.position);
        playerFarming.ChopTree(treeCore);
    }
}
