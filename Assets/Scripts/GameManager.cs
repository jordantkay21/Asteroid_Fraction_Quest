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
        S1_PhaseTwo,
        S1_PhaseThree
    }

    [System.Serializable]
    public class AsteroidData
    {
        public GameObject asteroidObj;
        public AsteroidType asteroidType;
        public Vector3 position;
        public List<OrbData> orbs;
        public int orbTotal;
        public int cellTotal;
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
        public int orbSelectionCount;

        private void OnEnable()
        {
            EventManager.Instance.OnStart += InitilizeGame;
            EventManager.Instance.OnAsteroidSelection += HandleAsteroidSelection;
            EventManager.Instance.OnOrbSelection += HandleOrbSelection;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnStart -= InitilizeGame;
            EventManager.Instance.OnAsteroidSelection -= HandleAsteroidSelection;
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

            switch (currentPhase)
            {
                case GamePhase.S1_PhaseOne:
                    break;
                case GamePhase.S1_PhaseTwo:
                    if(orbSelectionCount == selectedAsteroid.orbTotal)
                    {
                        SetPhase(GamePhase.S1_PhaseThree);
                    }
                    break;
                default:
                    break;
            }
        }

        public void SpawnAsteroids(int count)
        {
            asteroidManager.SpawnAsteroids(count);
        }

        private void SetPhase(GamePhase phase, AsteroidData asteroid = null)
        {
            // Step 1: Update teh current phase
            currentPhase = phase;

            // Step 2: Handle Logic specific to the new phase
            if (phase == GamePhase.S1_PhaseThree && selectedAsteroid != null)
            {
                AsteroidController asteroidController = selectedAsteroid.asteroidObj.GetComponent<AsteroidController>();

                //Define the group center position (e.g., screen center)
                Vector3 screenCenter = new Vector3(0, 0, Camera.main.transform.position.z + 10);

                //Move orbs to the center is a side-by-side arrangement and reparent them
                asteroidController.MoveOrbsToCenter(2f, screenCenter, 1.5f); // Center spacing and duration

                //Disable the asteroid after a delay to ensure the move is triggered
                StartCoroutine(DisableAsteoridAfterDelay(selectedAsteroid.asteroidObj, 1.5f));

                //Adjust the camera to focus on the grouped orbs
                float groupWidth = asteroidController.orbTotal * 2f;
                cameraManager.FocusOnOrbs(screenCenter, groupWidth);
            }

            //Step 3: Notify other systems about the phase change
            inputHandler.SetPhase(phase, asteroid);
        }

        private IEnumerator DisableAsteoridAfterDelay(GameObject asteroid, float delay)
        {
            yield return new WaitForSeconds(delay);
            asteroid.SetActive(false);
        }

        public void HandleAsteroidSelection(AsteroidData selectedAsteroid)
        {
            if (currentPhase == GamePhase.S1_PhaseOne)
            {
                this.selectedAsteroid = selectedAsteroid;

                //Disable other asteroids
                AsteroidController selectedController = selectedAsteroid.asteroidObj.GetComponent<AsteroidController>();
                asteroidManager.DisableNonSelectedAsteroids(selectedController);
                
                //Focus the camera on the selected asteroid
                cameraManager.ActivateAsteroidCamera(selectedAsteroid);

                //Transition to Phase 2
                SetPhase(GamePhase.S1_PhaseTwo, selectedAsteroid);
            }

        }

        public void HandleOrbSelection(OrbController selectedOrb)
        {
            if (selectedOrb.isSelected == true)
            {
                orbSelectionCount++;
            }
            else
                orbSelectionCount--;
        }

        #region Helper Methods
        public GameObject InstantiateObject(GameObject prefab)
        {
            return Instantiate(prefab);
        }

        public Transform CreateOrbContainer()
        {
            GameObject orbContainer = new GameObject("OrbContainer");
            return orbContainer.transform;
        }
        #endregion
    }
}