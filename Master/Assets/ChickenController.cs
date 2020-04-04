using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ChickenController : MonoBehaviourPunCallbacks
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
    
    private List<Color32> colors = new List<Color32>(){new Color32(255,162,162,255),new Color32(162,214,255,255),new Color32(132,255,140,255),new Color32(255,255,255,255)};
    private int colorCounter = 0;
    



    
    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        controller = GetComponent<CharacterController>();

        if (_photonView.IsMine) Camera.main.GetComponent<CustomCameraMovement>().SetChicken(gameObject);
    }

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!_photonView.IsMine) return;
            ChangeColor(_photonView.ViewID);
            
            _photonView.RPC("ChangeColor",RpcTarget.Others,_photonView.ViewID);
        }
        
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
    
    [PunRPC]
    private void ChangeColor(int viewId)
    {
        if (photonView.ViewID != viewId) return;
        GetComponent<Renderer>().material.color = colors[colorCounter];

        colorCounter++;

        if (colorCounter > 3)
        {
            colorCounter = 0;
        }
    }
}
