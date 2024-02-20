using System.Collections.Generic;
using GameDefinitions;
using UnityEngine;

namespace Player{
    public class PlayerCore : MonoBehaviour
    {
        public PlayerScriptableObject playerScriptableObjectReference;
        private PlayerScriptableObject playerScriptableObject;
        PlayerMovement playerMovement;
        PlayerAbilities playerAbilities;
        PlayerCamera playerCamera;
        public float speed = 5f;
        private Direction direction = Direction.None;
        Dictionary<int, bool> abilityInputs = new Dictionary<int, bool>() {
            {0, false},
            {1, false},
            {2, false},
            {3, false}
        };

        Dictionary<string, GameObject> abilityGameObjects;
        void Awake()
        {
            // unitMovement = GetComponent<UnitMovement>();
            // unitCombat = GetComponent<UnitCombat>();
            // unitMovement.SetScriptableObject(unitScriptableObject);
            // unitCombat.SetScriptableObject(unitScriptableObject);
            playerMovement = GetComponent<PlayerMovement>();
            playerCamera = this.transform.GetChild(0).GetComponent<PlayerCamera>();
            playerAbilities = GetComponent<PlayerAbilities>();
            playerScriptableObject = Instantiate(playerScriptableObjectReference);
            abilityGameObjects = playerScriptableObject.GetAbilities();
        }

        private void Update() {
            GetInputs();
            playerAbilities.UseAbilities(abilityInputs, abilityGameObjects);
        }

        private void FixedUpdate() {
            playerMovement.Move(direction, speed);
        }

        private void GetInputs() {
            // Get movement inputs
            direction = new Direction(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            // Get ability inputs {1, 2, 3, 4}
            // for (int i = 0; i < 4; i++) {
            //     abilityInputs[i] = Input.GetKey((i + 1).ToString()); // Will change this to be inputtable in the future
            // }
            abilityInputs[0] = Input.GetKeyDown("1");
            abilityInputs[1] = Input.GetKey("1");
            abilityInputs[2] = Input.GetKeyUp("1");
        }
    }
}