using System;
using TMPro;
using UnityEngine;

namespace MoreLanguages
{
    internal class CustomLangSupport : MonoBehaviour
    {
        private void OnEnable()
        {
            // If the 'text' field is null, set the text properties for the TMP_Text component.
            if (this.text == null)
            {
                SetText(base.GetComponent<TMP_Text>());
            }
        }

        private void SetText(TMP_Text textComp)
        {
            // Set the 'text' field to the provided TMP_Text component.
            this.text = textComp;
            
            // Enable word wrapping and auto-sizing for the text component.
            this.text.enableWordWrapping = true;
            this.text.fontSizeMax = this.text.fontSize;
            this.text.enableAutoSizing = true;
            
            // If the left and right margins are less than 10 units, adjust them to be 10 units.
            var vector4 = this.text.margin;
            if (vector4.x < 10f && vector4.z < 10f)
            {
                this.text.margin = new Vector4(10f, this.text.margin.y, 10f, this.text.margin.w);
            }
        }

        private TMP_Text text;
    }
}