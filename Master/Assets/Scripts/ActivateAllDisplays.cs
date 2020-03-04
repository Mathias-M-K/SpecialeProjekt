
using UnityEngine;

public class ActivateAllDisplays : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    private static void ActivateDisplay ()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }
}