using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controller
{
    public class Button : EventTrigger
    {
        public enum ButtonState
        {
            Unpressed, Down, Hold, Up
        }
        [SerializeField] bool pressedPrev;
        [SerializeField] bool pressed;
        [SerializeField] bool hovered;
        [SerializeField] ButtonState state;

        public ButtonState State
        {
            get { return state; }
        }
        public override void OnPointerEnter(PointerEventData data)
        {
            hovered = true;
            Debug.Log("OnPointerExit called.");
        }

        public override void OnPointerExit(PointerEventData data)
        {
            Debug.Log("OnPointerExit called.");
            hovered = false;
        }

        public override void OnPointerDown(PointerEventData data)
        {
            state = ButtonState.Down;
            pressedPrev = pressed;
            pressed = true;
            Debug.Log("OnPointerDown called.");
        }
        public override void OnPointerClick(PointerEventData data)
        {
            state = ButtonState.Up;
            Debug.Log("OnPointerClick called.");
            pressedPrev = pressed;
            pressed = false;
        }

        private void FixedUpdate()
        {
            if(state == ButtonState.Down) {
                state = ButtonState.Hold;
            }
            else if (state == ButtonState.Up)
            {
                state = ButtonState.Unpressed;
            }
        }


        /*
        public override void OnInitializePotentialDrag(PointerEventData data)
        {
            Debug.Log("OnInitializePotentialDrag called.");
        }

        public override void OnPointerClick(PointerEventData data)
        {
            Debug.Log("OnPointerClick called.");
        }

        public override void OnPointerDown(PointerEventData data)
        {
            Debug.Log("OnPointerDown called.");
        }

        public override void OnPointerEnter(PointerEventData data)
        {
            Debug.Log("OnPointerEnter called.");
        }

        public override void OnPointerUp(PointerEventData data)
        {
            Debug.Log("OnPointerUp called.");
        }*/
        /*
         * 
         * 
        public override void OnDeselect(BaseEventData data)
        {
            Debug.Log("OnDeselect called.");
        }

        public override void OnDrop(PointerEventData data)
        {
            Debug.Log("OnDrop called.");
        }


        public override void OnMove(AxisEventData data)
        {
            Debug.Log("OnMove called.");
        }

         public override void OnCancel(BaseEventData data)
                {
                    Debug.Log("OnCancel called.");
                }

                public override void OnScroll(PointerEventData data)
                {
                    Debug.Log("OnScroll called.");
                }

                public override void OnSelect(BaseEventData data)
                {
                    Debug.Log("OnSelect called.");
                }

                public override void OnSubmit(BaseEventData data)
                {
                    Debug.Log("OnSubmit called.");
                }

                public override void OnUpdateSelected(BaseEventData data)
                {
                    Debug.Log("OnUpdateSelected called.");
                }
               */
        // Start is called before the first frame update
    }

}