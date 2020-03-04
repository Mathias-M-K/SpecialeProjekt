using System;
using UnityEngine;

namespace CoreGame
{
    public class TriggerController : MonoBehaviour
    {
        public Player color;
        public WallController wall;
        public bool closeOnExit;
        
        private void OnTriggerEnter(Collider other)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController.player == color)
            {
                wall.Open();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!closeOnExit) return;
            
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController.player == color)
            {
                wall.Close();
            }
        }
    }
}
