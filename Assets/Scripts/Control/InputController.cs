using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public class InputController : MonoBehaviour
    {
        static public InputController input;

        protected void Awake()
        {
            input = this;
        }
        virtual public bool Visible
        {
            get;
            set;
        }
        virtual public Vector2 GetPointer()
        {
            throw new System.Exception("not implemented");
        }
        virtual public Vector2 GetInputAxis()
        {
            throw new System.Exception("not implemented");
        }
        virtual public bool GetButtonDownA()
        {
            throw new System.Exception("not implemented");
        }
        virtual public bool GetButtonDownB()
        {
            throw new System.Exception("not implemented");
        }
        virtual public bool GetButtonB()
        {
            throw new System.Exception("not implemented");
        }
        virtual public bool AnyButton()
        {
            throw new System.Exception("not implemented");
        }
    }

}