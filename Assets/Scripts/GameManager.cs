using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using KayosStudios.AsteroidQuest.AsteroidManagement;

namespace KayosStudios.AsteroidQuest
{
    /// <summary>
    /// defines a contract: any class implementing it must provide an InstantiateObject method that 
    /// takes a GameObject (the prefab) and returns a GameObject (the instantiated object).
    /// </summary>
    public interface IObjectInstantiator
    {
        GameObject InstantiateObject(GameObject prefab);
    }

    public enum AsteroidType
    {
        Red,
        Blue,
        Green
    }

    [System.Serializable]
    public class AsteroidData
    {
        public GameObject asteroidObj;
        public AsteroidType asteroidType;
        public Vector3 position;
        public List<OrbData> orbs;
    }

    [System.Serializable]
    public class OrbData
    {
        public Vector3 position;
        public List<CellData> cells;
    }

    [System.Serializable]
    public class CellData
    {
        public bool isEnergized;
    }

    public class GameManager : MonoBehaviour, IObjectInstantiator
    {
        //Manager Classes
        private AsteroidManager asteroidManager;
        private DataManager dataManager;
        private CameraManager cameraManager;

        [Header("Global Settings")]
        public int seed = -1;

        [Header("Camera Settings")]
        public CinemachineVirtualCamera selectionCamera;
        public CinemachineVirtualCamera asteroidCamera;


        [Header("Asteroid Settings")]
        public int spawnCount;
        public GameObject[] asteroidPrefabs;
        public float minAsteroidSpacing;
        
        [Header("Phase 1 Settings")]
        public List<AsteroidData> spawnedAsteroids;

        [Header("Phase 2 Settings")]
        public AsteroidData selectedAsteroid;

        private void OnEnable()
        {
            EventManager.Instance.OnStart += InitilizeGame;
            EventManager.Instance.OnAsteroidSelected += (asteroid) => SelectAsteroid(asteroid);
        }

        private void OnDisable()
        {
            EventManager.Instance.OnStart -= InitilizeGame;
        }

        private void InitilizeGame()
        {
            dataManager = new DataManager();
            cameraManager = new CameraManager(selectionCamera, asteroidCamera);
           

            //Pass GameManager (implementing IObjectInstatiator) to AsteroidManager
            asteroidManager = new AsteroidManager(this)
            {
                asteroidPrefabs = asteroidPrefabs,
                minSpacing = minAsteroidSpacing
            };

            cameraManager.ActivateSelectionCamera();
            SpawnAsteroids(spawnCount);

        }

        public GameObject InstantiateObject(GameObject prefab)
        {
            return Instantiate(prefab);
        }

        public void SpawnAsteroids(int count)
        {
            asteroidManager.SpawnAsteroids(count);
        }

        public void SelectAsteroid(AsteroidData selectedAsteroid)
        {
            this.selectedAsteroid = selectedAsteroid;
            cameraManager.ActivateAsteroidCamera(selectedAsteroid);

        }
    }
}