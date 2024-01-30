using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFarming : MonoBehaviour
{
    private TreeCore treeCore;
    public float characterRange = 1.5f;
    public int damage = 20;
    public float attackSpeed = 0.1f;

    public void ChopTree(TreeCore treeCore)
    {
        this.treeCore = treeCore;
        // Call DamageTree function every 1 second
        InvokeRepeating("DamageTree", 0, attackSpeed);
    }

    private void DamageTree()
    {
        if (treeCore == null)
        {
            return;
        }
        if (Vector3.Distance(transform.position, treeCore.transform.position) > characterRange)
        {
            return;
        }
        if (treeCore.TakeDamage(damage))
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
