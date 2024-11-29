using System;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.AsteroidManagement
{
    public class AsteroidManager
    {
        public GameObject[] asteroidPrefabs;
        public float screenBoundaryPadding = 1f;
        public float minSpacing = 3f;

        private List<AsteroidController> _spawnedAsteroids = new List<AsteroidController>();

        private IObjectInstantiator _instantiator;

        public AsteroidManager(IObjectInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public void SpawnAsteroids(int count)
        {
            float totalAsteroidWidth = (count - 1) * minSpacing; //Minimum spacing between asteroids
            float centerOffset = totalAsteroidWidth / 2f; //Centering offset for the group

            for (int i = 0; i < count; i++)
            {
                //Randomly pick an asteroid type
                GameObject asteroidPrefab = asteroidPrefabs[UnityEngine.Random.Range(0, asteroidPrefabs.Length)];

                //Use the interface to instantiate the object
                GameObject asteroidObject = _instantiator.InstantiateObject(asteroidPrefab);
                AsteroidController asteroid = asteroidObject.GetComponent<AsteroidController>();

                asteroid.RandomizeScale();
                asteroid.AssignType();

                //Calculate horizontal position for each asteroid
                float xPosition = -centerOffset + i * minSpacing;

                //Align vertically (same y-coordinate for all asteroids)
                Vector3 position = new Vector3(xPosition, 0f, 0f);
                asteroid.transform.position = position;

                // Assign a unique name to the asteroid
                asteroid.name = $"asteroid{i}_{asteroid.GetAsteroidType()}";

                //Spawn random orbs on the asteroid
                asteroid.SpawnOrbs();

                //Add to the list of spawned asteroids
                _spawnedAsteroids.Add(asteroid);
            }

            //Validate positions after all asteroids are spawned
            ValidateAsteroidPositions();

        }

        public void ValidateAsteroidPositions()
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

        public void CenterAsteroidGroup()
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