using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.AsteroidManagement
{
    public class AsteroidManager : MonoBehaviour
    {
        [Header("Asteroid Settings")]
        [Tooltip("Array of Asteroid Prefabs")]
        public GameObject[] asteroidPrefabs;
        [Tooltip("Padding to keep asteroids within the player's screen")]
        public float screenBoundaryPadding = 1f;
        [Tooltip("Minimum spacing between asteroids")]
        public float minSpacing = 3f;

        [Header("Seed Settings")]
        [Tooltip("Seed for reproducible environments (set -1 for random seed)")]
        public int seed = -1;

        private List<AsteroidController> _spawnedAsteroids = new List<AsteroidController>();

        private void OnEnable()
        {
            EventManager.Instance.OnStart += InitializeAsteroidManager;

            EventManager.Instance.OnAsteroidSelected += HandleAsteroidSelection;
            EventManager.Instance.OnAsteroidHovered += HandleAsteroidHover;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnStart -= InitializeAsteroidManager;

            EventManager.Instance.OnAsteroidSelected -= HandleAsteroidSelection;
            EventManager.Instance.OnAsteroidHovered -= HandleAsteroidHover;
        }

        private void InitializeAsteroidManager()
        {
            //Set the random seed
            if (seed != -1)
            {
                Debug.Log($"Initializing with Seed: {seed}");
                Random.InitState(seed);
            }
            else
            {
                seed = System.DateTime.Now.Millisecond;
                Random.InitState(seed);
                Debug.Log($"Initializing with Random Seed : {seed}");
            }

            SpawnAsteroids(2);
            ValidateAsteroidPositions();
            CenterAsteroidGroup();
        }

        private void HandleAsteroidSelection(AsteroidController selectedAsteroid)
        {
            //Handle asteroid selection
            Debug.Log($"Asteroid {selectedAsteroid.name} selected!");
        }

        private void HandleAsteroidHover(AsteroidController hoveredAsteroid)
        {
            //Handle asteroid hover
            Debug.Log($"Currently hovering over asteroid {hoveredAsteroid.name}!");
        }

        public void SpawnAsteroids(int count)
        {
            float totalAsteroidWidth = (count - 1) * minSpacing; //Minimum spacing between asteroids
            float centerOffset = totalAsteroidWidth / 2f; //Centering offset for the group

            for (int i = 0; i < count; i++)
            {
                //Randomly pick an asteroid type
                GameObject asteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];

                //Spawn the asteroid and randomize its properties
                AsteroidController asteroid = Instantiate(asteroidPrefab).GetComponent<AsteroidController>();
                asteroid.RandomizeScale();
                asteroid.AssignType();

                //Calculate horizontal position for each asteroid
                float xPosition = -centerOffset + i * minSpacing;

                //Align vertically (same y-coordinate for all asteroids)
                Vector3 position = new Vector3(xPosition, 0f, 0f);
                asteroid.transform.position = position;

                // Assign a unique name to the asteroid
                asteroid.name = $"asteroid{i}_{asteroid.GetAsteroidType()}";

                //Trigger the asteroid spawned event
                EventManager.Instance.TriggerAsteroidSpawned(asteroid);

                //Add to the list of spawned asteroids
                _spawnedAsteroids.Add(asteroid);
            }

            //Validate positions after all asteroids are spawned
            ValidateAsteroidPositions();

            //Trigger the all asteroids spawned event
            EventManager.Instance.TriggerAllAsteroidsSpawned();
        }

        private void ValidateAsteroidPositions()
        {
            float screenLeft = -Camera.main.aspect * Camera.main.orthographicSize + screenBoundaryPadding;
            float screenRight = Camera.main.aspect * Camera.main.orthographicSize - screenBoundaryPadding;

            foreach (var asteroid in _spawnedAsteroids)
            {
                Vector3 position = asteroid.transform.position;

                //Clamp the asteroid's position within the screen boundaries
                position.x = Mathf.Clamp(position.x, screenLeft, screenRight);
                position.y = 0f; //Ensures all asteroids remain at the same vertical level
                asteroid.transform.position = position;
            }
        }

        private void CenterAsteroidGroup()
        {
            float groupCenter = 0f;

            //Calculate the average x position of all asteroids (group center)
            foreach (var asteroid in _spawnedAsteroids)
            {
                groupCenter += asteroid.transform.position.x;
            }
            groupCenter /= _spawnedAsteroids.Count;

            //Offset all asteroids to center the group
            foreach (var asteroid in _spawnedAsteroids)
            {
                asteroid.transform.position -= new Vector3(groupCenter, 0, 0);
            }
        }
    }
} 