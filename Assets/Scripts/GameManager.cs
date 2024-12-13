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
        public AsteroidManager asteroidManager;
        public DataManager dataManager;
        public CameraManager cameraManager;
        public InputHandler inputHandler;

        [Header("Global Settings")]
        public GamePhase currentPhase;
        [SerializeField] Color gizmoColor = Color.green;
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
        public GameObject orbContainer;
        public int orbSelectionCount;

        [Header("Phase 3 Data")]
        public GameObject cellContainer;
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
            InitializeSeed();

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

        private void InitializeSeed()
        {
            if (seed != 0)
            {
                Debug.Log($"Setting random seed to: {seed}");
                Random.InitState(seed); //Use the provided seed
            }
            else
            {
                //Generate a random seed based on the current system time
                seed = System.Environment.TickCount;
                Debug.Log($"No seed provided. Setting random seed to {seed}");
                Random.InitState(seed); // Use the generated seed
            }
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
                asteroidController.MoveOrbsToCenter(1f, screenCenter, 1.5f); // Center spacing and duration

                //Disable the asteroid after a delay to ensure the move is triggered
                StartCoroutine(DisableAsteoridAfterDelay(selectedAsteroid.asteroidObj, 1.5f));

                //Adjust the camera to focus on the grouped orbs
                float groupWidth = asteroidController.orbTotal * 2f;
                cameraManager.FocusOnOrbs(orbContainer.transform);
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

        #region Container Management
        public Transform CreateOrbContainer()
        {
            orbContainer = new GameObject("OrbContainer");
            return orbContainer.transform;
        }
        
        public Transform CreateCellContainer()
        {
            cellContainer = new GameObject("CellContainer");
            return cellContainer.transform;
        }

        public void ConstrainOrbsToFOV(Transform orbContainer)
        {
            if (orbContainer == null) return;

            // Get the camera's frustum size at the container's depth
            float distance = Mathf.Abs(Camera.main.transform.position.z - orbContainer.position.z);
            Vector2 frustumSize = cameraManager.GetFrustumSizeAtDepth(distance);

            // Calculate the visible bounds
            Vector3 containerCenter = orbContainer.position;
            float halfWidth = frustumSize.x / 2;
            float halfHeight = frustumSize.y / 2;
            Vector3 minBounds = containerCenter - new Vector3(halfWidth, halfHeight, 0);
            Vector3 maxBounds = containerCenter + new Vector3(halfWidth, halfHeight, 0);

            // Adjust each orb's position to fit within the bounds
            foreach (Transform child in orbContainer)
            {
                Vector3 clampedPosition = child.position;
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);
                clampedPosition.z = containerCenter.z; // Keep Z consistent with container depth
                child.position = clampedPosition;
            }

            Debug.Log("Constrained orbs to fit within camera's FOV.");
        }
        #endregion
        #endregion

        #region Gizmo Debug
        private void OnDrawGizmos()
        {
            Camera mainCamera = Camera.main;

            if (mainCamera == null) return;

            //Set Gizmo color for OrbContainer boundaries
            if (orbContainer != null)
            {
                Gizmos.color = Color.green; // Orb container boundary color
                DrawContainerBounds(orbContainer.transform, mainCamera);
            }

            if (cellContainer != null)
            {
                Gizmos.color = Color.blue;
                DrawContainerBounds(cellContainer.transform, mainCamera);
            }
        }

        /// <summary>
        /// Draws the boundary of a container and checks if it is within the camera's FOV.
        /// </summary>
        /// <param name="container">The container's transform.</param>
        /// <param name="camera">The camera to check visibility against.</param>
        private void DrawContainerBounds(Transform container, Camera camera)
        {
            // Calculate combined bounds of the container
            Bounds combinedBounds = new Bounds(container.position, Vector3.zero);

            foreach (Transform child in container)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    combinedBounds.Encapsulate(childRenderer.bounds);
                }
            }

            // Draw the bounds as a wireframe box
            Gizmos.DrawWireCube(combinedBounds.center, combinedBounds.size);

            // Check if the bounds are within the camera's FOV
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            bool isWithinFOV = GeometryUtility.TestPlanesAABB(frustumPlanes, combinedBounds);

            if (!isWithinFOV)
            {
                Debug.LogWarning($"Bounds of {container.name} fall outside the camera's FOV!");
            }
        }


        #endregion
    }
}