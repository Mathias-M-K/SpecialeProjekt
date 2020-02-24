using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceHandler : MonoBehaviour
{
    private List<PlayerMove> playerMoves = new List<PlayerMove>();
    private List<PlayerController> players = new List<PlayerController>();

    public PlayerController pcRed;
    public PlayerController pcGreen;
    public PlayerController pcBlue;
    public PlayerController pcYellow;

    private void Start()
    {
        AddActivePlayersToList(pcRed);
        AddActivePlayersToList(pcBlue);
        AddActivePlayersToList(pcGreen);
        AddActivePlayersToList(pcYellow);
    }

    private void AddActivePlayersToList(PlayerController pc)
    {
        if (pc != null)
        {
            players.Add(pc);   
        }
    }

    public void AddMove(PlayerMove pm)
    {
        playerMoves.Add(pm);
    }

    public List<PlayerMove> GetMoves()
    {
        return playerMoves;
    }

    public IEnumerator ExcecuteMoves(float delayBetweenMoves)
    {

        PlayerController pcTemp;
        
        foreach(PlayerMove pm in playerMoves)
        {
            switch (pm.PlayerColor)
            {
                case PlayerColor.Red:
                    pcTemp = pcRed;
                    break;
                case PlayerColor.Blue:
                    pcTemp = pcBlue;
                    break;
                case PlayerColor.Green:
                    pcTemp = pcGreen;
                    break;
                case PlayerColor.Yellow:
                    pcTemp = pcYellow;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            pcTemp.MovePlayer(pm.Direction);
            yield return new WaitForSeconds(delayBetweenMoves);
            
        }
        playerMoves.Clear();
    }
}
