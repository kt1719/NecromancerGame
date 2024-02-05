using UnityEngine;
using GameDefinitions;

namespace Player {
    public class PlayerMovement : MonoBehaviour
    {
        Rigidbody2D rb;

        private void Awake() {
            InstantiateScript();
        }

        public void Move(Direction direction, float speed) {
            // animator.SetBool("isWalking", horizontal != 0 || vertical != 0);
            if (direction == Direction.None) return;
            rb.velocity = direction * speed;
        }

        public void InstantiateScript() {
            rb = gameObject.GetComponent<Rigidbody2D>();
            // Check if we have a Rigidbody2D component
            if (rb == null) {
                // Add a Rigidbody2D component to the GameObject
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
        }
    }
}