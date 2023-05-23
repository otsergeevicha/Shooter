using UnityEngine;

namespace CarLogic
{
    public class DoorDriver : Car
    {
        private Vector3 _firstPosition;
        private Animation _animation;

        private void Awake() => 
            _animation = Get<Animation>();

        public void Open() => 
            _animation.Play();

        public void Close() => 
            _animation.Play();
    }
}