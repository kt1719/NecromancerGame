using System.Collections.Generic;
using UnityEngine;

namespace Player{
    public class PlayerAbilities : MonoBehaviour
    {
        private GameObject laserInstance;
        Camera playerCamera;
        // Start is called before the first frame update
        void Awake()
        {
            playerCamera = this.transform.GetChild(0).GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            // if (Input.GetKeyDown("1"))
            // {
            //     laserInstance = Instantiate(laser, transform.position, transform.rotation);
            // }
            // if (Input.GetKey("1"))
            // {
            //     // Draw a line from the player to the mouse position
            //     // Get the camera from the child object
            //     Vector3 mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
            //     // Loop through the laser's line renderer points and set them to the player's position
            //     laserInstance.GetComponent<LineRenderer>().SetPosition(0, transform.position);
            //     laserInstance.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
            // }
            // if (Input.GetKeyUp("1"))
            // {
            //     // Destroy the laser
            //     Destroy(laserInstance);
            // }
        }

        public void UseAbilities(Dictionary<int, bool> abilityInputs, Dictionary<string, GameObject> abilityGameObjects) {
            if (abilityInputs[0])
            {
                Debug.Log("Laser");
                laserInstance = Instantiate(abilityGameObjects["laser"], transform.position, transform.rotation);
            }
            if (abilityInputs[1])
            {
                // Draw a line from the player to the mouse position
                // Get the camera from the child object
                Vector3 mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
                // Loop through the laser's line renderer points and set them to the player's position
                laserInstance.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                laserInstance.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
            }
            if (abilityInputs[2])
            {
                // Destroy the laser
                Destroy(laserInstance);
            }
        }
    }
}
