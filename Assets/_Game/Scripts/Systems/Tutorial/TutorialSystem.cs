using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Balance;
using _Game.Scripts.Core;
using _Game.Scripts.Enums;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Ui;
using _Game.Scripts.Ui.Base;
using _Game.Scripts.View;
using _Game.Scripts.View.Points;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Systems.Tutorial
{
    public class TutorialSystem
    {
        private enum TutorialNextStepCondition
        {
            None,
            Tap,
            PressedButton,
            Action,
            UIOpened,
            UIClosed,
            GoToNextStep
        }
        
        private int _currentTutorialId = 1;
        private int _currentStepIndex = 1;
        
        [Inject] private DiContainer _container;
        [Inject] private ProjectSettings _settings;
        [Inject] private SceneData _sceneData;
        [Inject] private WindowsSystem _windows;
        [Inject] private GameBalanceConfigs _balance;
        [Inject] private LevelSystem _levels;
        [Inject] private TargetArrow _targetArrow;
        [Inject] private GameFlags _gameFlags;
        [Inject] private GameCamera _gameCamera;

        private GameWindow _gameWindow;
        private bool _saveLoaded;

        private readonly List<Func<(TutorialNextStepCondition, object)>> _steps = new();

        private TutorialNextStepCondition _nextStepCondition;
        private object _nextStepConditionParam;
        
        private TutorialWindow _tutorial;

        public bool IsPlaying { get; private set; }
        
        public void Init()
        {
            _tutorial = (TutorialWindow)_windows.GetWindow<TutorialWindow>();

            _levels.OnLoadedLevel += OnLevelLoaded;
            _levels.OnPlayLevel += OnPlayLevel;


            // _playerArrow.Hide();
            _targetArrow.Hide();
            
            _tutorial = (TutorialWindow)_windows.GetWindow<TutorialWindow>();
        }

        private void OnLevelLoaded()
        {
            BaseButton.AnyButtonClickedEvent += OnButtonPressed;
            _windows.WindowOpenedEvent += OnWindowOpened;
            _windows.WindowClosedEvent += OnWindowClosed;
            
            _gameWindow = _windows.GetWindow<GameWindow>() as GameWindow;

            if (_levels.CurrentLevel.Id == 1 || _settings.DevBuild)
            {
                _levels.PlayLevel();
            }
        }

        private void OnPlayLevel()
        {
            if (_settings.SkipTutorial || _gameFlags.Has(GameFlag.TutorialFinished))
            {
                _gameFlags.Set(GameFlag.TutorialFinished);
                IsPlaying = false;
                return;
            }
            
            IsPlaying = true;
            StartTutorial(_container.Resolve<GameSystem>().Level, 1);
        }

        public void StartTutorial(int tutorialId, int stepId)
        {
            BaseButton.AnyButtonClickedEvent += OnButtonPressed;
            _windows.WindowOpenedEvent += OnWindowOpened;
            _windows.WindowClosedEvent += OnWindowClosed;
            
            StartTutorialSteps(tutorialId, stepId);
        }
        
        private void OnButtonPressed()
        {
            if (_nextStepCondition == TutorialNextStepCondition.PressedButton)
            {
                OnStepFinished();
            }
        }
        
        private void OnWindowOpened(BaseWindow window)
        {
            if (_nextStepCondition == TutorialNextStepCondition.UIOpened &&
                _nextStepConditionParam is Type type &&
                type == window.GetType())
            {
                OnStepFinished();
            }
        }

        private void OnWindowClosed(BaseWindow window)
        {
            if (_nextStepCondition == TutorialNextStepCondition.UIClosed &&
                _nextStepConditionParam is Type type &&
                type == window.GetType())
            {
                OnStepFinished();
            }
        }
        
        public void OnAction(GameEvents eventType, object[] parameters)
        {
            if (_nextStepCondition == TutorialNextStepCondition.Action &&
                _nextStepConditionParam is GameEvents conditionEvent &&
                conditionEvent == eventType)
            {
                StartStep(_currentStepIndex + 1);
            }

            // if (eventType == GameEvents.SaveLoaded)
            // {
            //     OnSaveLoaded();
            // }
        }
        
        private void OnStepFinished()
        {
            // _playerArrow.Hide();
            _targetArrow.Hide();
            StartStep(_currentStepIndex + 1);
        }

        private void StartTutorialSteps(int tutorialId, int id)
        {
            _currentTutorialId = tutorialId;
            
            var steps = _balance.DefaultBalance.Tutorial.FindAll(s => s.Region == tutorialId && s.StepId >= id);
            foreach (var step in steps)
            {
                SetTutorialAction(step);
            }
            
            StartStep(0);
        }

        private void SetTutorialAction(TutorialConfig config)
        {
            switch (config.StepAction)
            {
                case TutorialStepAction.None:
                    break;
            }
        }

        private void StartStep(int stepIndex)
        {
            if (_steps.Count <= stepIndex)
            {
                FinishTutorial();
                return;
            }

            _currentStepIndex = stepIndex;
            (_nextStepCondition, _nextStepConditionParam) = _steps[stepIndex].Invoke();
            
            Debug.Log($"tutorial step {stepIndex}");
            
            if (_nextStepCondition != TutorialNextStepCondition.GoToNextStep) return;
            StartStep(_currentStepIndex + 1);
        }

        private void FinishTutorial()
        {
            var deactivates = _sceneData.GetComponentsInChildren<CustomTutorialTarget>(true)
                .Where(t => t.Target == TutorialTarget.BlockTile1).ToList();
            foreach (var deactivate in deactivates)
            {
                deactivate.GetComponent<CollisionListener>().UnblockTile();
            }
            
            IsPlaying = false;
            
            _currentTutorialId = -1;
            _currentStepIndex = -1;
            
            _tutorial.Close();
            _targetArrow.Hide();
            _sceneData.Arrow.StopSequence();

            _steps.Clear();
            
            _gameFlags.Set(GameFlag.TutorialFinished);

            BaseButton.AnyButtonClickedEvent -= OnButtonPressed;
        }
    }
}