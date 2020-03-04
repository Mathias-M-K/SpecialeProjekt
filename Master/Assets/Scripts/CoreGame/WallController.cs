using System;
using UnityEngine;

namespace CoreGame
{
    public class WallController : MonoBehaviour
    {
        public float smoothingFactor;
        public Vector3 openPosition;
        [SerializeField]private Vector3 closedPosition;
        private Vector3 _targetPos;

        // Start is called before the first frame update
        private void Start()
        {
            closedPosition = transform.position;
            _targetPos = closedPosition;
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
            _targetPos = openPosition;
        }

        public void Close()
        {
            _targetPos = closedPosition;
        }
    }
}
