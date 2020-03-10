using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshManager : MonoBehaviour
{
    public bool ready;
    
    private void Awake()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
        ready = true;
    }

    // Update is called once per frame
    private void Update()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
