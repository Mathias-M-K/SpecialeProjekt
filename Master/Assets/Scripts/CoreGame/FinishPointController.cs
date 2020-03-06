using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using CoreGame.Interfaces;
using UnityEngine;

public class FinishPointController : MonoBehaviour
{
    List<IFinishPointObserver> _observers = new List<IFinishPointObserver>();
    private int _nrOfFinishedPlayers;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,2,0),Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerController>().Die();
        _nrOfFinishedPlayers++;
        NotifyObservers();
    }

    public void AddObserver(IFinishPointObserver observer)
    {
        _observers.Add(observer);
    }

    public void NotifyObservers()
    {
        foreach (IFinishPointObserver observer in _observers)
        {
            observer.GameProgressUpdate(_nrOfFinishedPlayers);
        }
    }
}
