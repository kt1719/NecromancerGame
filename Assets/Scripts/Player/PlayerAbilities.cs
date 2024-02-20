using UnityEngine;

namespace Player{
    public class PlayerAbilities : MonoBehaviour
    {
        public GameObject laser;
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
            if (Input.GetKeyDown("1"))
            {
                laserInstance = Instantiate(laser, transform.position, transform.rotation);
            }
            if (Input.GetKey("1"))
            {
                // Draw a line from the player to the mouse position
                // Get the camera from the child object
                Vector3 mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
                // Loop through the laser's line renderer points and set them to the player's position
                laserInstance.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                laserInstance.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
            }
            if (Input.GetKeyUp("1"))
            {
                // Destroy the laser
                Destroy(laserInstance);
            }
        }
    }
}
