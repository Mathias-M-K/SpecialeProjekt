using System.Collections.Generic;
using CoreGame;
using UnityEngine;


public class TestHandler : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            List<PlayerController> tempList = GameHandler.Current.GetPlayers();
            foreach (PlayerController controller in GameHandler.Current.GetPlayers())
            {
                controller.ready = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Vector2[] positions = GameHandler.Current.GetSpawnLocations();

            int i = 0;
            foreach (PlayerController playerController in GameHandler.Current.GetPlayers())
            {
                playerController.MoveToPos(positions[i].x, positions[i].y);
                i++;
            }
        }
    }
}