using Plugins.MonoCache;
using UnityEngine;

namespace Weapons
{
    enum IndexAbility
    {
        Fist = 0,
        Pistol = 1,
        Uzi = 2,
        Grenade = 3
    }
    
    public class WeaponSelector : MonoCache
    {
        [SerializeField] private Weapon[] _weapons;

        public void SelectAbility(int selectIndexAbility)
        {
            foreach (Weapon weapon in _weapons) 
                weapon.gameObject.SetActive(weapon.GetIndexAbility() == selectIndexAbility);
        }

        public Weapon[] GetAllAbility() =>
            _weapons;
    }
}