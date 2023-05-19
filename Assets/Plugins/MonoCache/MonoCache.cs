using System;
using Plugins.MonoCache.Interfaces;
using Plugins.MonoCache.System;
using UnityEngine.Device;

namespace Plugins.MonoCache
{
    public abstract class MonoCache : MonoShortСuts, IUpdateCached, IFixedUpdateCached, ILateUpdateCached
    {
        private GlobalUpdate _globalUpdate;
        private bool _isSetup;

        private void OnEnable()
        {
            OnEnabled();

            if (_isSetup == false)
                TrySetup();

            if (_isSetup)
                SubscribeToGlobalUpdate();
        }

        private void OnDisable()
        {
            if (_isSetup)
                UnsubscribeFromGlobalUpdate();

            OnDisabled();
        }

        private void TrySetup()
        {
            if (Application.isPlaying)
            {
                _globalUpdate = Singleton<GlobalUpdate>.Instance;
                _isSetup = true;
            }
            else
                throw new Exception($"You tries to get {nameof(GlobalUpdate)} instance when application is not playing!");
        }

        private void SubscribeToGlobalUpdate()
        {
            _globalUpdate.AddRunSystem(this);
            _globalUpdate.AddFixedRunSystem(this);
            _globalUpdate.AddLateRunSystem(this);
        }

        private void UnsubscribeFromGlobalUpdate()
        {
            _globalUpdate.RemoveRunSystem(this);
            _globalUpdate.RemoveFixedRunSystem(this);
            _globalUpdate.RemoveLateRunSystem(this);
        }

        void IUpdateCached.OnUpdate() => 
            UpdateCached();
        void IFixedUpdateCached.OnFixedUpdate() => 
            FixedUpdateCached();
        void ILateUpdateCached.OnLateUpdate() => 
            LateUpdateCached();

        protected virtual void OnEnabled() {}

        protected virtual void OnDisabled() {}

        protected virtual void UpdateCached() {}

        protected virtual void FixedUpdateCached() {}

        protected virtual void LateUpdateCached() {}
    }
}