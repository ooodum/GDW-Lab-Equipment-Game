using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBar : MonoBehaviour {
    RectTransform rect;
    [SerializeField] RectTransform arrowRect;
    private void Awake() {
        rect = GetComponent<RectTransform>();
    }

    public void Hover() {
        rect.DOAnchorPosY(-140, .6f).SetEase(Ease.OutCirc);
        arrowRect.DOLocalRotate(Vector3.forward * 180, .7f).SetEase(Ease.OutBack);
    }

    public void Unhover() {
        rect.DOAnchorPosY(-440, .6f).SetEase(Ease.OutCirc);
        arrowRect.DOLocalRotate(Vector3.forward * 0, .7f).SetEase(Ease.OutBack);
    }

    public void Hide() {
        rect.DOAnchorPosY(-650, .6f).SetEase(Ease.OutCirc);
    }
}
