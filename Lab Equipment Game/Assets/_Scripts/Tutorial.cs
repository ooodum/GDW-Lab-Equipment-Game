using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour {
    GameManager manager;
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI text;

    bool wasd, rclick, hover, place, start;
    State currentState;
    enum State {
        Move,
        Place,
        Start
    }
    void Start() {
        manager = GameManager.Instance;

        manager.MoveEvent += (input) => { if (currentState == State.Move && input.magnitude == 0) {wasd = true; print(input); } };
        manager.LockMouseEvent += () => { if (currentState == State.Move) rclick = true; };
        manager.ClickEvent += () => { if (currentState == State.Place) place = true; };
        manager.StartGameEvent += () => { if (currentState == State.Start) start = true;};
        currentState = State.Move;

        TutorialMove();
    }

    async void TutorialMove() {
        SetTutorial(Vector3.right * 31, "Use <i>WASD</i> to move, and hold <i>RIGHT CLICK</i> to turn the camera", new Vector2(353, 252));
        while (!(wasd && rclick)) await Awaitable.NextFrameAsync();
        currentState = State.Place;
        TutorialSelectTroop();
    }

    async void TutorialSelectTroop() {
        SetTutorial(new Vector2(32, -72), "Select a unit from the bar below", new Vector2(711, 80));
        while (!place) await Awaitable.NextFrameAsync();
        TutorialPlace();

    }

    async void TutorialPlace() {
        SetTutorial(new Vector2(166, -5), "<i>LEFT CLICK</i> to place a unit on the field", new Vector2(430, 232));
        while (manager.FriendlyTroops.Count == 0) await Awaitable.NextFrameAsync();
        currentState = State.Start;
        TutorialStart();
    }

    async void TutorialStart() {
        SetTutorial(new Vector2(1470, 173), "Press <i>SPACE</i> to watch the battle begin!", new Vector2(430, 182));
        while (!start) await Awaitable.NextFrameAsync();
        rect.GetComponent<CanvasGroup>().DOFade(0, 1);
    }

    void SetTutorial(Vector2 position, string text, Vector2 delta) {
        this.text.text = text;
        rect.DOAnchorPos(position, 1).SetEase(Ease.OutCirc);
        rect.DOSizeDelta(delta, .7f).SetEase(Ease.OutBack);
    }
}
