using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayosStudios.AsteroidQuest.AsteroidManagement
{
    public class CellController : MonoBehaviour
    {
        [SerializeField] private GameObject energizedIcon;
        private bool isEnergized;

        [Header("Movement Properties")]
        [SerializeField] bool canMove = true;
        [Tooltip("Speed of the cell")]
        [SerializeField] float speed = 1f;
        [Tooltip("current movement direction")]
        [SerializeField] Vector3 direction;
        [Tooltip("Default radius of the orb")]
        [SerializeField] float orbRadius;

        public void Initilize(Vector3 startDirection, float radius)
        {
            direction = startDirection.normalized; //Ensures normalized direction
            orbRadius = radius; //Set orb radius
        }
        

        public void SetEnergized(bool energized)
        {
            isEnergized = energized;
            if (energizedIcon != null)
                energizedIcon.SetActive(isEnergized);
        }

        private void Update()
        {
            MoveCell();
        }

        private void MoveCell()
        {

            if (!canMove) return; //exit if mvoement is disabled

            //Move the cell
            transform.localPosition += direction * speed * Time.deltaTime;

            //Check if the cell hits the orb boundary
            if (transform.localPosition.magnitude > orbRadius)
            {
                // Reflect direction
                direction = Vector3.Reflect(direction, transform.localPosition.normalized);
                // Clamp position to prevent escaping the orb
                transform.localPosition = transform.localPosition.normalized * orbRadius;
            }
        }

        public bool IsEnergized()
        {
            return isEnergized;
        }

        public void StopMovement()
        {
            canMove = false;
        }
    }
}