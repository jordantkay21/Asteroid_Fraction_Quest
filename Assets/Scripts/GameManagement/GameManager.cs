using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KayosStudios.AsteroidQuest.AsteroidManagement;
using KayosStudios.AsteroidQuest.DataManagement;

namespace KayosStudios.AsteroidQuest.GameManagement
{
    public enum ObjectiveTypes
    {
        OrbSelection
    }

    public class GameManager : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.Instance.OnOrbSelected += HandleOrbSelection;
        }

        private void HandleOrbSelection()
        {
            Debug.Log("Interacted with Orb Instance");
        }

        void Update()
        {
            if (DataController.Instance.SelectedOrbCount == AsteroidManager.Instance.TotalOrbsSpawned)
            {
                EventManager.Instance.ObjectiveCompleted(ObjectiveTypes.OrbSelection);
                Debug.Log("All Orbs Selected!");
            }
        }
    }
}