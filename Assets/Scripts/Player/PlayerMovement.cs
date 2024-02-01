using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public void Move(float speed) {
        // Get the horizontal and vertical input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        // Set the animator parameters
        // animator.SetBool("isWalking", horizontal != 0 || vertical != 0);

        // Move the player
        transform.Translate(new Vector3(horizontal, vertical, 0) * Time.deltaTime * speed);
    }
}
