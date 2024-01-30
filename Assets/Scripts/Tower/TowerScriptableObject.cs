using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "ScriptableObjects/Tower", order = 1)]
public class TowerScriptableObject : ScriptableObject
{
    // Scriptable Object for Tower
    public string towerName = "Tower";
    public int towerCost;
    public int towerDamage;
    public float towerRange;
    public Sprite towerSprite;
}
