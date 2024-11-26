using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.OrbManagement
{
    public class OrbController : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;

        public void SpawnCells(int cellCount)
        {

            for (int i=0; i < cellCount; i++)
            {
                //Instantiate the cell as a child of the orb
                GameObject cell = Instantiate(cellPrefab, transform);

                //Randomize position within the orb
                Vector3 randomPosition = Random.insideUnitSphere * 0.5f;
                cell.transform.localPosition = randomPosition;
            }

            Debug.Log($"Orb contains <color=orange>{cellCount} cells</color> based on asteroid type.");
        }
    }
}