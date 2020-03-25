using System;
using System.Collections;
using AdminGUI.PlayerMap;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
 {
     public class GUIPlayerMap : MonoBehaviour,ISequenceObserver
     {
         public GameObject Row;
         public GameObject Tile;
         
         [Header("Settings")] 
         public float MapCreationTime;
         public float MapPopulationTime;

         private void Awake()
         {
             AdminGUIEvents.current.onManualOverride += OnManualOverride;
         }

         private void OnManualOverride()
         {
             GameHandler.current.AddSequenceObserver(this);
             CreateMap();
             StartCoroutine(PopulateMap(MapCreationTime+0.5f));
         }

         private void CreateMap()
         {
             MapManager mm = MapManager.current;

             int rowName = mm.mapData.ySize;
             for (int i = 0; i < mm.mapData.ySize; i++)
             {
                 GameObject row = Instantiate(Row, transform, false);
                 row.name = rowName.ToString();
                 rowName--;

                 int tileName = mm.mapData.xSize;
                 for (int j = 0; j < mm.mapData.xSize; j++)
                 {
                     GameObject tile = Instantiate(Tile, row.transform, false);
                     tile.name = tileName.ToString();
                     tileName--;
                     tile.transform.SetAsFirstSibling();
                 }
             }
         }

         private IEnumerator PopulateMap(float delay)
         {
             yield return new WaitForSeconds(delay);
             string[,] mapValues = MapManager.current.GenerateMapValues();

             for (int y = 1; y <= MapManager.current.mapData.ySize; y++)
             {
                 for (int x = 1; x <= MapManager.current.mapData.xSize; x++)
                 {
                     Color32 color;
                     switch (mapValues[x,y])
                     {
                         case "w":
                             color = ColorPalette.current.wallsColor;
                             break;
                         case "f":
                             color = ColorPalette.current.floorColor;
                             break;
                         case "fp":
                             color = ColorPalette.current.finishPointColor;
                             break;
                         default:
                             color = ColorPalette.current.GetPlayerColor(mapValues[x, y].Substring(0,1));
                             break;
                     }

                     GetTileAt(x, y).GetComponent<Image>().color = color;
                     
                     if (mapValues[x, y].Length > 1)
                     {
                         if (mapValues[x,y].Substring(1,1).Equals("t"))
                         {
                             GetTileAt(x, y).gameObject.AddComponent<GUITriggerElement>();
                         }

                         if (mapValues[x,y].Substring(1,1).Equals("g"))
                         {
                             GetTileAt(x, y).gameObject.AddComponent<GUIGateElement>();
                         }
                     }
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
         
         public void OnSequenceChange(SequenceActions sequenceAction, StoredPlayerMove move)
         {
             StartCoroutine(PopulateMap(0.5f));
         }
     }
 }