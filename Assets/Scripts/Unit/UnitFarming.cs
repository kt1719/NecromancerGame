using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    private EnemyCore enemyCore;
    private UnitScriptableObject unitScriptableObject;

    public void AttackEnemy(EnemyCore enemyCore)
    {
        this.enemyCore = enemyCore;
        // Call DamageEnemy function every 1 second
        InvokeRepeating("DamageEnemy", 0, 1/unitScriptableObject.attackSpeed);
    }

    public void SetScriptableObject(UnitScriptableObject unitScriptableObject) {
        this.unitScriptableObject = unitScriptableObject;
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
    }
}
