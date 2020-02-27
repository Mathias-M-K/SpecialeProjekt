using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace CoreGame
{
    public class PlayerController : MonoBehaviour
    {
        public Player player;
        [Space]
        
        public NavMeshAgent agent;
        
        private Camera _cam;

        [SerializeField]private Direction[] moves = {Direction.Up,Direction.Down,Direction.Left,Direction.Right};
        
        // Update basically contains the "click with mouse" functionality, and that only
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

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
                    
                    agent.SetDestination(CalculateGridPos(hit.point));
                }
            }
        }

        public bool HaveMove(Direction d)
        {
            return moves.Any(cus => cus == d);
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

        public void SetColor(Material material)
        {
            GetComponent<Renderer>().material = material;

            switch (material.name)  
            {
                case "Blue":
                    player = Player.Blue;
                    break;
                case "Green":
                    player = Player.Green;
                    break;
                case "Red":
                    player = Player.Red;
                    break;
                case "Yellow":
                    player = Player.Yellow;
                    break;
                default:
                    throw new ArgumentException("Color not valid");
            }
        }
        
        public void SetCamera(Camera camera)
        {
            _cam = camera;
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