using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.OrbManagement
{
    public class CellController : MonoBehaviour
    {
        [SerializeField] private GameObject energizedIcon;
        private bool isEnergized;

        public void SetEnergized(bool energized)
        {
            isEnergized = energized;
            if (energizedIcon != null)
                energizedIcon.SetActive(isEnergized);
        }

        public bool IsEnergized()
        {
            return isEnergized;
        }
    }
}