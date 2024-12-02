using KayosStudios.AsteroidQuest.AsteroidManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest
{
    public class InputHandler
    {
        private GamePhase currentPhase;
        private AsteroidData selectedAsteroid;
        
        private float rotationSpeed;

        public void SetPhase(GamePhase phase, AsteroidData asteroid = null)
        {
            currentPhase = phase;
            selectedAsteroid = asteroid; //Assign selected asteroid for Phase Two
        }

        public void HandleInputs(float rotationSpeed)
        {
            this.rotationSpeed = rotationSpeed;

            switch (currentPhase)
            {
                case GamePhase.S1_PhaseOne:
                    HandlePhaseOneInputs();
                    break;
                case GamePhase.S1_PhaseTwo:
                    HandlePhaseTwoInputs();
                    break;
                default:
                    break;
            }
        }

        private void HandlePhaseOneInputs()
        {
            if (Input.GetMouseButtonDown(0)) //Left Click
            {
                //Create a ray from the camera to the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //Perform the raycast
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    //Check if the object hit implemts ISelectable
                    if (hit.collider.TryGetComponent<ISelectable>(out ISelectable selectable))
                    {
                        selectable.OnSelect();
                    }
                }
            }
        }

        private void HandlePhaseTwoInputs()
        {
            if (selectedAsteroid == null) return;

            float horizontalInput = Input.GetAxis("Horizontal"); // Arrow keys or A/D
            float verticalInput = Input.GetAxis("Vertical"); // Arrow keys or W/S

            AsteroidController selectedController = selectedAsteroid.asteroidObj.GetComponent<AsteroidController>();
            selectedController.OnRotate(horizontalInput, verticalInput, rotationSpeed);

            // Select orbs
            if (Input.GetMouseButtonDown(0)) // Left-click
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    OrbController orb = hit.collider.GetComponent<OrbController>();
                    if (orb != null)
                    {
                        Debug.Log($"Orb selected: {orb.name}");
                        // Handle orb selection logic here
                    }
                }
            }
        }
    }
}