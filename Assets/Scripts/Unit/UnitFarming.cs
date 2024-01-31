using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFarming : MonoBehaviour
{
    private TreeCore treeCore;
    private UnitScriptableObject unitScriptableObject;

    public void ChopTree(TreeCore treeCore)
    {
        this.treeCore = treeCore;
        // Call DamageTree function every 1 second
        InvokeRepeating("DamageTree", 0, 1/unitScriptableObject.attackSpeed);
    }

    public void SetScriptableObject(UnitScriptableObject unitScriptableObject) {
        this.unitScriptableObject = unitScriptableObject;
    }

    private void DamageTree()
    {
        if (treeCore == null)
        {
            return;
        }
        if (Vector3.Distance(transform.position, treeCore.transform.position) > unitScriptableObject.range)
        {
            return;
        }
        if (treeCore.TakeDamage(unitScriptableObject.damage))
        {
            // Tree is destroyed
            treeCore = null;
            // Stop calling DamageTree function
            CancelInvoke("DamageTree");
        }
    }

    public void CancelChop()
    {
        treeCore = null;
    }
}
