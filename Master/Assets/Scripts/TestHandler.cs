using CoreGame;
using UnityEngine;

public class TestHandler : MonoBehaviour
{
    [Range(0f, 3f)] [SerializeField] private float sequenceDelay;
    public PlayerController agent;

    public GameHandler gameHandler;

    private void Start()
    {
        gameHandler.AddMove(Player.Yellow,Direction.Left);
        
        gameHandler.AddMove(Player.Red,Direction.Down);
        
        gameHandler.AddMove(Player.Blue,Direction.Up);
        gameHandler.AddMove(Player.Blue,Direction.Up);
        gameHandler.AddMove(Player.Blue,Direction.Up);
        gameHandler.AddMove(Player.Blue,Direction.Right);
        
        gameHandler.AddMove(Player.Green,Direction.Up);
        
        gameHandler.AddMove(Player.Yellow,Direction.Left);
        gameHandler.AddMove(Player.Yellow,Direction.Left);
        
        gameHandler.AddMove(Player.Red,Direction.Down);
        
        gameHandler.AddMove(Player.Green,Direction.Up);
        
        gameHandler.AddMove(Player.Red,Direction.Right);
        
        gameHandler.AddMove(Player.Green,Direction.Left);
        
        gameHandler.AddMove(Player.Red,Direction.Right);
        
        gameHandler.AddMove(Player.Yellow,Direction.Down);
        
        gameHandler.AddMove(Player.Green,Direction.Left);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            agent.MovePlayer(Direction.Left);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            agent.MovePlayer(Direction.Right);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            agent.MovePlayer(Direction.Up);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            agent.MovePlayer(Direction.Down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(gameHandler.PerformMoves(sequenceDelay));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            GoHome();
        }
    }

    void GoHome()
    {
        agent.MoveToPos(1.5f,9.5f);

    }
}