using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ToolTips : MonoBehaviour
{
    [SerializeField] private float xLimit = 960;
    [SerializeField] private float yLimit = 540;

    [SerializeField] private float xOffset = 200;
    [SerializeField] private float yOffset = 100;
    public virtual void AdjustPosition() {
        Vector2 mousePosition = Input.mousePosition;

        float newXOffset = 0;
        float newYOffset = 0;

        if (mousePosition.x > xLimit) newXOffset = - xOffset;
        else newXOffset = xOffset;

        if (mousePosition.y > yLimit) newYOffset = - yOffset;
        else newYOffset = yOffset + 50;
        transform.position = new Vector2(mousePosition.x + newXOffset, mousePosition.y + newYOffset);
    }
    protected void AdjustFontSize(TextMeshProUGUI _text, int _defaultFontSize) {
        if (_text.text.Length > 12) {
            _text.fontSize = _defaultFontSize * .7f;
        }
        else {
            _text.fontSize = _defaultFontSize;
        }
    }
}
