using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObjects/Unit")]
public class UnitScriptableObject : ScriptableObject
{
    // Scriptable Object for Tower
    public string unitName = "DefaultName";
    public float range = 2.5f;
    public int damage;
    public float attackSpeed;
    public int health;
    public float movementSpeed;
    public float pointerAbovePlayer = 4f;
    public Sprite pointerSprite;

    private void OnEnable() {
        if (pointerSprite == null) {
            pointerSprite = (Sprite)AssetDatabase.LoadAssetAtPath<Sprite>("Assets/PixelArt/UI/placeholder_pointer.png").ConvertTo(typeof(Sprite));
        }
    }
}
