using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.DataManagement
{
    public class DataController : MonoBehaviour
    {
        public static DataController Instance { get; private set; }


        //Shared Orb Stats
        public int SelectedOrbCount { get; private set; }
        public int CellCount { get; private set; }

        [Header("Global Data")]
        [SerializeField] int orbSeed = 0;
        [SerializeField] int cellSeed = 0;

        [Header("Asteroid Scale Configuration")]
        [Tooltip("Minimum scale for the asteroid")]
        [SerializeField] float minScale = 5f;
        [Tooltip("Maximum scale for the asteroid")]
        [SerializeField] float maxScale = 15f;

        [Header("Orb Spawn Configuration")]
        [Tooltip("Minimum number of orbs allowed to spawn")]
        [SerializeField] int minOrb;
        [Tooltip("Maximum number of orbs allowed to spawn")]
        [SerializeField] int maxOrb;

        [Header("Cell Data Configuration")]
        [Tooltip("Minimum number of cells an orb could have")]
        [SerializeField]  int minCells;
        [Tooltip("Maximum number of cells an orb could have")]
        [SerializeField]  int maxCells;
        [SerializeField] int rows;
        [SerializeField] int columns;

        public int MinOrb => minOrb;
        public int MaxOrb => maxOrb;
        public float MinScale => minScale;
        public float MaxScale => maxScale;
        public int MinCells => minCells;
        public int MaxCells => maxCells;
        public int Rows => rows;
        public int Columns => columns;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeOrbStats();
            }

            else
                Destroy(gameObject);
        }

        public int CalculateOrbCount()
        {
            orbSeed = System.DateTime.Now.Millisecond;
            Debug.Log($"Current Orb Seed = {orbSeed}");


            // Initialize the random number generator with the seed
            Random.InitState(orbSeed);
            return Random.Range(minOrb, maxOrb);

        }

        private void InitializeOrbStats()
        {
            cellSeed = System.DateTime.Now.Millisecond;
            Debug.Log($"Current Cell Seed = {cellSeed}");

            Random.InitState(cellSeed);
            CellCount = Random.Range(minCells, maxCells);
            CalculateGridDimensions();
        }

        private void CalculateGridDimensions()
        {
            rows = Mathf.CeilToInt(Mathf.Sqrt(CellCount));
            columns = Mathf.CeilToInt((float)CellCount / Rows);
        }

        public void IncrementSelectedCount()
        {
            SelectedOrbCount++;
            Debug.Log($"Selected Orbs: {SelectedOrbCount}");
        }

        public void DecrementSelectedCount()
        {
            SelectedOrbCount--;
            Debug.Log($"Selected Orbs: {SelectedOrbCount}");
        }
    }
}