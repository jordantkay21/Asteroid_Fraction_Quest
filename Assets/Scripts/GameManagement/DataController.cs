using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.DataManagement
{
    public class DataController : MonoBehaviour
    {
        public static DataController Instance { get; private set; }

        public int SelectedOrbCount { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
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