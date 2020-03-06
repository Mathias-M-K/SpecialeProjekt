using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoMovingPlayer : MonoBehaviour
{
    public Camera camera;
    public NavMeshAgent agent;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                print(hit.point);
                agent.SetDestination(hit.point);
            }
        }

        StartCoroutine(SetRandomPosition());
    }

    private IEnumerator SetRandomPosition()
    {
        

        while (true)
        {
            float x = Random.Range(2.1f, 9.9f);
            float z = Random.Range(2.1f, 9.9f);
        
            agent.SetDestination(new Vector3(x,1.5f,z));

            yield return new WaitForSeconds(5);
        }
    }
}
