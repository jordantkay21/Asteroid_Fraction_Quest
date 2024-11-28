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

        //event for when an asteroid is selected
        public event Action<AsteroidData> OnAsteroidSelected;


        private void Start()
        {
            //Trigger OnStart event to notify all subscribers
            OnStart?.Invoke();
        }

        //Methods to invoke events
        public void TriggerAsteroidSelected(AsteroidData asteroid)
        {
            OnAsteroidSelected?.Invoke(asteroid);
        }
    }
}