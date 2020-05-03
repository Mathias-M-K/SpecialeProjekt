using System;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class PlayerListController : MonoBehaviour
    {
        public Image background;
        public GameObject listRect;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ShowList();
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                HideList();
            }
        }

        private void ShowList()
        {
            LeanTween.moveLocalY(listRect, 0, 0.3f).setEase(LeanTweenType.easeOutQuad);
            LeanTween.alpha(background.rectTransform, 0.55f, 0.3f).setRecursive(false);
        }

        private void HideList()
        {
            LeanTween.moveLocalY(listRect, 1124, 0.3f).setEase(LeanTweenType.easeInQuad);
            LeanTween.alpha(background.rectTransform, 0, 0.3f).setRecursive(false);
        }

    }
}