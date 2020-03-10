using Photon.Pun;
using UnityEngine;

namespace Networking.Game
{
    public class CubeController : MonoBehaviour
    {
        // Start is called before the first frame update

        private Rigidbody _rigidbody;
        public float forwardForce;
        public float turningForce;
        public float jumpForce;
        public bool spacePressed;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }


        // Update is called once per frame
        void Update()
        {
            PhotonView pv = GetComponent<PhotonView>();

            if (pv != null)
            {
                if (!GetComponent<PhotonView>().IsMine)
                {
                    return;
                }
            }
            
            
        
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _rigidbody.AddForce(transform.forward*forwardForce);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                _rigidbody.AddForce(-transform.forward*forwardForce);
            }        

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _rigidbody.AddTorque(-transform.up*turningForce);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                _rigidbody.AddTorque(transform.up*turningForce);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                spacePressed = true;
                _rigidbody.AddForce(transform.up*jumpForce);
            }
        }
    }
}
