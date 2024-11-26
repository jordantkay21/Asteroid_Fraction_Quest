using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.AsteroidManagement
{
    /// <summary>
    /// Controls the behavior of individual asteroids.
    /// </summary>
    public class AsteroidController : MonoBehaviour
    {
        [Header("Asteroid Properties")]
        public float minScale;
        public float maxScale;
        public enum AsteroidType
        {
            Red,
            Blue,
            Green
        }

        private AsteroidType _asteroidType;

        public void RandomizeScale()
        {
            float randomScale = Random.Range(minScale, maxScale);
            transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }

        public void AssignType()
        {
            //Randomly select a type from the AsteroidType enum
            _asteroidType = (AsteroidType)Random.Range(0, System.Enum.GetValues(typeof(AsteroidType)).Length);
            ApplyTypeVisuals(_asteroidType);
        }

        private void ApplyTypeVisuals(AsteroidType type)
        {
            Renderer renderer = GetComponent<Renderer>();
            switch (type)
            {
                case AsteroidType.Red:
                    renderer.material.color = Color.red;
                    break;
                case AsteroidType.Blue:
                    renderer.material.color = Color.blue;
                    break;
                case AsteroidType.Green:
                    renderer.material.color = Color.green;
                    break;
            }
        }

        public AsteroidType GetAsteroidType()
        {
            return _asteroidType;
        }

        private void OnMouseEnter()
        {
            //Trigger the asteroid hovered event
            EventManager.Instance.TriggerAsteroidHovered(this);
        }

        private void OnMouseDown()
        {
            //Trigger the asteroid selected event
            EventManager.Instance.TriggerAsteroidSelected(this);
        }
    }
}