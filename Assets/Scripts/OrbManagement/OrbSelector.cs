using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KayosStudios.AsteroidQuest.DataManagement;
using KayosStudios.AsteroidQuest.GameManagement;

namespace KayosStudios.AsteroidQuest.OrbManagement
{
    public class OrbSelector : MonoBehaviour
    {
        [Tooltip("Reference to the material for the orb when it is selected")]
        [SerializeField] private Material selectedMaterial;
        [Tooltip("Reference to the original material for the orb")]
        [SerializeField] private Material defaultMaterial;

        private bool isSelected = false;
        private Renderer orbRenderer;
        private int rows;
        private int columns;

        private void Start()
        {
            //Get the Renderer component to cahnge the orb's material
            orbRenderer = GetComponent<Renderer>();
            orbRenderer.material = defaultMaterial;
        }

        public void InitilizeOrb(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            SetUpGrid();
        }

        private void SetUpGrid()
        {
            // Logic to set up the grid using rows and columns
            Debug.Log($"Setting up grid with {rows} rows and {columns} columns.");

            // Get the dimensions of the orb's rectangular cube
            float cubeWidth = transform.localScale.x;
            float cubeHeight = transform.localScale.y;

            // Define the border size (gap between cells)
            float borderSize = 0.05f;

            //Calculate cell dimensions
            float cellWidth = (cubeWidth / columns) - borderSize;
            float cellHeight = (cubeHeight / rows) - borderSize;
            float cellDepth = transform.localScale.z / Mathf.Max(rows, columns);

            //Loop through rows and colums to create cells
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col <columns; col++)
                {
                    //create a new cell
                    GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cell.transform.SetParent(transform); // Parent the cell to the orb

                    //Calculate the position of the cell
                    float xPosition = (col * (cellWidth + borderSize)) - (cubeWidth / 2) + (cellWidth / 2);
                    float yPosition = (row * (cellHeight + borderSize)) - (cubeHeight / 2) + (cellHeight / 2);
                    float zPosition = 0f;

                    //Set the position and scale of the cell
                    cell.transform.localPosition = new Vector3(xPosition, yPosition, zPosition);
                    cell.transform.localScale = new Vector3(cellWidth, cellHeight, cellDepth);

                    //Apply visual effects
                    Renderer cellRenderer = cell.GetComponent<Renderer>();
                    cellRenderer.material.color = Color.white;
                    
                }
            }
            // Add logic here to create the visual grid on the orb
        }

        private void OnMouseDown()
        {
            //Toggle selection status
            isSelected = !isSelected;

            EventManager.Instance.OrbSelected();

            // Change the orb's color based on its selection status
            if (isSelected)
            {
                orbRenderer.material = selectedMaterial;
                DataController.Instance.IncrementSelectedCount();
            }
            else
            {
                orbRenderer.material = defaultMaterial;
                DataController.Instance.DecrementSelectedCount();
            }
        }
    }
}