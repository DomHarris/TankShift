using System;
using DG.Tweening;
using UnityEngine;

namespace Entity.Damage
{
    public class EndOnDie : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fader;
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnHit += OnHit;
        }

        private void OnDisable()
        {
            _health.OnHit -= OnHit;
        }

        private void OnHit(float currentHealth, float previousHealth, float maxHealth, float healthPercentage)
        {
            if (currentHealth > 0) return;
            Time.timeScale = 0;
            fader.DOFade(1, 0.5f).SetUpdate(true);
            fader.blocksRaycasts = true;
        }
        
        public void Quit()
        {
            Time.timeScale = 1;
            Application.Quit();
        }
    }
}