using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DamagePopupAnimation : MonoBehaviour
{
    RectTransform rectTransform;
    Text text;

	private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponent<Text>();
	}

    public void SetValue(int value)
    {
        if (value < 0)
            text.color = Color.green;
        else
            text.color = Color.yellow;
        text.text = Math.Abs(value).ToString();
    }

    public IEnumerator Animate()
    {
        rectTransform.DOAnchorPos(new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + 30), 1f, false).SetEase(Ease.OutBounce);
        Color c = new Color(text.color.r, text.color.g, text.color.b, 0.1f);
        Tweener tweener = text.DOColor(c, 1f);
        yield return tweener.WaitForCompletion();
        Destroy(gameObject);
    }
}
