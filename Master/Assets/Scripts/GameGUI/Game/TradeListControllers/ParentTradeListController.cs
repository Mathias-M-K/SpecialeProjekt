using System;
using System.Collections.Generic;
using System.Linq;
using AdminGUI;
using Container;
using CoreGame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;
// ReSharper disable All

namespace AdminGUI
{
    public abstract class ParentTradeListController : MonoBehaviour,ITradeObserver
    {
        [Header("Prefab")] 
        public GameObject listElementPrefab;

        [Header("Title")] 
        public GameObject titleObj;

        [Header("Animation Values")] 
        public float animateInTime;
        public float animateOutTime;
        public float animationRepositionTime;
        public LeanTweenType easeInType;
        public LeanTweenType easeOutType;
        public LeanTweenType repositionType;
        

        [Header("Settings")] 
        [SerializeField] private float elementHeight = 30;
        [SerializeField] private float spacing = 5;
        [SerializeField] private float elementStartPos = 5;
        protected float _bottomPos;
        
        
        //Other
        protected PlayerController PlayerController;
        protected Dictionary<PlayerTrade,GameObject> ElementDictionary = new Dictionary<PlayerTrade, GameObject>();
        protected List<PlayerTrade> Elements = new List<PlayerTrade>(); 

        private void Start()
        {
            LeanTween.moveLocalX(titleObj, elementStartPos, 0);
            GetButtonPos();
            GUIEvents.current.onPlayerChange += OnPlayerChange;
            GUIEvents.current.OnTradeAction += OnTradeBtn;
        }

        private void Update()
        {
            PlayerTrade pt = new PlayerTrade(PlayerTags.Black, PlayerTags.Red, Direction.Right, 
                null, 2, null, Random.Range(0, 10000));
            
            
            /*if (Input.GetKeyDown(KeyCode.A))
            {
                AddElement(pt);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                int i = 0;
                int randomNumber = Random.Range(0, Elements.Count-1);
                PlayerTrade trade = null;
                foreach (KeyValuePair<PlayerTrade,GameObject> pair in ElementDictionary)
                {
                    if (i == randomNumber)
                    {
                        trade = pair.Key;
                    }

                    i++;
                }

                RemoveElement(trade);
            }*/
        }

        protected abstract void OnTradeBtn(Button arg1, TradeActions arg2, Direction arg3);

        private void OnPlayerChange(PlayerTags playerTags)
        {
            if(PlayerController != null)
            {
                PlayerController.RemoveTradeObserver(this);
            }
            PlayerController = GameHandler.Current.GetPlayerController(playerTags);
            PlayerController.AddTradeObserver(this);
        }

        /// <summary>
        /// Getting the lowest position a button can be placed.
        /// </summary>
        protected virtual void GetButtonPos()
        {
            _bottomPos = transform.GetChild(1).localPosition.y;
            Destroy(transform.GetChild(1).gameObject);
        }

        /// <summary>
        /// Adds element to the list
        /// </summary>
        /// <param name="trade"></param>
        protected virtual void AddElement(PlayerTrade trade)
        {
            GameObject listElement = Instantiate(listElementPrefab, transform, false);
            listElement.name = $"IncomingTrade {Random.Range(0,100000)}";

            IncomingTrade inTrade = listElement.GetComponent<IncomingTrade>();
            inTrade.SetInfo(trade,PlayerController);
            
            Elements.Add(trade);
            ElementDictionary.Add(trade,listElement);

            LeanTween.moveLocalX(listElement, elementStartPos, 0);
            LeanTween.moveLocalX(listElement, 0, animateInTime).setEase(easeInType);
            
            UpdateElementPositions();
        }

        /// <summary>
        /// Removes element from list
        /// </summary>
        /// <param name="trade"></param>
        protected void RemoveElement(PlayerTrade trade)
        {
            GameObject element = ElementDictionary[trade];
            Elements.Remove(trade);
            ElementDictionary.Remove(trade);


            LeanTween.moveLocalX(element, elementStartPos, animateOutTime).setEase(easeOutType).setOnComplete(() => Destroy(element.gameObject));
            
            
            UpdateElementPositions();
        }
        
        /// <summary>
        /// Updates elements, prompting them to go to their correct positions
        /// </summary>
        private void UpdateElementPositions()
        {
            LeanTween.moveLocalY(titleObj, _bottomPos + ((Elements.Count) *  elementHeight) + (spacing*(Elements.Count)), animationRepositionTime).setEase(repositionType);
            
            if (Elements.Count == 0)
            {
                LeanTween.moveLocalX(titleObj, elementStartPos, animateOutTime).setEase(easeOutType);
            }
            else
            {
                LeanTween.moveLocalX(titleObj, 0, animateInTime).setEase(easeInType);
            }
            
            int i = 1;
            foreach (PlayerTrade trade in Elements)
            {
                LeanTween.moveLocalY(ElementDictionary[trade], _bottomPos +((Elements.Count-i)*elementHeight)+(spacing*(Elements.Count-i)), animationRepositionTime).setEase(repositionType);
                i++;
            }

            
        }


        public abstract void OnNewTradeActivity(PlayerTrade playerTrade, TradeActions tradeAction);
        
    }
}