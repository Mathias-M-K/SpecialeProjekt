using System;
using AdminGUI;
using UnityEngine;
using UnityEngine.UI;

public class GameStartScreen : MonoBehaviour
{
    [Header("Start Screen")] 
    public GameObject StartScreenBackground;
    public float StartScreenBackgroundAnimationTime;
    public LeanTweenType StartScreenBackgroundEaseType;
    
    [Header("Spawn Panel")]
    public GameObject SpawnPanel;
    public float spawnPanelTime;
    public LeanTweenType spawnPanelEase;
    

    private void Start()
    {
        GUIEvents.current.onButtonHit += OnBtnHit;
        GUIEvents.current.onGameStart += OnGameStart;
    }

    private void OnBtnHit(Button button)
    {
        string key = button.name;
        if (key.Equals("SpawnPlayers"))
        {
            LeanTween.moveLocalY(SpawnPanel, 0, spawnPanelTime).setEase(spawnPanelEase);
        }
    }

    private void OnGameStart()
    {
        LeanTween.moveLocalY(StartScreenBackground, -1440, StartScreenBackgroundAnimationTime)
            .setEase(StartScreenBackgroundEaseType);
    }
}