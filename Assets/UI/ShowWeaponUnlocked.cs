using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Bullets;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Weapons.Player;

namespace UI
{
    public class ShowWeaponUnlocked : MonoBehaviour
    {
        [SerializeField] private float maxY = 200f;
        [SerializeField] private CanvasGroup canvas;
        [SerializeField, CanBeNull] private Volume volume;
        [SerializeField] private Transform title;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI titleShadow;
        [SerializeField] private TextMeshProUGUI subtitle;
        [SerializeField] private Image[] featureImages;
        [SerializeField] private TextMeshProUGUI[] featureText;
        [SerializeField] private WeaponHandler weaponHandler;
        [SerializeField] private float timeShown = 20f;
        [SerializeField] private bool highPriority;
        private RectTransform _rt;
    
        private void Awake()
        {
            _rt = (RectTransform)transform;
        }

        private void OnEnable()
        {
            WeaponOnKillEvent.UnlockWeapon += Show;
        }

        private void OnDisable()
        {
            WeaponOnKillEvent.UnlockWeapon -= Show;
        }

        public void Show(WeaponBase weapon, bool isHighPriority, Transform obj)
        {
            if (isHighPriority != highPriority) return;
            if (!weaponHandler.UnlockWeapon(weapon)) return;
            titleText.text = weapon.name;
            titleShadow.text = weapon.name;
            subtitle.text = weapon.ShortDescription;
            for (var i = 0; i < featureImages.Length; i++)
            {
                featureImages[i].gameObject.SetActive(i < weapon.Features.Count);
                if (i < weapon.Features.Count)
                {
                    featureImages[i].sprite = weapon.Features[i].Type;
                    featureText[i].text = weapon.Features[i].Description;
                }
            }
        

            canvas.DOFade(1, .75f).SetDelay(.75f);
            DOVirtual.Float(0, 1, .75f, val =>
            {
                _rt.offsetMin = new Vector2(0, Mathf.LerpUnclamped(-maxY, 0, val));
                _rt.offsetMax = new Vector2(0, Mathf.LerpUnclamped(maxY, 0, val));
                if (volume != null)
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
                if (volume != null)
                    volume.weight = 1 - val;
            }).SetEase(Ease.InBack);
            title.DOScale(Vector3.zero, .75f).SetEase(Ease.InBack);
        }
    }
}