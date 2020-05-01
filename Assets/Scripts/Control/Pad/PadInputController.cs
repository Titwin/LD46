using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public class PadInputController : InputController
    {
        new public GameObject renderer;
        public Pad pad;
        public Button A;
        public Button B;
        protected void Awake()
        {
            input = this;
        }
        override public bool Visible
        {
            get
            {
                return renderer.activeSelf;
            }
            set
            {
                renderer.SetActive(value);
            }
        }
        override public Vector2 GetInputAxis()
        {
            return pad.Position;
        }
        override public bool GetButtonDownA()
        {
            return A.State == Button.ButtonState.Down;
        }
        override public bool GetButtonDownB()
        {
            return B.State == Button.ButtonState.Down;
        }
        override public bool GetButtonB()
        {
            return B.State == Button.ButtonState.Down || B.State == Button.ButtonState.Hold;
        }

        override public bool AnyButton()
        {
            return Input.anyKey || Input.GetMouseButton(0);
        }
    }

}