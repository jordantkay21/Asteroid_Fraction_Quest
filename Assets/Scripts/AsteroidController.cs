using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.AsteroidManagement
{
    /// <summary>
    /// Controls the behavior of individual asteroids.
    /// </summary>
    public class AsteroidController : MonoBehaviour, ISelectable
    {
        [Header("Asteroid Properties")]
        public float minScale;
        public float maxScale;

        [Header("Orb Settings")]
        public GameObject orbPrefab;
        public int minOrbs = 3;
        public int maxOrbs = 8;
        [Tooltip("Minimum distance between orbs")]
        public float minSpacing;

        [Header("Orb Data")]
        public int orbTotal;
        public int cellTotal;

        private List<OrbController> _spawnedOrbs = new List<OrbController>();
        private AsteroidType _asteroidType;

        public void OnSelect()
        {
            Debug.Log($"Asteroid {name} selected.");

            AsteroidData selectedAsteroid = new AsteroidData
            {
                asteroidObj = gameObject,
                asteroidType = _asteroidType,
                position = transform.position,
                orbs = GenerateOrbData(out orbTotal),
                orbTotal = this.orbTotal,
                cellTotal = this.cellTotal
            };

            EventManager.Instance.TriggerAsteroidSelection(selectedAsteroid);
        }
        public void OnRotate(float horizontalInput, float verticalInput, float rotationSpeed)
        {
            transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, verticalInput * rotationSpeed * Time.deltaTime, Space.World);
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
        public int GetCellCountByType()
        {
            switch (_asteroidType)
            {
                case AsteroidType.Red:
                    return 3;
                case AsteroidType.Blue:
                    return 5;
                case AsteroidType.Green:
                    return 7;
                default:
                    return 3;
            }
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
                _spawnedOrbs.Add(orb.GetComponent<OrbController>());

                //Spawn cells in the orb
                OrbController orbController = orb.GetComponent<OrbController>();
                int cellCount = GetCellCountByType();
                orbController.SpawnCells(cellCount);

                //Energize a random subset of cells
                int energizedCellCount = Mathf.CeilToInt(cellCount / 2f);
                orbController.EnergizedCells(energizedCellCount);

                Debug.Log($"<color=aqua> Orb {i} </color> placed at {orbPosition} with <color=orange>{cellCount} cells</color> which has <color=yellow>{energizedCellCount} cells energized</color>.");
            }

            Debug.Log($" Spawned <color=aqua>{orbCount} orbs</color> on <color=fuchsia>{name}</color>");

        }

        private List<OrbData> GenerateOrbData(out int orbCount)
        {
            List<OrbData> orbDataList = new List<OrbData>();

            for (int i = 0; i < _spawnedOrbs.Count; i++)
            {
                var orb = _spawnedOrbs[i];

                //Generate data for each orb
                OrbData orbData = new OrbData
                {
                    position = orb.transform.position,
                    cells = orb.GenerateCellData(out int CellTotal)
                };

                //Assign a unique name to the orb
                orb.name = $"orb{i}.{name}";

                //Add the orb's data to the list
                orbDataList.Add(orbData);
            }

            orbCount = orbDataList.Count;

            return orbDataList;

        }



        /// <summary>
        /// Validates if the given position is far enough from all other orbs.
        /// </summary>
        /// <param name="position">Position to validate.</param>
        /// <param name="minSpacing">Minimum allowed spacing.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private bool IsPositionValid(Vector3 position, float minSpacing)
        {
            foreach (var existingOrb in _spawnedOrbs)
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