using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    private EnemyCore enemyCore;
    private UnitScriptableObject unitScriptableObject;

    public void SetScriptableObject(UnitScriptableObject unitScriptableObject) {
        this.unitScriptableObject = unitScriptableObject;
    }

    public void AttackEnemy(EnemyCore enemyCore)
    {
        if (enemyCore == this.enemyCore) return; // Same enemy -> Do nothing
        // Cancel previous invoke if there are any
        CancelInvoke("DamageEnemy");
        this.enemyCore = enemyCore;
        // Call DamageEnemy function every 1 second
        InvokeRepeating("DamageEnemy", 0, 1/unitScriptableObject.attackSpeed);
    }

    private void DamageEnemy()
    {
        if (enemyCore == null)
        {
            return;
        }
        if (Vector3.Distance(transform.position, enemyCore.transform.position) > unitScriptableObject.range)
        {
            return;
        }
        if (enemyCore.TakeDamage(unitScriptableObject.damage))
        {
            // Enemy is destroyed
            enemyCore = null;
            // Stop calling DamageEnemy function
            CancelInvoke("DamageEnemy");
        }
    }

    public void CancelUnitsAttack()
    {
        enemyCore = null;
        CancelInvoke("DamageEnemy");
    }
}
