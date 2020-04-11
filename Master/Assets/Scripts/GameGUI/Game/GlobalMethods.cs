using System;
using CoreGame;
using GameGUI;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class GlobalMethods : MonoBehaviour
    {
        public static void UpdateArrows(Transform _arrows, PlayerController _player)
        {
            //Settings colors for arrows
            foreach (Transform t in _arrows)
            {
                Button b = t.GetComponent<Button>();
                ColorBlock cb = b.colors;
                cb.selectedColor = ColorPalette.current.GetPlayerColor(_player.playerTag);
                cb.highlightedColor = ColorPalette.current.GetPlayerColor(_player.playerTag);
                cb.selectedColor = ColorPalette.current.GetPlayerColor(_player.playerTag);

                b.colors = cb;
            }

            //Setting correct arrow directions
            int i = 0;
            foreach (Transform t in _arrows)
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

        public static PlayerTags GetTagByNumber(int number)
        {
            int i = 0;

            foreach (PlayerTags playerTag in Enum.GetValues(typeof(PlayerTags)))
            {
                if (i == number)
                {
                    return playerTag;
                }

                i++;
            }

            return PlayerTags.Black;
        }

        public static string GetRole(string name)
        {
            if (name.Equals(GlobalValues.HostTag))
            {
                return "Host";
            }

            if (IsObserver(name))
            {
                return "Observer";
            }

            return "Participant";
        }

        public static string CleanNickname(string nickname)
        {
            if (IsObserver(nickname))
            {
                return nickname.Substring(0, nickname.Length - 4);
            }
            return nickname;
        }

        public static bool IsObserver(string nickname)
        {
            if (nickname.Length > 3)
            {
                if (nickname.Substring(nickname.Length - 4, 4).Equals(GlobalValues.ObserverTag))
                {
                    return true;
                }
            }

            return false;
        }
    }
}