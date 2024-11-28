using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest
{
    public enum AsteroidType
    {
        Red,
        Blue,
        Green
    }

    [System.Serializable]
    public class AsteroidData
    {
        public AsteroidType asteroidType;
        public Vector3 position;
        public List<OrbData> orbs;
    }

    [System.Serializable]
    public class OrbData
    {
        public Vector3 position;
        public List<CellData> cells;
    }

    [System.Serializable]
    public class CellData
    {
        public bool isEnergized;
    }

    public class DataManager : MonoBehaviour
    {

        private void OnEnable()
        {
            EventManager.Instance.OnAsteroidSelected += (asteroid) => SetSelectedAsteroid(asteroid);
        }

        #region Selected Asteroid Data Management
        private AsteroidData _selectedAsteroid;
        public void SetSelectedAsteroid(AsteroidData asteroid)
        {
            _selectedAsteroid = asteroid;
        }

        public AsteroidData GetSelectedAsteroid()
        {
            return _selectedAsteroid;
        }
        #endregion
    }
}
