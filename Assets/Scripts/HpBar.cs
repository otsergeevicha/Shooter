using Plugins.MonoCache;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoCache
{
    [SerializeField] private Image _imageCurrent;

    public void SetValue(float current, float max) =>
        _imageCurrent.fillAmount = current / max;
}