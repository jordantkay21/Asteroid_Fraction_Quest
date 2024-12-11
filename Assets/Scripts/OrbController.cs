using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.AsteroidManagement
{
    [RequireComponent(typeof(MeshRenderer))]
    public class OrbController : MonoBehaviour, ISelectable
    {
        [Header("Materials")]
        [SerializeField] MeshRenderer renderer;
        [SerializeField] Material defaultMat;
        [SerializeField] Material selectedMat;
        [SerializeField] Material singleHitMat;
        [SerializeField] Material doubleHitMat;

        [Header("Settings")]
        [SerializeField] GameObject cellPrefab;
        [SerializeField] float orbRadius;
        [SerializeField] int hitCount = 0;

        public bool isSelected { get; private set; }
        private List<CellController> _spawnedCells = new List<CellController>();

        private void Start()
        {
            renderer = GetComponent<MeshRenderer>();
        }

        public void OnSelect()
        {
            GamePhase currentPhase = GameManager.Instance.currentPhase;

            Debug.Log($"Orb {name} selected in phase {currentPhase}.");

            switch (currentPhase)
            {
                case GamePhase.S1_PhaseTwo:
                    HandlePhaseTwoSelection();
                    break;

                case GamePhase.S1_PhaseThree:
                    HandlePhaseThreeSelection();
                    break;

                default:
                    Debug.LogWarning($"Orb selection is not valid in during {currentPhase}.");
                    break;
            }


        }

        private void HandlePhaseTwoSelection()
        {
            if (!isSelected)
            {
                isSelected = true;
                renderer.material = selectedMat;
            }
            else
            {
                isSelected = false;
                renderer.material = defaultMat;
            }


            EventManager.Instance.TriggerOrbSelection(this);
        }

        private void HandlePhaseThreeSelection()
        {
            hitCount++;

            Debug.Log($"Orb {name} has been hit {hitCount} times.");

            if (hitCount == 1)
                renderer.material = singleHitMat;
            else if (hitCount == 2)
                renderer.material = doubleHitMat;
            else if (hitCount == 3)
            {
                Debug.Log($"Processing third hit logic for {name}.");

                // Step 1: Create a new parent object for the cells
                GameObject cellParent = new GameObject($"{name}_Cells");
                cellParent.transform.position = transform.position; // Place at the orb's position

                // Step 2: Separate energized and non-energized cells
                List<CellController> energizedCells = new List<CellController>();
                List<CellController> nonEnergizedCells = new List<CellController>();

                foreach (var cell in _spawnedCells)
                {
                    cell.StopMovement();

                    if (cell.IsEnergized())
                        energizedCells.Add(cell);
                    else
                        nonEnergizedCells.Add(cell);
                }

                // Step 3: Reparent and stack cells
                float yOffset = 0.2f; // Adjust based on cell size
                float baseY = transform.position.y;

                // Non-energized cells (on top)
                for (int i = 0; i < nonEnergizedCells.Count; i++)
                {
                    var cell = nonEnergizedCells[i];
                    cell.transform.SetParent(cellParent.transform);
                    cell.transform.position = new Vector3(transform.position.x, baseY + (i * yOffset), transform.position.z);
                }

                // Energized cells (on bottom)
                for (int i = 0; i < energizedCells.Count; i++)
                {
                    var cell = energizedCells[i];
                    cell.transform.SetParent(cellParent.transform);
                    cell.transform.position = new Vector3(transform.position.x, baseY - ((i + 1) * yOffset), transform.position.z);
                }

                // Step 4: Disable the orb
                gameObject.SetActive(false);
            }
        }

        public void SpawnCells(int cellCount)
        {
            for (int i=0; i < cellCount; i++)
            {
                //Instantiate the cell as a child of the orb
                GameObject cell = Instantiate(cellPrefab, transform);

                //Randomize position within the orb
                Vector3 randomPosition = Random.insideUnitSphere * 0.5f;
                cell.transform.localPosition = randomPosition;

                Vector3 randomDirection = Random.onUnitSphere; //Random initial direction

                //Initialize the cell
                CellController cellController = cell.GetComponent<CellController>();
                cellController.Initilize(randomDirection, orbRadius);

                //Add cell to list for tracking
                _spawnedCells.Add(cellController);
            }

            Debug.Log($"Orb contains <color=orange>{cellCount} cells</color> based on asteroid type.");
        }

        public void EnergizedCells(int energizedCellCount)
        {
            if (energizedCellCount > _spawnedCells.Count)
            {
                Debug.LogWarning("Energized cell count exceeds total cells. Adjusting to max");
                energizedCellCount = _spawnedCells.Count;
            }

            //Shuffle and select random cells to energize
            List<CellController> shuffledCells = new List<CellController>(_spawnedCells);
            for(int i = 0; i < energizedCellCount; i++)
            {
                int randomIndex = Random.Range(0, shuffledCells.Count);
                CellController cellToEnergize = shuffledCells[randomIndex];
                cellToEnergize.SetEnergized(true);
                shuffledCells.RemoveAt(randomIndex); //prevent duplicate energization
            }

            Debug.Log($"Energized <color=yellow>{energizedCellCount} cells</color>");
        }

        public List<CellData> GenerateCellData(out int cellTotal)
        {
            List<CellData> cellDataList = new List<CellData>();

            for(int i=0; i < _spawnedCells.Count; i++)
            {
                var cell = _spawnedCells[i];

                //Generate data for each cell
                CellData cellData = new CellData
                {
                    isEnergized = cell.IsEnergized()
                };

                //Assign a unique name to the orb
                cell.name = $"cell{i}.{name}";
            }

            cellTotal = cellDataList.Count;

            return cellDataList;

        }

        public void ResetMat()
        {
            renderer.material = defaultMat;
        }

    }
}