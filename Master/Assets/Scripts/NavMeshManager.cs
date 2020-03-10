using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshManager : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    // Update is called once per frame
    private void Update()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
