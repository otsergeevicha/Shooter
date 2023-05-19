using System.Collections.Generic;
using Plugins.MonoCache.Interfaces;
using Plugins.MonoCache.System;
using UnityEngine;

namespace Plugins.MonoCache
{
    [DisallowMultipleComponent]
    public sealed class GlobalUpdate : Singleton<GlobalUpdate>
    {
        public const string OnEnableMethodName = "OnEnable";
        public const string OnDisableMethodName = "OnDisable";

        public const string UpdateMethodName = nameof(Update);
        public const string FixedUpdateMethodName = nameof(FixedUpdate);
        public const string LateUpdateMethodName = nameof(LateUpdate);

        private readonly List<IUpdateCached> _runSystems = new(1024);
        private readonly List<IFixedUpdateCached> _fixedRunSystems = new(512);
        private readonly List<ILateUpdateCached> _lateRunSystems = new(256);

        private readonly ExceptionsChecker _exceptionsChecker = new();

        private void Awake() =>
            _exceptionsChecker.CheckForExceptions();

        public void AddRunSystem(IUpdateCached updateCached) =>
            _runSystems.Add(updateCached);

        public void AddFixedRunSystem(IFixedUpdateCached fixedUpdateCached) =>
            _fixedRunSystems.Add(fixedUpdateCached);

        public void AddLateRunSystem(ILateUpdateCached lateUpdateCached) =>
            _lateRunSystems.Add(lateUpdateCached);

        public void RemoveRunSystem(IUpdateCached updateCached) =>
            _runSystems.Remove(updateCached);

        public void RemoveFixedRunSystem(IFixedUpdateCached fixedUpdateCached) =>
            _fixedRunSystems.Remove(fixedUpdateCached);

        public void RemoveLateRunSystem(ILateUpdateCached lateUpdateCached) =>
            _lateRunSystems.Remove(lateUpdateCached);

        private void Update()
        {
            for (int i = 0; i < _runSystems.Count; i++)
                _runSystems[i].OnUpdate();
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _fixedRunSystems.Count; i++)
                _fixedRunSystems[i].OnFixedUpdate();
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _lateRunSystems.Count; i++)
                _lateRunSystems[i].OnLateUpdate();
        }
    }
}