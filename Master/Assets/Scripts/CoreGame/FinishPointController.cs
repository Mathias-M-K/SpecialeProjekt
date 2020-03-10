using System;
using System.Collections.Generic;
using CoreGame.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoreGame
{
    public class FinishPointController : MonoBehaviour
    {
        readonly List<IFinishPointObserver> _observers = new List<IFinishPointObserver>();

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(new Vector3(0,2,0),Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.gameHandler.NotifyGameProgressObservers(playerController.player);
            playerController.Die();
        }
    }
}
