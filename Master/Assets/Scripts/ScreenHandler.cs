using System;
using UnityEngine;

public class ScreenHandler : MonoBehaviour
{
    public Canvas AdminCanvas;

    public Canvas GameCanvas;
    public bool GameCanvasActive = true;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (GameCanvasActive)
            {
                AdminCanvas.targetDisplay = 0;
                GameCanvas.targetDisplay = 1;
            }
            else
            {
                AdminCanvas.targetDisplay = 1;
                GameCanvas.targetDisplay = 0;
            }

            GameCanvasActive = !GameCanvasActive;
        }
    }
}