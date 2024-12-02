using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.AsteroidManagement
{
    public class OrbController : MonoBehaviour, ISelectable
    {
        [SerializeField] Material defaultMat;
        [SerializeField] Material selectedMat;
        [SerializeField] GameObject cellPrefab;
        [SerializeField] float orbRadius;
        public bool isSelected { get; private set; }

        private List<CellController> _spawnedCells = new List<CellController>();

        public void OnSelect()
        {
            Debug.Log($"Orb {name} selected.");

            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                if (!isSelected)
                {
                    isSelected = true;
                    renderer.material = selectedMat;
                }
                else
                {
                    isSelected = false;
                    renderer.material = defaultMat; }
            }

            EventManager.Instance.TriggerOrbSelection(this);
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

    }
}