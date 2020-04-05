using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DefaultNamespace
{
    public class ElevatorButtonController : MonoBehaviour
    {
        public PlatformDirection Direction;
        public ElevatorController elevator;
        
        [Header("ElevatorBtn")]
        public GameObject elevatorBtn;
        public Vector3 buttonInactivePos;
        public LeanTweenType buttonEaseType;
        private Vector3 buttonActivePos;

        private bool elevatorActive;

        private void Start()
        {
            buttonActivePos = transform.localPosition;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (elevatorActive) return;
            
            StartCoroutine(ElevatorActivated());
        }

        private IEnumerator ElevatorActivated()
        {
            elevatorActive = true;
            LeanTween.moveLocal(elevatorBtn, buttonInactivePos, 0.5f).setEase(buttonEaseType);
            
            elevator.DoAction(Direction);
            yield return new WaitForSeconds(elevator.animationTime);

            elevatorActive = false;
            LeanTween.moveLocal(elevatorBtn, buttonActivePos, 0.5f).setEase(buttonEaseType);
            if (Direction == PlatformDirection.Origin)
            {
                Direction = PlatformDirection.Target;
            }
            else
            {
                Direction = PlatformDirection.Origin;
            }
        }
    }
    
    public enum PlatformDirection
    {
        Target,
        Origin
    }
}