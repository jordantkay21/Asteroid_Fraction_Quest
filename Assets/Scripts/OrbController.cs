using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.OrbManagement
{
    public class OrbController : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;

        private List<CellController> spawnedCells = new List<CellController>();

        public void SpawnCells(int cellCount)
        {
            for (int i=0; i < cellCount; i++)
            {
                //Instantiate the cell as a child of the orb
                GameObject cell = Instantiate(cellPrefab, transform);

                //Randomize position within the orb
                Vector3 randomPosition = Random.insideUnitSphere * 0.5f;
                cell.transform.localPosition = randomPosition;

                //Add cell to list for future reference
                CellController cellController = cell.GetComponent<CellController>();
                spawnedCells.Add(cellController);
            }

            Debug.Log($"Orb contains <color=orange>{cellCount} cells</color> based on asteroid type.");
        }

        public void EnergizedCells(int energizedCellCount)
        {
            if (energizedCellCount > spawnedCells.Count)
            {
                Debug.LogWarning("Energized cell count exceeds total cells. Adjusting to max");
                energizedCellCount = spawnedCells.Count;
            }

            //Shuffle and select random cells to energize
            List<CellController> shuffledCells = new List<CellController>(spawnedCells);
            for(int i = 0; i < energizedCellCount; i++)
            {
                int randomIndex = Random.Range(0, shuffledCells.Count);
                CellController cellToEnergize = shuffledCells[randomIndex];
                cellToEnergize.SetEnergized(true);
                shuffledCells.RemoveAt(randomIndex); //prevent duplicate energization
            }

            Debug.Log($"Energized <color=yellow>{energizedCellCount} cells</color>");
        }
    }
}