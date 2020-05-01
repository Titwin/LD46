using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public class PCInputController : InputController
    {
        public KeyCode[] dxp = { KeyCode.RightArrow, KeyCode.D };
        public KeyCode[] dxn = { KeyCode.LeftArrow, KeyCode.A };
        public KeyCode[] dyp = { KeyCode.UpArrow, KeyCode.W };
        public KeyCode[] dyn = { KeyCode.DownArrow, KeyCode.S };

        public KeyCode[] keyA = { KeyCode.Space, KeyCode.Z };
        public KeyCode[] keyB = { KeyCode.LeftControl, KeyCode.RightControl, KeyCode.X };

        protected void Awake()
        {
            input = this;
        }

        override public Vector2 GetPointer()
        {
            return Camera.main.ViewportToWorldPoint(Input.mousePosition);
        }
        override public Vector2 GetInputAxis()
        {
            return new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        override public bool GetButtonDownA()
        {
            foreach (var key in keyA)
            {
                if (Input.GetKeyDown(key))
                    return true;
            }
            return false;
        }
        override public bool GetButtonDownB()
        {
            foreach (var key in keyB)
            {
                if (Input.GetKeyDown(key))
                    return true;
            }
            return false;
        }
        override public bool GetButtonB()
        {
            foreach (var key in keyB)
            {
                if (Input.GetKey(key))
                    return true;
            }
            return false;
        }
        override public bool AnyButton()
        {
            return Input.anyKey || Input.GetMouseButton(0);
        }
    }

}