using System;
using UnityEngine;

namespace CoreGame
{
    public class TriggerController : MonoBehaviour
    {
        public Player owner;
        public WallController wall;
        public bool closeOnExit;

        private void Awake()
        {
            wall.SetOwner(owner);
            LeanTween.color(gameObject, ColorPalette.current.GetPlayerColor(owner), 1);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController.player == owner)
            {
                wall.Open();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!closeOnExit) return;
            
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController.player == owner)
            {
                wall.Close();
            }
        }
    }
}
