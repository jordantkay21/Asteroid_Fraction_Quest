using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.ManagerClasses
{
    /// <summary>
    /// Manages the overall asteroid and the orbs placed on its surface
    /// 1. Spawns the asteroid and generates the specified number of orbs
    /// 2. Positions orbs randomly on the asteroid's surface
    /// 3. Handles transitions between visual states, like zooming in/out from orbs
    /// 4. Manages the flow from Visual 1 to Visual 2 and Visual 3
    /// </summary>
    public class AsteroidManager : MonoBehaviour
    {

        [Header("Prefab References")]
        [Tooltip("Reference to the asteroid GameObject")]
        [SerializeField] GameObject asteroidPrefab;
        [Tooltip("Prefab reference for the orbs")]
        [SerializeField] GameObject orbPrefab;

        [Header("Orb Spawn Settings")]
        [Tooltip("")]
        [SerializeField] int orbSeed = 0;
        [Tooltip("Minimum number of orbs allowed to spawn")]
        [SerializeField] int minOrb;
        [Tooltip("Maximum number of orbs allowed to spawn")]
        [SerializeField] int maxOrb;
        [Tooltip("Integer to determine how many orbs to generate")]
        [SerializeField] int numOrbs;
        [Tooltip("Float to control the distance between orbs")]
        [SerializeField] float orbSpacing;

        [Tooltip("List to store references to all created orbs")]
        [SerializeField] List<GameObject> orbsList = new();
        [Tooltip("List to store orb positions")]
        [SerializeField] List<Vector3> orbPositions = new();


        [Header("Asteroid Settings")]
        [Tooltip("Radius of the asteroidPrefab to ensure the orbs are correctly placed on the surface")]
        [SerializeField] float asteroidRadius;
        [Tooltip("Minimum scale for the asteroid")]
        [SerializeField] float minScale = 5f;
        [Tooltip("Maximum scale for the asteroid")]
        [SerializeField] float maxScale = 15f;

        private GameObject asteroidInstance;


        private void Start()
        {
            //Instantiate the aseroid at a random scale
            asteroidInstance = Instantiate(asteroidPrefab, Vector3.zero, Quaternion.identity);

            //Set random scale for the asteroid
            float randomScale = Random.Range(minScale, maxScale);
            asteroidInstance.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            //Recalculate asteroidRadius based on the new scale
            asteroidRadius = asteroidInstance.GetComponent<SphereCollider>().radius * asteroidInstance.transform.localScale.x;

            //Spawn the orbs
            OrbsToSpawn();
        }

        private void OrbsToSpawn()
        {
            if(orbSeed == 0)
            {
                orbSeed = System.DateTime.Now.Millisecond;
                Debug.Log($"Current Orb Seed = {orbSeed}");
            }

            // Initialize the random number generator with the seed
            Random.InitState(orbSeed);
            numOrbs = Random.Range(minOrb, maxOrb);
            

            for (int currentOrbCount = 0; currentOrbCount != numOrbs; currentOrbCount++)
            {
                Vector3 orbPosition;
                bool validPosition = false;
                int attempts = 0;
                const int maxAttempts = 100; //Limit to prevent infinite loops

                //Try to find a valid position
                do
                {
                    //Generate Random Angles for current orb
                    float latitude = Random.Range(0f, Mathf.PI);
                    float longitude = Random.Range(0f, 2 * Mathf.PI);

                    //Convert angle into 3D Coordinates
                    float x = asteroidRadius * Mathf.Sin(latitude) * Mathf.Cos(longitude);
                    float y = asteroidRadius * Mathf.Sin(latitude) * Mathf.Sin(longitude);
                    float z = asteroidRadius * Mathf.Cos(latitude);

                    //Create a potential orb position
                    orbPosition = new Vector3(x, y, z);
                    validPosition = true;

                    //Check if this position is too close to any existing orb position
                    foreach (Vector3 exsistingPosition in orbPositions)
                    {
                        if (Vector3.Distance(orbPosition, exsistingPosition) < orbSpacing)
                        {
                            validPosition = false;
                            break;
                        }
                    }

                    attempts++;
                } while (!validPosition && attempts < maxAttempts);

                //If a valid position is found add it to the list and instantiate the orb
                if (validPosition)
                {
                    orbPositions.Add(orbPosition);
                    Instantiate(orbPrefab, orbPosition, Quaternion.LookRotation(orbPosition.normalized));
                    Debug.Log($"Orb {currentOrbCount}/numOrbs position = {orbPosition}");
                }
                else
                {
                    Debug.LogWarning($"failed to find a valid position for orb {currentOrbCount} after {maxAttempts} attempts");
                }
            }
        }
    }
}