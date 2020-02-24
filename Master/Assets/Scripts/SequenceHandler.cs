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

    public void ExcecuteMoves()
    {
        
    }
}
