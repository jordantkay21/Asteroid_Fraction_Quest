using UnityEngine;
using System;
using KayosStudios.AsteroidQuest.AsteroidManagement;

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

        //event for when an asteroid is selected
        public event Action<AsteroidData> OnAsteroidSelection;
        public void TriggerAsteroidSelection(AsteroidData asteroid) => OnAsteroidSelection?.Invoke(asteroid);

        //event for when an orb is selected
        public event Action<OrbController> OnOrbSelection;
        public void TriggerOrbSelection(OrbController orb) => OnOrbSelection?.Invoke(orb);

        private void Start()
        {
            //Trigger OnStart event to notify all subscribers
            OnStart?.Invoke();
        }

        //Methods to invoke events
    }
}