using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controller
{
    public class Pad : EventTrigger
    {
        public enum PadState
        {
            Outside, Entered, Down, Hold, Move, Up
        }
        public RectTransform rect;
        [SerializeField] bool pressed;
        [SerializeField] bool hovered;
        [SerializeField] Vector2 normalizedPosition;

        [SerializeField] PadState state;
        [SerializeField] Vector2 sensitivity = new Vector2(0.1f, 0.9f);
        int fingerId = -1;
        public override void OnBeginDrag(PointerEventData data)
        {
            if (!pressed)
            {
                fingerId = data.pointerId;
                pressed = true;
                Debug.Log("OnBeginDrag called.");
                UpdatePosition(data);
            }
        }
        public override void OnDrag(PointerEventData data)
        {
            if (pressed && fingerId == data.pointerId)
            {
                Debug.Log("OnDrag called.");
                UpdatePosition(data);
            }
        }
        public override void OnEndDrag(PointerEventData data)
        {
            if (pressed)
            {
                Debug.Log("OnEndDrag called.");
                pressed = false;
                ResetPosition();
            }
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

        public Vector2 Position
        {
            get
            {
                return normalizedPosition;
            }
        }
        private void OnValidate()
        {
            rect = this.GetComponent<RectTransform>();
        }

        void UpdatePosition(PointerEventData evt)
        {
            var currentNormalizedPosition = rect.InverseTransformPoint(evt.position);

            currentNormalizedPosition.x /= rect.sizeDelta.x / 2;
            currentNormalizedPosition.y /= rect.sizeDelta.y / 2;
            // this is a hack for multitouch
            float m = currentNormalizedPosition.sqrMagnitude;
            if (m > 1 && m < 1.3f)
            {
                currentNormalizedPosition.Normalize();
            }
            else
            {

                this.normalizedPosition.x = Mathf.Sign(currentNormalizedPosition.x) * Mathf.Lerp(0, 1, Mathf.InverseLerp(sensitivity[0], sensitivity[1], Mathf.Abs(currentNormalizedPosition.x)));
                this.normalizedPosition.y = Mathf.Sign(currentNormalizedPosition.y) * Mathf.Lerp(0, 1, Mathf.InverseLerp(sensitivity[0], sensitivity[1], Mathf.Abs(currentNormalizedPosition.y)));
            }
        }
        void ResetPosition()
        {
            normalizedPosition = Vector2.zero;
        }
    }

}