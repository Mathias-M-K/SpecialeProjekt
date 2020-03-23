using System;
using UnityEngine;

namespace CoreGame
{
    public class TriggerController : MonoBehaviour
    {
        public Player owner;
        public GateController gate;
        public bool closeOnExit;

        private void Awake()
        {
            gate.Owner = owner;
            LeanTween.color(gameObject, ColorPalette.current.GetPlayerColor(owner), 1);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController.player == owner)
            {
                gate.Open();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!closeOnExit) return;
            
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController.player == owner)
            {
                gate.Close();
            }
        }
    }
}
