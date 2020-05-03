using System;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace GameGUI.TermsAndConditions
{
    public class TermsAndConditionsSceneController : MonoBehaviour
    {
        public GameObject content;
        public CustomDropdown dropdown;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
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

        public void SetFullscreen()
        {
            Resolution[] resolutions = Screen.resolutions;
            Screen.SetResolution(resolutions[resolutions.Length-1].width,resolutions[resolutions.Length-1].height,FullScreenMode.FullScreenWindow,60);
        }
    }
}