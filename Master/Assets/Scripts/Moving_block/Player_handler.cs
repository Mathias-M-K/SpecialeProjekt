using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player_handler : MonoBehaviour
{
    public Player_handler()
    {
        
    }
    private List<Block> listOfBlock = new List<Block>();
    private int maxAmountOfBlocks = 4;

    public void AddBlock(string id)
    {
        bool colorPicker = GameObject.Find("ScriptHolder").GetComponent<ArduinoCommunication>().scanByColor;
        
        //Had to trim this fucking string - caused random errors. Fuck unity.
        string input = id.Trim();
        if (!ContainsElement(input) && listOfBlock.Count < maxAmountOfBlocks)
        {
            string state = "";
            if (colorPicker)
            {
                state = DetermineMoveByColor(input);

            }
            else
            {
                state = DetermineMove();
            }
            Block _block = new Block(input, state);
            listOfBlock.Add(_block);
            print($"added block with id {input} and it's state is: {state}");
            
        }
    }

    public void SwapBlocks(string id, string state)
    {
        foreach (var block in listOfBlock.Where(block => block.Id.Equals(id)))
        {
            block.CurrentState = state;
        }
    }

    private bool ContainsElement(string _id)
    {
        return listOfBlock.Any(block => _id.Equals(block.Id));
    }

    private string DetermineMoveByColor(string id)
    {
        switch (id)
        {
            //yellow is up
            case "6978301": return "up";
            //white is right
            case "6983496": return "right";
            //red is down
            case "1929590": return "down";
            //black is left
            case "6995009": return "left";
            default: return null;
        }
    }

    private string DetermineMove()
    {
        switch (listOfBlock.Count)
        {
            //yellow is up
            case 0: return "up";
            //white is right
            case 1: return "right";
            //red is down
            case 2: return "down";
            //black is left
            case 3: return "left";
            default: return null;
        }
    }
}
