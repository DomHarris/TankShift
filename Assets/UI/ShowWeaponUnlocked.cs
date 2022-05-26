using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class ShowWeaponUnlocked : MonoBehaviour
{
    [SerializeField] private float maxY = 200f;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private Volume volume;
    [SerializeField] private Transform title;
    [SerializeField] private float timeShown = 20f;
    private RectTransform _rt;

    private void Awake()
    {
        _rt = (RectTransform)transform;
    }

    [ContextMenu("Show")]
    public void Show()
    {
        canvas.DOFade(1, .75f).SetDelay(.75f);
        DOVirtual.Float(0, 1, .75f, val =>
        {
            _rt.offsetMin = new Vector2(0, Mathf.LerpUnclamped( -maxY, 0, val));
            _rt.offsetMax = new Vector2(0, Mathf.LerpUnclamped(maxY, 0, val));
            volume.weight = val;
        }).SetEase(Ease.OutBack).SetDelay(0.5f);
        title.DOScale(Vector3.one, .75f).SetEase(Ease.OutBack).SetDelay(0.5f);
        title.DOPunchRotation(new Vector3(0, 0, -5), .75f, 0).SetDelay(0.5f);

        DOVirtual.DelayedCall(timeShown, Hide);
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        canvas.DOFade(0, 0.5f);
        DOVirtual.Float(0, 1, 1f, val =>
        {
            _rt.offsetMin = new Vector2(0, Mathf.LerpUnclamped(0, -maxY, val));
            _rt.offsetMax = new Vector2(0, Mathf.LerpUnclamped(0, maxY, val));
            volume.weight = 1 - val;
        }).SetEase(Ease.InBack);
        title.DOScale(Vector3.zero, .75f).SetEase(Ease.InBack);
    }
}
