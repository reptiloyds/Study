using System;
using System.Collections.Generic;
using _Game.Scripts.Components.Base;
using _Game.Scripts.Components.Loader;
using _Game.Scripts.Ui;
using Zenject;

namespace _Game.Scripts.Systems.Base
{
    public class LoadingSystem
    {
        public enum LoaderState
        {
            GameLoading,
            LevelLoading
        }
        
        public Action OnLoadedGame;
        public Action<float> OnUpdateProgress;

        [Inject] private WindowsSystem _windows;
        
        private readonly DiContainer _container;
        private LoaderState _state;
        
        private readonly List<BaseComponent> _steps = new();
        private BaseComponent _currentStep;
        private float _loadingProgress;

        public LoadingSystem(DiContainer container)
        {
            _container = container;
            _container.Bind<ATTrackingStatusComponent>().AsSingle().NonLazy();
            _container.Bind<ConnectionCheckComponent>().AsSingle().NonLazy();
            _container.Bind<PrivacyPolicyComponent>().AsSingle().NonLazy();
            _container.Bind<AnalyticsComponent>().AsSingle().NonLazy();
            _container.Bind<GameBalanceInitializeComponent>().AsSingle().NonLazy();
            _container.Bind<InitGameUI>().AsSingle().NonLazy();
            _container.Bind<InitGameSystems>().AsSingle().NonLazy();
            _container.Bind<SaveLoaderComponent>().AsSingle().NonLazy();
            _container.Bind<LevelLoaderComponent>().AsSingle().NonLazy();
            // _container.Bind<EventBehaviourComponent>().AsSingle().NonLazy();
        }
        public void Loading()
        {
            StartLoader(LoaderState.GameLoading);
        }

        public void StartLoader(LoaderState state)
        {
            _state = state;
            _steps.Clear();
            
            switch (_state)
            {
                case LoaderState.GameLoading:
                    /// <summary>
                    /// Порядок событий при запуске игры. Закоментировать ненужное
                    /// </summary>
                    //_steps.Add(_container.Resolve<ATTrackingStatusComponent>());
                    //_steps.Add(_container.Resolve<ConnectionCheckComponent>());
                    //_steps.Add(_container.Resolve<PrivacyPolicyComponent>());
                    _steps.Add(_container.Resolve<AnalyticsComponent>());
                    _steps.Add(_container.Resolve<GameBalanceInitializeComponent>());
                    _steps.Add(_container.Resolve<InitGameUI>());
                    _steps.Add(_container.Resolve<InitGameSystems>());
                    _steps.Add(_container.Resolve<SaveLoaderComponent>());
                    _steps.Add(_container.Resolve<LevelLoaderComponent>());
                    // _steps.Add(_container.Resolve<EventBehaviourComponent>());
                    break;
                
                case LoaderState.LevelLoading:
                    _steps.Add(_container.Resolve<LevelLoaderComponent>());
                    break;
            }
            
            GoToNextStep();
            
            //_windows.OpenWindow<LoadingWindow>();
        }

        private void GoToNextStep()
        {
            _currentStep = _steps[0];
            _currentStep.OnEnd += OnEndStep;
            _currentStep.Start();
        }

        private void OnEndStep()
        {
            UpdateProgress();
            
            _currentStep.OnEnd -= OnEndStep;
            _steps.Remove(_currentStep);
            _currentStep = null;

            if (_steps.Count == 0)
            {
                _windows.CloseWindow<LoadingWindow>();
                OnLoadedGame?.Invoke();
                return;
            }
            
            GoToNextStep();
        }

        private void UpdateProgress()
        {
            _loadingProgress = (float)(_steps.IndexOf(_currentStep)+1) / _steps.Count;
            OnUpdateProgress?.Invoke(_loadingProgress);
        }
    }
}