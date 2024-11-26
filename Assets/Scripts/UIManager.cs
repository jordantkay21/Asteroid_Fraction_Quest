using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest
{
    public class UIManager : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.Instance.OnStart += InitializeUIManager;

            EventManager.Instance.OnAsteroidSpawned += UpdateAsteroidUI;
            EventManager.Instance.OnAsteroidSelected += ShowAsteroidDetails;
            EventManager.Instance.OnAsteroidHovered += HighlightAsteroid;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnStart -= InitializeUIManager;

            EventManager.Instance.OnAsteroidSpawned -= UpdateAsteroidUI;
            EventManager.Instance.OnAsteroidSelected -= ShowAsteroidDetails;
            EventManager.Instance.OnAsteroidHovered -= HighlightAsteroid;
        }

        private void InitializeUIManager()
        {
            //Setup UI elements
        }

        private void UpdateAsteroidUI(AsteroidManagement.AsteroidController asteroid)
        {
            // Update UI to display information about the spawned asteroid.
        }

        private void ShowAsteroidDetails(AsteroidManagement.AsteroidController asteroid)
        {
            // Show details of the selected asteroid.
        }

        private void HighlightAsteroid(AsteroidManagement.AsteroidController asteroid)
        {
            // Highlight the hovered asteroid visually.
        }
    }
}