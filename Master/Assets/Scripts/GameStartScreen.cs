using System;
using AdminGUI;
using UnityEngine;

public class GameStartScreen : MonoBehaviour
{
    public GameObject SpawnPanel;
    public float spawnPanelTime;
    public LeanTweenType spawnPanelEase;

    private void Start()
    {
        AdminGUIEvents.current.onButtonHit += OnBtnHit;
    }

    private void OnBtnHit(string key)
    {
        print(key);
        if (key.Equals("SpawnPlayers"))
        {
            LeanTween.moveLocalY(SpawnPanel, 0, spawnPanelTime).setEase(spawnPanelEase);
        }   
    }
}