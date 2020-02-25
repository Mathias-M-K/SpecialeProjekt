using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExternalPlayerControl : MonoBehaviour
{
    [Header("Set Player")] public PlayerColor player;
    [Space]
    
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button btn4;

    public Button tradeBtn;
    public Button acceptBtn;
    public Button rejectBtn;
    
    List<Direction> moves = new List<Direction>();
    
    
    // Start is called before the first frame update
    void Start()
    {
        moves.Add(Direction.RIGHT);
        moves.Add(Direction.LEFT);
        moves.Add(Direction.UP);
        moves.Add(Direction.DOWN);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick()
    {
        GameObject go = EventSystem.current.currentSelectedGameObject;
        PlayerMove btnAttributes = go.GetComponent<PlayerMove>();
        if (go != null && btnAttributes.player == player)
        {
            Debug.Log("Clicked on : "+ go.name);
            Debug.Log(go.GetComponent<PlayerMove>().player);
            Debug.Log(go.GetComponent<PlayerMove>().direction);
        }
        else
            Debug.Log("currentSelectedGameObject is null");
    }
}
