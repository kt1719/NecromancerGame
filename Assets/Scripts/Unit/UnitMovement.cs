using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    Animator animator;
    Vector2 targetPosition = new Vector2(0, 0);
    private UnitScriptableObject unitScriptableObject;
    private void Awake() {
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     // // Get the horizontal and vertical input
    //     // float horizontal = Input.GetAxisRaw("Horizontal");
    //     // float vertical = Input.GetAxisRaw("Vertical");
    //     // // Set the animator parameters
    //     // animator.SetBool("isWalking", horizontal != 0 || vertical != 0);

    //     // // Move the player
    //     // transform.Translate(new Vector3(horizontal, vertical, 0) * Time.deltaTime);
    // }

    public void SetScriptableObject(UnitScriptableObject unitScriptableObject) {
        this.unitScriptableObject = unitScriptableObject;
    }

    public void Move(bool isAttacking) {
        if (unitScriptableObject == null || isAttacking && Vector3.Distance(transform.position, targetPosition) < unitScriptableObject.range) {
            return;
        }
        // Move the player to the position over time
        // Move our position a step closer to the target.
        float step =  unitScriptableObject.movementSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }

    public void SetTargetPosition(Vector3 position) {
        // Set the target position with random offset
        targetPosition = position + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
    }
}
