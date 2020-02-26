using System;
using UnityEngine;
using UnityEngine.AI;

namespace CoreGame
{
    public class PlayerController : MonoBehaviour
    {
        public Player player;
        [Space]
        
        public Camera cam;
        public NavMeshAgent agent;

        [Space] [Header("Materials")] 
        public Material redMaterial;
        public Material blueMaterial;
        public Material greenMaterial;
        public Material yellowMaterial;
        
        private void Start()
        {
            SetColor();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.GetComponent<PlayerController>() != null)
                    {
                        throw new Exception("Can't move on top of another player");
                    }
                
                    //If wall is hit, y will be 2.5
                    if (hit.point.y == 2.5f)
                    {
                        throw new Exception("That's is a wall");
                    }
                
                    Debug.Log("Hit-Point: "+hit.point);
                    agent.SetDestination(CalculateGridPos(hit.point));
                }
            }
        }

        //Move player one unit in a given direction
        public void MovePlayer(Direction d)
        {
            //The new position
            Vector3 newGridPos;

            Vector3 agentPos = agent.destination;
            switch (d)
            {
                case Direction.Up:
                    newGridPos = new Vector3(agentPos.x,agentPos.y,agentPos.z+1);
                    break;
                case Direction.Down:
                    newGridPos = new Vector3(agentPos.x,agentPos.y,agentPos.z-1);
                    break;
                case Direction.Left:
                    newGridPos = new Vector3(agentPos.x-1,agentPos.y,agentPos.z);
                    break;
                case Direction.Right:
                    newGridPos = new Vector3(agentPos.x+1,agentPos.y,agentPos.z);
                    break;
                default:
                    newGridPos = new Vector3(agentPos.x,agentPos.y,agentPos.z);
                    break;                
            }
        
            //Creating navMeshPath and testing if is possible
            NavMeshPath navMeshPath = new NavMeshPath();
            agent.CalculatePath(newGridPos, navMeshPath);
        
        
            if(navMeshPath.status == NavMeshPathStatus.PathInvalid)
            {
                throw new ArgumentException("Position not reachable");
            }

            agent.SetDestination(newGridPos);
        }
    
        //Player object will find it's way to the position
        public void MoveToPos(float x, float z)
        {
            agent.SetDestination(new Vector3(x,1.5f,z));
        }

        //Calculate position on grid from mouse pointer
        private Vector3 CalculateGridPos(Vector3 point)
        {
            double x = Math.Floor(point.x)+0.5f;
            double z = Math.Floor(point.z)+0.5f;
        
            Vector3 gridPos = new Vector3((float)x,point.y,(float)z);

            return gridPos;
        }

        public void SetColor()
        {
            Material m;
            switch (player)
            {
                case Player.Red:
                    m = redMaterial;
                    break;
                case Player.Blue:
                    m = blueMaterial;
                    break;
                case Player.Green:
                    m = greenMaterial;
                    break;
                case Player.Yellow:
                    m = yellowMaterial;
                    break;
                default:
                    m = redMaterial;
                    break;
            }

            GetComponent<Renderer>().material = m;

        }
    
    }

    public enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }

    public enum Player
    {
        Red,
        Blue,
        Green,
        Yellow
    }
}