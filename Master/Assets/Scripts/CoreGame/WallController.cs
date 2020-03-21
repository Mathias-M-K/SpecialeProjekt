using System;
using UnityEngine;

namespace CoreGame
{
    public class WallController : MonoBehaviour
    {
        public Player owner; 
        public Direction openDirection;
        public LeanTweenType easeMethod;
        
        public float smoothingFactor;
        private Vector3 _openPosition;
        private Vector3 closedPosition;

        // Start is called before the first frame update
        private void Start()
        {
            closedPosition = transform.position;
            DetermineOpenPos(openDirection);
        }

        public void Open()
        {
            LeanTween.move(gameObject, _openPosition, smoothingFactor).setEase(easeMethod);
        }

        public void Close()
        {
            LeanTween.move(gameObject, closedPosition, smoothingFactor).setEase(easeMethod);
        }

        public void SetOwner(Player newOwner)
        {
            owner = newOwner;
            LeanTween.color(gameObject, ColorPalette.current.GetPlayerColor(newOwner), 1);
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
