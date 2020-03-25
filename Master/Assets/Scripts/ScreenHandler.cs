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
                
                AdminCanvas.sortingOrder = 1;
                GameCanvas.sortingOrder = 0;
            }
            else
            {
                AdminCanvas.targetDisplay = 1;
                GameCanvas.targetDisplay = 0;
                
                AdminCanvas.sortingOrder = 0;
                GameCanvas.sortingOrder = 1;
            }

            GameCanvasActive = !GameCanvasActive;
        }
    }
}