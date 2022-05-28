using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapons.Player;

namespace UI
{
    public class ShowWeaponUI : MonoBehaviour
    {
        [SerializeField] private WeaponSlot[] weaponSlots;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private WeaponHandler weapons;
        [SerializeField] private CanvasGroup canvas;
        [SerializeField] private Color selectedColor = Color.black;
        [SerializeField] private Color unselectedColor = Color.gray;
        
        private bool _showing = false;
        
        [ContextMenu("Fill weapon slots")]
        private void FillWeapons()
        {
            weaponSlots = GetComponentsInChildren<WeaponSlot>();
        }
        
        public void Show(InputAction.CallbackContext ctx)
        {
            if (!ctx.ReadValueAsButton()) return;
            
            _showing = !_showing;
            canvas.blocksRaycasts = _showing;
            if (_showing)
                canvas.DOFade(1, 0.5f);
            else
                canvas.DOFade(0, 0.5f);
            
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (i < weapons.UnlockedWeapons.Count)
                {
                    weaponSlots[i].Init(weapons.UnlockedWeapons[i], title, description, weapons.IsCurrentWeapon(weapons.UnlockedWeapons[i]) ? selectedColor : unselectedColor);
                    weaponSlots[i].OnSelect += OnWeaponSelect;
                }
                else
                    weaponSlots[i].Init(null, title, description, unselectedColor);
            }
        }

        private void OnWeaponSelect(WeaponBase newWeapon)
        {
            if (weapons.IsCurrentWeapon(newWeapon))
                return;
            
            weapons.SetWeapon(newWeapon);

            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (i < weapons.UnlockedWeapons.Count)
                    weaponSlots[i].SetColor(weapons.IsCurrentWeapon(weapons.UnlockedWeapons[i])
                        ? selectedColor
                        : unselectedColor);
                else
                    weaponSlots[i].SetColor(unselectedColor);
            }
        }
    }
}