using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TabOpener : MonoBehaviour {

    public RectTransform controllsWindow;
    private bool controllsAreShown;
    private Vector3 controllsStartScale;

    private void Awake() {
        controllsStartScale = controllsWindow.localScale;
    }

    public void OnShowControllsButtonClick() {
        DOTween.Kill(controllsWindow, true);
        controllsAreShown = !controllsAreShown;

        if (controllsAreShown) {
            controllsWindow.localScale = controllsStartScale * 0.8f;
            controllsWindow.gameObject.SetActive(true);
            controllsWindow.DOScale(controllsStartScale, 0.2f).SetEase(Ease.OutBack);
        } else {
            controllsWindow.DOScale(controllsStartScale * 0.8f, 0.2f).SetEase(Ease.InBack).OnComplete(() => {
                controllsWindow.gameObject.SetActive(false);
            });
        }
    }

}
