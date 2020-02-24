
using UnityEngine;

public class TestHandler : MonoBehaviour
{
    public PlayerController playerController;
    public SequenceHandler sh;

    private void Start()
    {
        sh.AddMove(new PlayerMove(PlayerColor.Red, Direction.DOWN));
        
        sh.AddMove(new PlayerMove(PlayerColor.Blue, Direction.UP));

        sh.AddMove(new PlayerMove(PlayerColor.Green, Direction.UP));
        sh.AddMove(new PlayerMove(PlayerColor.Green, Direction.LEFT));
        
        sh.AddMove(new PlayerMove(PlayerColor.Yellow, Direction.UP));
        sh.AddMove(new PlayerMove(PlayerColor.Yellow, Direction.UP));
        
        sh.AddMove(new PlayerMove(PlayerColor.Red, Direction.RIGHT));
        sh.AddMove(new PlayerMove(PlayerColor.Red, Direction.RIGHT));
        
        sh.AddMove(new PlayerMove(PlayerColor.Yellow, Direction.UP));
        sh.AddMove(new PlayerMove(PlayerColor.Yellow, Direction.RIGHT));
        
        sh.AddMove(new PlayerMove(PlayerColor.Blue, Direction.UP));
        sh.AddMove(new PlayerMove(PlayerColor.Blue, Direction.LEFT));

        sh.AddMove(new PlayerMove(PlayerColor.Yellow, Direction.RIGHT));
        sh.AddMove(new PlayerMove(PlayerColor.Yellow, Direction.UP));
        
        sh.AddMove(new PlayerMove(PlayerColor.Red, Direction.DOWN));
        
        sh.AddMove(new PlayerMove(PlayerColor.Blue, Direction.LEFT));
        sh.AddMove(new PlayerMove(PlayerColor.Blue, Direction.UP));

        sh.AddMove(new PlayerMove(PlayerColor.Yellow, Direction.UP));
    }

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(sh.ExcecuteMoves(0.5f));
        }
    }
}