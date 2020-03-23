using System;
using UnityEngine;

namespace CoreGame
{
    public class GateController : MonoBehaviour
    {
        private Player owner;
        public Player Owner
        {
            get => owner;
            set
            {
                owner = value;
                LeanTween.color(gameObject, ColorPalette.current.GetPlayerColor(owner), 1);
            }
        }

        [SerializeField]private Direction openDirection;
        public Direction OpenDirection
        {
            get => openDirection;
            set
            {
                openDirection = value;
                DetermineOpenPos(value);
            }
        }
        public LeanTweenType easeMethod;
        
        public float animationTime;
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
            LeanTween.move(gameObject, _openPosition, animationTime).setEase(easeMethod);
        }

        public void Close()
        {
            LeanTween.move(gameObject, closedPosition, animationTime).setEase(easeMethod);
        }
        
        private void DetermineOpenPos(Direction d)
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
                case Direction.Blank:
                    throw new ArgumentException("Direction blank is not valid");
                default:
                    newGridPos = new Vector3(closedPosition.x, closedPosition.y, closedPosition.z);
                    break;
            }

            _openPosition = newGridPos;

        }
    }
}
