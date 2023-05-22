using UnityEngine;

namespace CarLogic
{
    public class DoorDriver : Car
    {
        [SerializeField] private float _speed = 180f;
        
        private Vector3 _firstPosition;
        private Animation _animation;

        private void Awake() => 
            _animation = Get<Animation>();

        public void Open() => 
            _animation.Play();
    }
}