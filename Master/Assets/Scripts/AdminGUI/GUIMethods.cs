using System;
using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class GUIMethods : MonoBehaviour
    {
        public static void UpdateArrows(GameObject _arrows, PlayerController _player)
        {
            //Settings colors for arrows
            foreach (Transform t in _arrows.transform.GetChild(0))
            {
                Button b = t.GetComponent<Button>();
                ColorBlock cb = b.colors;
                cb.selectedColor = GameHandler.current.GetPlayerMaterial(_player.player).color;
                cb.highlightedColor = GameHandler.current.GetPlayerMaterial(_player.player).color;

                b.colors = cb;
            }


            //Setting correct arrow directions
            int i = 0;
            foreach (Transform t in _arrows.transform.GetChild(0))
            {
                Button b = t.GetComponent<Button>();
                Direction d = _player.GetMoves()[i];

                int rotation = GetDirectionRotation(d);
                if (d != Direction.Blank) b.transform.localRotation = Quaternion.Euler(0, 0, rotation);

                if (d == Direction.Blank)
                {
                    b.interactable = false;
                }
                else
                {
                    b.interactable = true;
                }

                i++;
            }
        }
        
        public static int GetDirectionRotation(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return 90;
                case Direction.Down:
                    return -90;
                case Direction.Right:
                    return 0;
                case Direction.Left:
                    return 180;
                case Direction.Blank:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction");
            }
        }
        
        
    }
}