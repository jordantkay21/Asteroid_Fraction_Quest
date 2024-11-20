using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.AsteroidManagement
{
    public class AsteroidRotation : MonoBehaviour
    {
        [Tooltip("Speed of rotation for the asteroid")]
        [SerializeField] float rotationSpeed = 100f;

        [Tooltip("Variable to store whether or not the player is able to rotate the asteroid")]
        private bool isRotationEnabled = true;

        private void Update()
        {
            // Check if rotation is enabled
            if (!isRotationEnabled) return;

            // Get input from arrow keys and calculate rotation
            float horizontalInput = Input.GetAxis("Horizontal"); //Left/Right arrow keys
            float verticalInput = Input.GetAxis("Vertical"); //Up/Down arrow keys

            //Apply rotation based on input
            RotateAsteroid(horizontalInput, verticalInput);
        }

        // Method to rotate the asteroid
        private void RotateAsteroid(float horizontalInput, float verticalInput)
        {
            // Calculate rotation around the Y-axis for left/right movement
            float yRotation = horizontalInput * rotationSpeed * Time.deltaTime;
            // Calculate rotation around the X-axis for up/down movement
            float xRotation = verticalInput * rotationSpeed * Time.deltaTime;

            // Apply the rotations to the asteroid
            transform.Rotate(Vector3.up, yRotation, Space.World);
            transform.Rotate(Vector3.right, -xRotation, Space.World);
        }

        // Method to enable rotation
        public void EnableRotation()
        {
            isRotationEnabled = true;
        }

        // Method to disable rotation
        public void DisableRotation()
        {
            isRotationEnabled = false;
        }
    }
}