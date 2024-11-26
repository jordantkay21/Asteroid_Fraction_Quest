using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.AsteroidManagement
{
    /// <summary>
    /// Controls the behavior of individual asteroids.
    /// </summary>
    public class AsteroidController : MonoBehaviour
    {
        public enum AsteroidType
        {
            Red,
            Blue,
            Green
        }

        [Header("Asteroid Properties")]
        public float minScale;
        public float maxScale;

        [Header("Orb Settings")]
        public GameObject orbPrefab;
        public int minOrbs = 3;
        public int maxOrbs = 8;
        [Tooltip("Minimum distance between orbs")]
        public float minSpacing;

        private List<GameObject> _spawnedOrbs = new List<GameObject>();
        private AsteroidType _asteroidType;

        private void OnMouseEnter()
        {
            //Trigger the asteroid hovered event
            EventManager.Instance.TriggerAsteroidHovered(this);
        }

        private void OnMouseDown()
        {
            //Trigger the asteroid selected event
            EventManager.Instance.TriggerAsteroidSelected(this);
        }
        public void RandomizeScale()
        {
            float randomScale = Random.Range(minScale, maxScale);
            transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }

        public void AssignType()
        {
            //Randomly select a type from the AsteroidType enum
            _asteroidType = (AsteroidType)Random.Range(0, System.Enum.GetValues(typeof(AsteroidType)).Length);
            ApplyTypeVisuals(_asteroidType);
        }

        private void ApplyTypeVisuals(AsteroidType type)
        {
            Renderer renderer = GetComponent<Renderer>();
            switch (type)
            {
                case AsteroidType.Red:
                    renderer.material.color = Color.red;
                    break;
                case AsteroidType.Blue:
                    renderer.material.color = Color.blue;
                    break;
                case AsteroidType.Green:
                    renderer.material.color = Color.green;
                    break;
            }
        }

        public AsteroidType GetAsteroidType()
        {
            return _asteroidType;
        }

        public void SpawnOrbs()
        {
            int orbCount = Random.Range(minOrbs, maxOrbs);

            for (int i = 0; i < orbCount; i++)
            {
                Vector3 orbPosition;
                int attempts = 0;
                const int maxAttempts = 100; //Prevents infinite Loops

                do
                {
                    //Generate a random position on the asteroid's surface
                    Vector3 randomDirection = Random.onUnitSphere; //Random point on a sphere
                    orbPosition = transform.position + randomDirection * (transform.localScale.x / 2f);

                    attempts++;
                    if (attempts >= maxAttempts)
                    {
                        Debug.LogWarning("maxAttempts attempts reached while placing orbs.");
                        break;
                    }
                }
                while (!IsPositionValid(orbPosition, minSpacing));

                // Instantiate the orb at the validated position
                GameObject orb = Instantiate(orbPrefab, transform);
                orb.transform.position = orbPosition;

                // Make the orb stick out vertically
                Quaternion upwardRotation = Quaternion.FromToRotation(Vector3.up, orbPosition - transform.position);
                orb.transform.rotation = upwardRotation;

                // Add to list for tracking
                _spawnedOrbs.Add(orb);

                Debug.Log($"Orb {i} placed after {attempts} attempts at {orbPosition}");
            }

            Debug.Log($"Spawned {orbCount} orbs on {name}");

        }

        /// <summary>
        /// Validates if the given position is far enough from all other orbs.
        /// </summary>
        /// <param name="position">Position to validate.</param>
        /// <param name="minSpacing">Minimum allowed spacing.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private bool IsPositionValid(Vector3 position, float minSpacing)
        {
            foreach (GameObject existingOrb in _spawnedOrbs)
            {
                if (Vector3.Distance(position, existingOrb.transform.position) < minSpacing)
                {
                    return false; // Too close to an existing orb
                }
            }
            return true;
        }
    }
}