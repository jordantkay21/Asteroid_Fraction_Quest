using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.OrbManagement
{
    /// <summary>
    /// Handles claculating valid positions for each orb on the asteroid's surface. Ensures that orbs do not overlap, maintaining visual clarity and gameplay fairness.
    /// </summary>
    public class OrbPlacementHandler : MonoBehaviour
    {
        [Tooltip("Minimum required spacing between orbs to prevent them from appearing clustereed or overlapping")]
        public float minOrbSpacing; 
    }
}