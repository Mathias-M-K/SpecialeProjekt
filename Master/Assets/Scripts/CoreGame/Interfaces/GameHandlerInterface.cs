using System;
using System.Collections;
using System.Collections.Generic;
using Container;
using DefaultNamespace;
using UnityEngine;

namespace CoreGame.Interfaces
{
    public interface IGameHandlerInterface
    {
    /// <summary>
    /// starts game
    /// </summary>
    void StartGame();

    /// <summary>
    /// Sets spawn positions and max allowed players
    /// </summary>
    /// <param name="mapData"></param>
    void SetMapData(MapData mapData);

    /// <summary>
    /// Checks if the given position is occupied by another player
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    bool IsPositionOccupied(Vector2 position);

    /// <summary>
    /// Lets player announce their new position
    /// </summary>
    /// <param name="playerTags"></param>
    /// <param name="position"></param>
    void RegisterPosition(PlayerTags playerTags, Vector2 position);

    /// <summary>
    /// Creates new PlayerTrade
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="directionIndex"></param>
    /// <param name="playerTagsReceiving"></param>
    /// <param name="playerTagsOffering"></param>
    /// <param name="tradeId"></param>
    void NewTrade(Direction direction, int directionIndex, PlayerTags playerTagsReceiving, PlayerTags playerTagsOffering, int tradeId);

    /// <summary>
    /// Returns all current playerTrades
    /// </summary>
    /// <returns></returns>
    List<PlayerTrade> GetTrades();
    
    /// <summary>
    /// Adds a move to the common sequence
    /// </summary>
    void AddMoveToSequence(PlayerTags p, Direction d, int index);

    /// <summary>
    /// Removes a move from the common sequence
    /// </summary>
    /// <param name="move"></param>
    void RemoveMoveFromSequence(StoredPlayerMove move);

    /// <summary>
    /// Performs the sequence, by moving the players in their desired directions
    /// </summary>
    /// <returns></returns>
    IEnumerator PerformSequence();

    /// <summary>
    /// Gets a specific playercontroller based on the playertag.
    /// </summary>
    /// <param name="p"></param>
    /// <returns>PlayerController</returns>
    PlayerController GetPlayerController(PlayerTags p);

    /// <summary>
    /// Returns the predefined spawn positions that are dictated by the mapdata file
    /// </summary>
    /// <returns></returns>
    Vector2[] GetSpawnLocations();

    /// <summary>
    /// Returns all moves that are currently in que to be played
    /// </summary>
    /// <returns>Player moves</returns>
    List<StoredPlayerMove> GetSequence();

    /// <summary>
    /// When the locally owned photonview spawns, it sets itself as the networked agent
    /// </summary>
    /// <param name="netController"></param>
    void SetNetworkedAgent(NetworkAgentController netController);

    /// <summary>
    /// Spawn the max amount of players that the mapdata dictates are allowed
    /// </summary>
    void SpawnMaxPlayers();

    /// <summary>
    /// Spawns a specific player on a preselected position
    /// </summary>
    /// <param name="playerTag"></param>
    void SpawnNewPlayer(PlayerTags playerTag);

    /// <summary>
    /// Spawns the next player in line, and returns said player
    /// </summary>
    /// <returns>Spawned player</returns>
    PlayerTags SpawnNewPlayer();

    /// <summary>
    /// Removes a player controller from _players list
    /// </summary>
    /// <param name="playerController"></param>
    void RemovePlayerController(PlayerController playerController);

    /// <summary>
    /// Returns all active player controllers
    /// </summary>
    /// <returns></returns>
    List<PlayerController> GetPlayers();


    //Add and notify methods for observers
    void AddSequenceObserver(ISequenceObserver iso);

    void AddTradeObserver(ITradeObserver ito);

    void AddGameProgressObserver(IFinishPointObserver ifo);

    void NotifySequenceObservers(SequenceActions sequenceAction, StoredPlayerMove move);

    void NotifyGameProgressObservers(PlayerTags player1);
    }
}