using CarLogic;
using Plugins.MonoCache;
using UnityEngine;

public class ActorUI : MonoCache
{
    [SerializeField] private HpBar _hpBar;
    [SerializeField] private CarHealth _carHealth;

    protected override void OnEnabled() => 
        _carHealth.HealthChanged += UpdateHpBar;

    protected override void OnDisabled() => 
        _carHealth.HealthChanged -= UpdateHpBar;

    private void UpdateHpBar() => 
        _hpBar.SetValue(_carHealth.CurrentHealth, _carHealth.MaxHealth);
}