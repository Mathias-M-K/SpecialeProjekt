using System;
using UnityEngine;

namespace CoreGame
{
    public class WallController : MonoBehaviour
    {
        public Player owner; 
        public float smoothingFactor;
        private Vector3 _openPosition;
        public Direction openDirection;
        [SerializeField]private Vector3 closedPosition;
        private Vector3 _targetPos;

        // Start is called before the first frame update
        private void Start()
        {
            closedPosition = transform.position;
            _targetPos = closedPosition;
            DetermineOpenPos(openDirection);
        }

        // Update is called once per frame
        void Update()
        {
            Move();
        }

        private void Move()
        {
            Vector3 pos = transform.position;
            
            float xPos = Mathf.Lerp(pos.x, _targetPos.x,Time.deltaTime * smoothingFactor);
            float yPos = Mathf.Lerp(pos.y, _targetPos.y,Time.deltaTime * smoothingFactor);
            float zPos = Mathf.Lerp(pos.z, _targetPos.z,Time.deltaTime * smoothingFactor);

            transform.position = new Vector3(xPos, yPos, zPos);
        }

        public void Open()
        {
            _targetPos = _openPosition;
        }

        public void Close()
        {
            _targetPos = closedPosition;
        }

        public void SetOwner(Player newOwner)
        {
            owner = newOwner;
        }
        
        public void DetermineOpenPos(Direction d)
        {
            //The new position
            Vector3 newGridPos;

            switch (d)
            {
                case Direction.Up:
                    newGridPos = new Vector3(closedPosition.x, closedPosition.y, closedPosition.z + 1);
                    break;
                case Direction.Down:
                    newGridPos = new Vector3(closedPosition.x, closedPosition.y, closedPosition.z - 1);
                    break;
                case Direction.Left:
                    newGridPos = new Vector3(closedPosition.x - 1, closedPosition.y, closedPosition.z);
                    break;
                case Direction.Right:
                    newGridPos = new Vector3(closedPosition.x + 1, closedPosition.y, closedPosition.z);
                    break;
                default:
                    newGridPos = new Vector3(closedPosition.x, closedPosition.y, closedPosition.z);
                    break;
            }

            _openPosition = newGridPos;

        }
    }
}
