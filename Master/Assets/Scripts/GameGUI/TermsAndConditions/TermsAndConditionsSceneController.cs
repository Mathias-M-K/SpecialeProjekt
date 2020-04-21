using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameGUI.TermsAndConditions
{
    public class TermsAndConditionsSceneController : MonoBehaviour
    {
        public GameObject content;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Continue();
            }
        }

        public void Continue()
        {
            LeanTween.moveLocalX(content, 1250, 0.5f).setEase(LeanTweenType.easeInQuad).setOnComplete(() =>
            {
                SceneManager.LoadScene(GlobalValues.WelcomeScene);
            });
        }
    }
}