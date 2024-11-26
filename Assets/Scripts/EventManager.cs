using UnityEngine;
using System;

namespace KayosStudios.AsteroidQuest
{
    [DefaultExecutionOrder(-100)]
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        //event to notify when the EventManager starts
        public event Action OnStart;

        //event for when an asteroid is spawned
        public event Action<AsteroidManagement.AsteroidController> OnAsteroidSpawned;

        //event for when an asteroid is selected
        public event Action<AsteroidManagement.AsteroidController> OnAsteroidSelected;

        //event for when all asteroids are spawned
        public event Action OnAllAsteroidsSpawned;

        //event for when all asteroids are spawned
        public event Action<AsteroidManagement.AsteroidController> OnAsteroidHovered;

        private void Start()
        {
            //Trigger OnStart event to notify all subscribers
            OnStart?.Invoke();
        }

        //Methods to invoke events
        public void TriggerAsteroidSpawned(AsteroidManagement.AsteroidController asteroid)
        {
            OnAsteroidSpawned?.Invoke(asteroid);
        }

        public void TriggerAsteroidSelected(AsteroidManagement.AsteroidController asteroid)
        {
            OnAsteroidSelected?.Invoke(asteroid);
        }

        public void TriggerAllAsteroidsSpawned()
        {
            OnAllAsteroidsSpawned?.Invoke();
        }

        public void TriggerAsteroidHovered(AsteroidManagement.AsteroidController asteroid)
        {
            OnAsteroidHovered?.Invoke(asteroid);
        }


    }
}