using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHandler : MonoBehaviour
{
    public PlayerController playerController;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            playerController.MovePlayer(Direction.LEFT);
        }  
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            playerController.MovePlayer(Direction.RIGHT);
        }  
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerController.MovePlayer(Direction.UP);
        }  
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            playerController.MovePlayer(Direction.DOWN);
        }  
    }
}
