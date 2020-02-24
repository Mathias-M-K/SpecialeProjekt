using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    
    
    public Camera cam;
    public NavMeshAgent agent;
    public int mapSize;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit-Point: "+hit.point);
                agent.SetDestination(CalculateGridPos(hit.point));
            }
        }
    }

    //Returns true when done moving
    public void MovePlayer(Direction d)
    {
        Vector3 newGridPos;
        
        switch (d)
        {
            case Direction.UP:
                newGridPos = new Vector3(agent.destination.x,agent.destination.y,agent.destination.z+1);
                break;
            case Direction.DOWN:
                newGridPos = new Vector3(agent.destination.x,agent.destination.y,agent.destination.z-1);
                break;
            case Direction.LEFT:
                newGridPos = new Vector3(agent.destination.x-1,agent.destination.y,agent.destination.z);
                break;
            case Direction.RIGHT:
                newGridPos = new Vector3(agent.destination.x+1,agent.destination.y,agent.destination.z);
                break;
            default:
                newGridPos = new Vector3(agent.destination.x,agent.destination.y,agent.destination.z);
                break;                
        }

        if (newGridPos.x > mapSize+1 || newGridPos.x < 1 || newGridPos.y > mapSize+1 || newGridPos.y < 1)
        {
            throw new ArgumentException("Position out of bounds");
        }
        
        NavMeshPath navMeshPath = new NavMeshPath();
        agent.CalculatePath(newGridPos, navMeshPath);
        
        if(navMeshPath.status == NavMeshPathStatus.PathInvalid)
        {
            throw new ArgumentException("Position not reachable");
        }

        agent.SetDestination(newGridPos);
    }

    public void MoveToPos(float x, float z)
    {
        agent.SetDestination(new Vector3(x,1.5f,z));
    }

    private Vector3 CalculateGridPos(Vector3 point)
    {
        double x = Math.Floor(point.x)+0.5f;
        double z = Math.Floor(point.z)+0.5f;
        
        Vector3 gridPos = new Vector3((float)x,point.y,(float)z);

        return gridPos;
    }
}

public enum Direction
{
    UP,
    DOWN,
    RIGHT,
    LEFT
}
