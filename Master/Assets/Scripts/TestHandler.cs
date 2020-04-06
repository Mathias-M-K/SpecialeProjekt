using System.Collections.Generic;
using CoreGame;
using UnityEngine;


public class TestHandler : MonoBehaviour
{
    [Space] [Header("Other")] [Range(0f, 3f)]
    public float sequenceDelay;

    public PlayerController agent1;

    
    private bool _serverActive;

    public MapManager _mapManager;




    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            agent1.MovePlayer(Direction.Left);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            agent1.MovePlayer(Direction.Right);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            agent1.MovePlayer(Direction.Up);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            agent1.MovePlayer(Direction.Down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (PlayerController controller in GameHandler.Current.GetPlayers())
            {
                controller.Ready = true;
            }

            
            GameHandler.Current.delayBetweenMoves = sequenceDelay;
            //StartCoroutine(GameHandler.current.PerformSequence());
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

   

        if (Input.GetKeyDown(KeyCode.T))
        {
            _mapManager.GenerateMapValues();
        }
    }
}