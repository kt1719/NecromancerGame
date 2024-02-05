using GameDefinitions;
using UnityEngine;

namespace Player{
    public class PlayerCore : MonoBehaviour
    {
        // public PlayerScriptableObject playerScriptableObjectReference;
        // private PlayerScriptableObject playerScriptableObject;
        PlayerMovement playerMovement;
        public float speed = 5f;
        private Direction direction = Direction.None;
        void Awake()
        {
            // unitMovement = GetComponent<UnitMovement>();
            // unitCombat = GetComponent<UnitCombat>();
            // unitScriptableObject = Instantiate(unitScriptableObjectReference);
            // unitMovement.SetScriptableObject(unitScriptableObject);
            // unitCombat.SetScriptableObject(unitScriptableObject);
            playerMovement = GetComponent<PlayerMovement>();
        }

        private void Update() {
            GetInputs();
        }

        private void FixedUpdate() {
            playerMovement.Move(direction, speed);
        }

        private void GetInputs() {
            direction = new Direction(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }
}