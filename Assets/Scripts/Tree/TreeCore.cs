using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCore : MonoBehaviour
{
    CapsuleCollider2D capsuleCollider2D;
    SpriteRenderer spriteRenderer;
    public int health = 100;
    // Start is called before the first frame update
    void Awake() 
    {
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if mouse is hovering over capsule collider
        if (capsuleCollider2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            // Change the sprite colour to red
            spriteRenderer.color = Color.green;

            // If left mouse button is pressed
            if (Input.GetMouseButtonDown(0))
            {
                // Chop down tree
                DrawWithMouse.CommandChop(this);
            }
        }
        else if (!IsInvoking("ResetColor")) // Check we are not invoking the reset colour
        {
            // Change the sprite colour to white
            spriteRenderer.color = Color.white;
        }
    }

    public bool TakeDamage(int damage)
    {
        health -= damage;
        // Change the sprite colour to red for a short time
        spriteRenderer.color = Color.red;
        Invoke("ResetColor", 0.2f);
        if (health <= 0)
        {
            // Destroy the tree
            Destroy(gameObject);
            return true; // Return true if the tree is destroyed
        }
        return false; // Return false if the tree is not destroyed
    }

    private void ResetColor()
    {
        spriteRenderer.color = Color.white;
    }
}
