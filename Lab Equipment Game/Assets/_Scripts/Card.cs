using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour {
    [SerializeField] UnitInfo unit;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] RectTransform titleText, icon;
    [SerializeField] CanvasGroup overlay;
    float initialPosition;
    int ID;
    RectTransform rect;
    void Start() {
        costText.text = unit.Cost.ToString();
        rect = GetComponent<RectTransform>();
        initialPosition = rect.anchoredPosition.y;
        ID = GetInstanceID();

        overlay.alpha = 0;
    }

    public void Hover() {
        DOTween.Kill(ID);

        rect.DOAnchorPosY(initialPosition + 30, .4f).SetId(ID).SetEase(Ease.OutCirc);
        overlay.DOFade(1, .3f).SetId(ID);

        titleText.anchoredPosition = Vector3.left * 20;
        titleText.DOAnchorPosX(16, .6f).SetId(ID).SetEase(Ease.InOutSine);

        icon.DOScale(1.2f, .3f).SetId(ID).SetEase(Ease.OutCirc);
    }

    public void Unhover() {
        DOTween.Kill(ID);

        rect.DOAnchorPosY(initialPosition, .4f).SetId(ID).SetEase(Ease.OutCirc);
        overlay.DOFade(0, .3f).SetId(ID);


        icon.DOScale(1f, .3f).SetId(ID).SetEase(Ease.OutCirc);
    }

    public void Click() {
        GameManager.Instance.player.currentlySelectedUnit = unit;
    }
}
