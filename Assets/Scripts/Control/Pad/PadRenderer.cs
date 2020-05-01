using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public class PadRenderer : MonoBehaviour
    {
        public Pad pad;
        [SerializeField] RectTransform cursor;
        // Start is called before the first frame update
        private void LateUpdate()
        {
            var position = pad.Position;
            position.x *= pad.rect.sizeDelta.x * 0.5f;
            position.y *= pad.rect.sizeDelta.y * 0.5f;
            cursor.anchoredPosition = position;
        }
    }

}