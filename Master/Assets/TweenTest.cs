using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LeanTween.moveLocalY(gameObject, 0, 0.5f).setEase(LeanTweenType.easeOutSine);
        }
        
    }
}
