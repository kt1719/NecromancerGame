using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    private void Awake() {
        if (gameManager == null) {
            gameManager = this;
        } else {
            Destroy(gameObject);
        }
        TowerManager.InstantiateTowerMapping();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
