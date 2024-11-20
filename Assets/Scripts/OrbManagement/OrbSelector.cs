using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KayosStudios.AsteroidQuest.DataManagement;

namespace KayosStudios.AsteroidQuest.OrbManagement
{
    public class OrbSelector : MonoBehaviour
    {
        [Tooltip("Reference to the material for the orb when it is selected")]
        [SerializeField] private Material selectedMaterial;
        [Tooltip("Reference to the original material for the orb")]
        [SerializeField] private Material defaultMaterial;

        private bool isSelected = false;
        private Renderer orbRenderer;

        private void Start()
        {
            //Get the Renderer component to cahnge the orb's material
            orbRenderer = GetComponent<Renderer>();
            orbRenderer.material = defaultMaterial;
        }

        private void OnMouseDown()
        {
            //Toggle selection status
            isSelected = !isSelected;

            EventManager.Instance.OrbSelected();

            // Change the orb's color based on its selection status
            if (isSelected)
            {
                orbRenderer.material = selectedMaterial;
                DataController.Instance.IncrementSelectedCount();
            }
            else
            {
                orbRenderer.material = defaultMaterial;
                DataController.Instance.DecrementSelectedCount();
            }
        }
    }
}