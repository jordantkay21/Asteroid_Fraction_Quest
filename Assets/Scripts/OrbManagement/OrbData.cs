using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbData : MonoBehaviour
{
    [Header("Cell Stats")]
    [SerializeField] int cellSeed = 0;
    [Tooltip("Defines the minimum number of cells for the grid")]
    [SerializeField] int minCells;
    [Tooltip("Defines the maximum number of cells for the grid")]
    [SerializeField] int maxCells;
    [Tooltip("Total number of cells the orb contains")]
    [SerializeField] int cellCount;

    [Header("Grid Stats")]
    [SerializeField] int rows;
    [SerializeField] int columns;

    private void DefineCellCount()
    {
        if(cellSeed == 0)
        {
            cellSeed = System.DateTime.Now.Millisecond;
        }
    }

}
