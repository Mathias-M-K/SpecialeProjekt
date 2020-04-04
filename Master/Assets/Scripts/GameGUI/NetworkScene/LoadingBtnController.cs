using System;
using System.Collections;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameGUI.NetworkScene
{
    public class LoadingBtnController : MonoBehaviourPunCallbacks
    {
        private TextMeshProUGUI _button;

        private bool _loading = true;

        private void Start()
        {
            _button = GetComponentInChildren<TextMeshProUGUI>();
            StartCoroutine(LoadingAnimation());
        }

        private IEnumerator LoadingAnimation()
        {
            while (_loading)
            {
                _button.text = "Loading";
                
                for (int i = 0; i < 3; i++)
                {
                    _button.text += ".";
                    yield return new WaitForSeconds(0.4f);
                    print("add");
                }
                for (int i = 0; i < 3; i++)
                {
                    _button.text = _button.text.Remove(_button.text.Length - 1);
                    yield return new WaitForSeconds(0.4f);
                    print("remove");
                }
            }
        }

        public void StartLoadingAnimation()
        {
            _loading = true;
            StartCoroutine(LoadingAnimation());
        }

        public void StopLoadingAnimation()
        {
            _loading = false;
        }

        public void SetAsRetryBtn()
        {
            
        }

        public override void OnConnectedToMaster()
        {
            //_loading = false;
        }
    }
}