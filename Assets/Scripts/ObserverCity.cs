using System.Collections.Generic;
using System.Linq;
using CitizenLogic.AbstractEntity;
using Plugins.MonoCache;
using UnityEngine;

enum TypeDefense
{
    Attack = 0,
    Escape = 1
}

public class ObserverCity : MonoCache
{
    [SerializeField] private List<Transform> _targetPointsBot;
    [SerializeField] private Citizen[] _citizens;

    private bool _isFirstAttack;
    
    protected override void OnEnabled()
    {
        foreach (Citizen citizen in _citizens) 
            citizen.Attacked += HasAttack;
    }

    protected override void OnDisabled()
    {
        foreach (Citizen citizen in _citizens) 
            citizen.Attacked -= HasAttack;
    }

    private void HasAttack()
    {
        if (!_isFirstAttack)
        {
            _isFirstAttack = true;
            
            foreach (Citizen citizen in _citizens)
                if (citizen.IsAlive())
                    citizen.ProtectiveBehavior();
        }
    }
    
    public Vector3 GetTargetMovement(Vector3 ourPosition)
    {
        List<Transform> targets = _targetPointsBot.Where(target =>
            target.position != ourPosition).ToList();

        if (targets.Count > 0)
            return targets[Random.Range(0, targets.Count)].position;

        return ourPosition;
    }
}