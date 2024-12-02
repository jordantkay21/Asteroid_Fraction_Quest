using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest
{
    public class DataManager 
    {
        AsteroidData _selectedAsteroid;
        public void SetSelectedAsteroid(AsteroidData asteroid)
        {
            _selectedAsteroid = asteroid;
        }

        public AsteroidData GetSelectedAsteroid()
        {
            return _selectedAsteroid;
        }
    }
}
