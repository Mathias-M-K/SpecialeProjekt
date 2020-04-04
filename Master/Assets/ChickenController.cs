using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ChickenController : MonoBehaviour
{
    private PhotonView _photonView;
    
    public CharacterController controller;
    private float velX;
    private float velY;
    private float velZ;

    private Vector3 _actualVel;



    public float maxSpeed;
    public float jumpSpeed;
    public float speed;
    public float gravity;
    public float deAccValue;
    
    private bool _movement;
    



    
    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        controller = GetComponent<CharacterController>();
        Camera.main.GetComponent<CustomCameraMovement>().SetChicken(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_photonView.IsMine) return;

            /*_actualVel = controller.velocity;
            if (_actualVel.x != 0)
            {
                _movement = true;
            }
            if (_movement)
            {
                if (_actualVel.x == 0.0f)
                {
                    velX = 0;
                }
            }*/
        
        
        //Ground Movement Control
        if (Input.GetKey(KeyCode.UpArrow))
        {
            velZ += speed;
            if (velZ > maxSpeed)
            {
                velZ = maxSpeed;
            }
        }else if (Input.GetKey(KeyCode.DownArrow))
        {
            velZ -= speed;
            if (velZ < -maxSpeed)
            {
                velZ = -maxSpeed;
            }
        }
        else
        {
            if (Math.Abs(velZ - 0) < 1)
            {
                velZ = 0;
            }
            if (velZ > 0)
            {
                velZ -= deAccValue;
            }

            if (velZ < 0)
            {   
                velZ += deAccValue;
            }
        }
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            velX -= speed;
            if (velX < -maxSpeed)
            {
                velX = -maxSpeed;
            }
        }else if (Input.GetKey(KeyCode.RightArrow))
        {
            velX += speed;
            if (velX > maxSpeed)
            {
                velX = maxSpeed;
            }
            velX += speed;
        }
        else
        {
            if (Math.Abs(velX - 0) < 1)
            {
                velX = 0;
            }
            if (velX > 0)
            {
                velX -= deAccValue;
            }

            if (velX < 0)
            {   
                velX += deAccValue;
            }
        }

        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velY = jumpSpeed;
            }
        }

        if (velX > maxSpeed)
        {
            
        }
        

        velY -= gravity * Time.deltaTime;
        
        Vector3 newPosition = new Vector3(velX, 0.0f, velZ);
        transform.LookAt(newPosition + transform.position);
        
        
        controller.Move(new Vector3(velX * Time.deltaTime, velY * Time.deltaTime, velZ * Time.deltaTime));

        
    }
}
