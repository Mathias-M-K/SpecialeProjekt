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
        GUIEvents.current.onButtonHit += OnBtnHit;
    }

    private void OnBtnHit(string key)
    {
        if (key.Equals("SpawnPanel"))
        {
            LeanTween.moveLocalY(SpawnPanel, 0, spawnPanelTime).setEase(spawnPanelEase);
        }   
    }
}