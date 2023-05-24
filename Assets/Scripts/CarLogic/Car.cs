using Plugins.MonoCache;

namespace CarLogic
{
    public class Car : MonoCache
    {
        private int _damage = 100;

        public int Damage =>
            _damage;
    }
}