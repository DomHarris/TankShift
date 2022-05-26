using System.Collections.Generic;
using Entity.Damage;
using Lean.Pool;
using UnityEngine;

namespace Weapons.Bullets
{
    [RequireComponent(typeof(Health))]
    public class ExplodeOnDie : MonoBehaviour
    {
        private static Dictionary<ParticleSystem, ParticleSystem> _particleSystems = new Dictionary<ParticleSystem, ParticleSystem>();
    
        [SerializeField] private ParticleSystem explosionPrefab;
        [SerializeField] private Transform debris;
    
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
            if (!_particleSystems.ContainsKey(explosionPrefab))
                _particleSystems.Add(explosionPrefab, Instantiate(explosionPrefab));
        }

        private void OnEnable()
        {
            _health.OnHit += OnHit;   
        }

        private void OnHit(float currenthealth, float _, float __, float ___)
        {
            if (currenthealth <= 0)
            {
                var emit = new ParticleSystem.EmitParams
                {
                    position = transform.position
                };
                _particleSystems[explosionPrefab].Emit(emit, 1);
                LeanPool.Spawn(debris, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360))).localScale = Vector3.one;
                LeanPool.Spawn(debris, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360))).localScale = Vector3.one;
                gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            _health.OnHit -= OnHit;
        }
    }
}