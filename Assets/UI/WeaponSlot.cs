using System;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapons.Player;

namespace UI
{
    public class WeaponSlot : Button
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite defaultSprite;
        [CanBeNull] private WeaponBase _weapon;
        private TextMeshProUGUI _title;
        private TextMeshProUGUI _description;
        public event Action<WeaponBase> OnSelect; 

        public void Init([CanBeNull] WeaponBase weapon, TextMeshProUGUI title, TextMeshProUGUI description, Color color)
        {
            _weapon = weapon;
            _title = title;
            _description = description;
            if (weapon != null)
                image.sprite = weapon.UISprite;
            else
                image.sprite = defaultSprite;
            image.color = color;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (OnSelect != null)
                OnSelect(_weapon);
        }

        public void SetColor(Color c)
        {
            image.DOColor(c, 0.5f);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (_weapon != null)
            {
                _title.text = _weapon.name;
                _description.text = _weapon.Description;
            }
            else
            {
                _title.text = "-- LOCKED --";
                _description.text = "You can unlock new weapons and abilities by defeating enemies across the map";
            }
        }

        [ContextMenu("Get icon image")]
        private void GetIconImage()
        {
            image = transform.Find("Icon").GetComponent<Image>();
        }
    }
}