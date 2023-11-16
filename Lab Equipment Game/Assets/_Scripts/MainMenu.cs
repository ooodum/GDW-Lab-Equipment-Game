using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    [SerializeField] RectTransform logo, playButton, bg, loadingBox;
    [SerializeField] CanvasGroup loadingText;
    // Start is called before the first frame update
    void Start() {
        logo.localRotation = Quaternion.Euler(Vector3.forward * -4);
        logo.DORotate(Vector3.forward * 8, 2).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        bg.anchoredPosition = Vector3.left * 2171;

        loadingBox.sizeDelta = new Vector3(0, 100);
        loadingText.alpha = 0;
    }

    // Update is called once per frame
    void Update() {
        transform.localEulerAngles += Vector3.up * Time.deltaTime * 10;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void StartGame() {
        playButton.GetComponent<UIButton>().interactable = false;
        playButton.DOSizeDelta(new Vector2(2000, 100), .8f).SetEase(Ease.OutCirc);
        bg.DOAnchorPosX(0, .6f).SetEase(Ease.OutCirc);

        loadingBox.DOSizeDelta(new Vector2(350, 120), .7f).SetEase(Ease.OutBack).SetDelay(1).OnComplete(() => {
            loadingText.DOFade(1, .8f).SetDelay(.2f);
            WaitForGameStart();
        });
    }

    async void WaitForGameStart() {
        await Awaitable.WaitForSecondsAsync(2);
        SceneManager.LoadScene("Level1");
    }
}
