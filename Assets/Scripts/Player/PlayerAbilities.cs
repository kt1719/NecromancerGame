using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Player{
    public class PlayerAbilities : MonoBehaviour
    {
        private GameObject laserInstance;
        Camera playerCamera;
        Light2D abilityLight;
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
                laserInstance = Instantiate(abilityGameObjects["laser"], transform.position, transform.rotation);
                // Instantiate cone shaped light
                abilityLight= laserInstance.AddComponent<Light2D>();
                abilityLight.lightType = Light2D.LightType.Point;
                abilityLight.intensity = 3.0f;
                abilityLight.color = Color.white;
                abilityLight.pointLightInnerRadius = 0f;
                abilityLight.pointLightInnerAngle = 4f;
                abilityLight.pointLightOuterAngle = 4f;
                abilityLight.intensity = 2f;
                abilityLight.blendStyleIndex = 0;
                abilityLight.falloffIntensity = 0.45f;
            }
            if (abilityInputs[1])
            {
                // Draw a line from the player to the mouse position
                // Get the camera from the child object
                Vector3 mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
                // Loop through the laser's line renderer points and set them to the player's position
                laserInstance.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                laserInstance.GetComponent<LineRenderer>().SetPosition(1, mousePosition);

                // Set the light's position to the laser's position
                abilityLight.transform.position = transform.position;
                // Set the light's direction to the mouse position
                abilityLight.transform.LookAt(mousePosition);
                float distance = Vector3.Distance(transform.position, mousePosition);
                abilityLight.pointLightOuterRadius = distance * 0.75f;
                // Rotate the laserInstance z direciton to face the mouse
                Vector3 direction = mousePosition - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                angle -= 91.5f;
                laserInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
            if (abilityInputs[2])
            {
                // Destroy the laser
                Destroy(laserInstance);
            }
        }
    }
}
