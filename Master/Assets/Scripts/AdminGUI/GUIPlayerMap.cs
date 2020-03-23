using System;
using CoreGame;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
 {
     public class GUIPlayerMap : MonoBehaviour
     {
         public GameObject Row;
         public GameObject Tile;

         private void Update()
         {
             if (Input.GetKeyDown(KeyCode.End))
             {
                 CreateMap();
                 PopulateMap();
             }
         }


         private void CreateMap()
         {
             MapManager mm = MapManager.current;

             int rowName = mm.mapData.ySize;
             for (int i = 0; i < mm.mapData.ySize; i++)
             {
                 GameObject row = Instantiate(Row, transform, true);
                 row.name = rowName.ToString();
                 rowName--;

                 int tileName = mm.mapData.xSize;
                 for (int j = 0; j < mm.mapData.xSize; j++)
                 {
                     GameObject tile = Instantiate(Tile, row.transform, true);
                     tile.name = tileName.ToString();
                     tileName--;
                     tile.transform.SetAsFirstSibling();
                 }
             }
         }

         private void PopulateMap()
         {
             string[,] mapValues = MapManager.current.GenerateMapValues();

             for (int y = 1; y <= MapManager.current.mapData.ySize; y++)
             {
                 for (int x = 1; x <= MapManager.current.mapData.xSize; x++)
                 {
                     Color32 color;
                     switch (mapValues[x,y])
                     {
                         case "w":
                             color = ColorPalette.current.walls;
                             break;
                         case "f":
                             color = ColorPalette.current.floor;
                             break;
                         case "r":
                             color = ColorPalette.current.playerRed;
                             break;
                         case "b":
                             color = ColorPalette.current.playerBlue;
                             break;
                         case "g":
                             color = ColorPalette.current.playerGreen;
                             break;
                         case "y":
                             color = ColorPalette.current.playerYellow;
                             break;
                         default:
                             color = new Color32(211, 84, 0,255);
                             break;
                     }

                     GetTileAt(x, y).GetComponent<Image>().color = color;
                 }
             }

             
         }

         
         
         
         
         private GameObject GetTileAt(Vector2 pos)
         {
             return GetTileAt(new Vector2(pos.x, pos.y));
             
         }
         private GameObject GetTileAt(int x, int y)
         {
             foreach (Transform row in transform)
             {
                 if (row.name.Equals(y.ToString()))
                 {
                     foreach (Transform tile in row)
                     {
                         if (tile.name.Equals(x.ToString()))
                         {
                             return tile.gameObject;
                         }
                     }
                 }
             }
             
             throw new ArgumentException("Position not valid");
         }
     }
 }