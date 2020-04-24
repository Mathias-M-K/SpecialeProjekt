using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameGUI.TermsAndConditions
{
    public class TermsAndConditionsSceneController : MonoBehaviour
    {
        public GameObject content;

        
        public void Continue()
        {
            LeanTween.moveLocalX(content, 1250, 0.5f).setEase(LeanTweenType.easeInQuad).setOnComplete(() =>
            {
                SceneManager.LoadScene(GlobalValues.WelcomeScene);
            });
        }
    }
}