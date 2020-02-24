
using UnityEngine;

public class TestHandler : MonoBehaviour
{
    [Range(0f, 3f)] [SerializeField] private float sequenceDelay;
    public PlayerController playerRed;
    public PlayerController playerBlue;
    public PlayerController playerGreen;
    public PlayerController playerYellow;
    public SequenceHandler sh;

    private void Start()
    {
        sh.AddMove(new PlayerMove(PlayerColor.Red, Direction.DOWN));
        
        sh.AddMove(new PlayerMove(PlayerColor.Blue, Direction.UP));

        sh.AddMove(new PlayerMove(PlayerColor.Green, Direction.UP));
        sh.AddMove(new PlayerMove(PlayerColor.Green, Direction.LEFT));
        sh.AddMove(new PlayerMove(PlayerColor.Green, Direction.LEFT));
        sh.AddMove(new PlayerMove(PlayerColor.Green, Direction.LEFT));
        
        sh.AddMove(new PlayerMove(PlayerColor.Yellow, Direction.UP));
        sh.AddMove(new PlayerMove(PlayerColor.Yellow, Direction.UP));
        
        sh.AddMove(new PlayerMove(PlayerColor.Red, Direction.RIGHT));
        sh.AddMove(new PlayerMove(PlayerColor.Red, Direction.RIGHT));
        
        sh.AddMove(new PlayerMove(PlayerColor.Yellow, Direction.UP));
        sh.AddMove(new PlayerMove(PlayerColor.Yellow, Direction.RIGHT));
        
        sh.AddMove(new PlayerMove(PlayerColor.Blue, Direction.UP));
        sh.AddMove(new PlayerMove(PlayerColor.Blue, Direction.LEFT));
        
        sh.AddMove(new PlayerMove(PlayerColor.Red, Direction.DOWN));
        
        sh.AddMove(new PlayerMove(PlayerColor.Blue, Direction.LEFT));
        
  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            playerRed.MovePlayer(Direction.LEFT);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            playerRed.MovePlayer(Direction.RIGHT);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerRed.MovePlayer(Direction.UP);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            playerRed.MovePlayer(Direction.DOWN);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(sh.ExcecuteMoves(sequenceDelay));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            GoHome();
        }
    }

    void GoHome()
    {
        playerRed.MoveToPos(1.5f,9.5f);
        playerBlue.MoveToPos(10.5f,1.5f);
        playerGreen.MoveToPos(10.5f,9.5f);
        playerYellow.MoveToPos(1.5f,1.5f);
    }
}