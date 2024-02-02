using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUI : MonoBehaviour
{
    TowerScriptableObject towerScriptableObject;

    private void Awake() {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScriptableObject(TowerScriptableObject towerScriptableObject) {
        this.towerScriptableObject = towerScriptableObject;
    }
}
