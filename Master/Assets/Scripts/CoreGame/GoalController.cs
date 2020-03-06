using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using UnityEngine;

public class GoalController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,2,0),Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        
        other.GetComponent<PlayerController>().Die();
    }
}
