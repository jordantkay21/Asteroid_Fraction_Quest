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

    public interface ISelectable
    {
        void OnSelect();
    }

    [System.Serializable]
    public enum AsteroidType
    {
        Red,
        Blue,
        Green
    }
    [System.Serializable]
    public enum GamePhase
    {
        S1_PhaseOne,
        S1_PhaseTwo
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
        public static GameManager Instance { get; private set; }

        //Manager Classes
        private AsteroidManager asteroidManager;
        private DataManager dataManager;
        private CameraManager cameraManager;
        private InputHandler inputHandler;

        [Header("Global Settings")]
        public GamePhase currentPhase;
        public int seed = -1;

        [Header("Camera Settings")]
        public CinemachineVirtualCamera selectionCamera;
        public CinemachineVirtualCamera asteroidCamera;


        [Header("Asteroid Settings")]
        public int spawnCount;
        public GameObject[] asteroidPrefabs;
        public float minAsteroidSpacing;
        public float rotationSpeed;
        
        [Header("Phase 1 Data")]
        public List<AsteroidData> spawnedAsteroids;

        [Header("Phase 2 Data")]
        public AsteroidData selectedAsteroid; 

        private void OnEnable()
        {
            EventManager.Instance.OnStart += InitilizeGame;
            EventManager.Instance.OnAsteroidSelected += HandleAsteroidSelection;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnStart -= InitilizeGame;
            EventManager.Instance.OnAsteroidSelected -= HandleAsteroidSelection;
        }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        private void InitilizeGame()
        {
            dataManager = new DataManager();
            cameraManager = new CameraManager(selectionCamera, asteroidCamera);
            asteroidManager = new AsteroidManager(this)
            {
                asteroidPrefabs = asteroidPrefabs,
                minSpacing = minAsteroidSpacing
            };
            inputHandler = new InputHandler();

            cameraManager.ActivateSelectionCamera();
            SpawnAsteroids(spawnCount);
            SetPhase(GamePhase.S1_PhaseOne);
        }

        private void Update()
        {
            inputHandler.HandleInputs(rotationSpeed);
        }

        public void SpawnAsteroids(int count)
        {
            asteroidManager.SpawnAsteroids(count);
        }

        private void SetPhase(GamePhase phase, AsteroidData asteroid = null)
        {
            currentPhase = phase;
            inputHandler.SetPhase(phase, asteroid);
        }

        public void HandleAsteroidSelection(AsteroidData selectedAsteroid)
        {
            if (currentPhase == GamePhase.S1_PhaseOne)
            {
                this.selectedAsteroid = selectedAsteroid;
                cameraManager.ActivateAsteroidCamera(selectedAsteroid);
                SetPhase(GamePhase.S1_PhaseTwo, selectedAsteroid);
            }

        }

        #region Helper Methods
        public GameObject InstantiateObject(GameObject prefab)
        {
            return Instantiate(prefab);
        }
        #endregion
    }
}