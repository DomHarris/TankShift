using System;
using System.Collections;
using System.Collections.Generic;
using Entity.Stats;
using UnityEngine;

public class TEST_UpdateStatInsideTrigger : MonoBehaviour
{
    [SerializeField] private StatType statToUpdate;
    [SerializeField] private float newValue;
    [SerializeField] private float range;

    private Dictionary<StatController, float> _initialValues = new Dictionary<StatController, float>();

    private void FixedUpdate()
    {
        var collided = Physics2D.OverlapCircle(transform.position, range)?.GetComponent<StatController>();
        if (collided == null)
        {
            foreach (var kvp in _initialValues)
                kvp.Key.Stats.SetStat(statToUpdate, kvp.Value);
            _initialValues.Clear();   
        }
        else
        {
            if (!_initialValues.ContainsKey(collided))
            {
                _initialValues.Add(collided, collided.Stats.GetStat(statToUpdate));
                collided.Stats.SetStat(statToUpdate, newValue);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
