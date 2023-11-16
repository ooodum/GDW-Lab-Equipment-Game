using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour {
    RectTransform rect;
    public bool interactable = true;
    private void Awake() {
        rect = GetComponent<RectTransform>();
    }
    public void Hover() {
        if (!interactable) return;
        rect.DOSizeDelta(Vector2.right * 400 + Vector2.up * 100, .5f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void Unhover() {
        if (!interactable) return;
        rect.DOSizeDelta(Vector2.right * 300 + Vector2.up * 100, .5f).SetEase(Ease.OutBack).SetUpdate(true);
    }
}
