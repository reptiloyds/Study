using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems;
using _Game.Scripts.Systems.Ads;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Systems.InputSystems;
using _Game.Scripts.Systems.Save;
using _Game.Scripts.Systems.Tools;
using _Game.Scripts.Systems.Tutorial;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui;
using _Game.Scripts.UI.WorldSpace;
using _Game.Scripts.Ui.Yacht;
using _Game.Scripts.View;
using _Game.Scripts.View.Balls;
using _Game.Scripts.View.DrawLine;
using _Game.Scripts.View.Fx;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Core
{
    public class GameInstaller : MonoInstaller, IInitializable, ITickable, ILateTickable
    {
        [SerializeField] private SceneData _sceneData;

        [Inject] private Prefabs _prefabs;
        
        private GameSystem _game;
        
        public override void InstallBindings()
        {
            BindScriptableObjects();
            BindSystems();
            BindFactories();
            BindPools();
            InitExecutionOrder();
            
            _game = Container.Resolve<GameSystem>();
        }

        private void BindScriptableObjects()
        {
            Container.BindInstance(_sceneData).AsSingle().NonLazy();
        }
        
        private void BindSystems()
        {
            Container.Bind<AppEventProvider>().AsSingle().NonLazy();
            Container.Bind<TestTools>().AsSingle().NonLazy();
            Container.Bind<LoadingSystem>().AsSingle().NonLazy();
            Container.Bind<GameSystem>().AsSingle().NonLazy();
            Container.Bind<LevelSystem>().AsSingle().NonLazy();
            Container.Bind<SaveSystem>().AsSingle().NonLazy();
            Container.Bind<EventBehaviourSystem>().AsSingle().NonLazy();
            Container.Bind<AdSystem>().AsSingle().NonLazy();
            Container.Bind<TutorialSystem>().AsSingle().NonLazy();
            Container.Bind<SessionSystem>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<GameInstaller>().FromInstance(this).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameCamera>().FromInstance(_sceneData.Camera).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CheatsSystem>().AsSingle().NonLazy();

            AddSinglePrefab<ConnectionSystem>();
            AddSinglePrefab<TargetArrow>();
            
            Container.BindInterfacesAndSelfTo<WindowsSystem>().AsSingle().NonLazy();
            
            //Игровая логика
            Container.Bind<EndLevelSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InputSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InputBuySystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<DrawLineInputSystem>().AsSingle().NonLazy();
        }
        
        private void BindFactories()
        {
            Container.BindInterfacesAndSelfTo<GameProgressFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameParamFactory>().AsSingle().NonLazy();
            Container.Bind<UIFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PointsFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FxFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AnimalFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BallFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LineFactory>().AsSingle().NonLazy();
        }
        
        private void BindPools()
        {
            //Init game params
            Container.BindMemoryPool<GameParam, GameParam.Pool>().WithInitialSize(100);
            Container.BindMemoryPool<GameProgress, GameProgress.Pool>().WithInitialSize(100);
            
            //Init game objects
            Container.BindMemoryPool<Line, Line.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_prefabs.LoadPrefab<Line>())
                .UnderTransformGroup("Line");
            
            Container.BindMemoryPool<BaseBall, BaseBall.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_prefabs.LoadPrefab<BaseBall>())
                .UnderTransformGroup("BaseBall");

            Container.BindMemoryPool<UniversalFx, UniversalFx.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_prefabs.LoadPrefab<UniversalFx>())
                .UnderTransformGroup("UniversalFx");

            //Init UI elements
            Container.BindMemoryPool<MessageUI, MessageUI.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_prefabs.LoadPrefab<MessageUI>())
                .UnderTransformGroup("Messages");
            
            Container.BindMemoryPool<ResourceBubbleUI, ResourceBubbleUI.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_prefabs.LoadPrefab<ResourceBubbleUI>())
                .UnderTransformGroup("ResourceBubbles");
            
            //Init UI World Space elements
            Container.BindMemoryPool<Bubble, Bubble.Pool>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(_prefabs.LoadPrefab<Bubble>())
                .UnderTransformGroup("Bubbles");
            
            Container.BindMemoryPool<ProgressBarView, ProgressBarView.Pool>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(_prefabs.LoadPrefab<ProgressBarView>())
                .UnderTransformGroup("ProgressBars");

            Container.BindMemoryPool<BubbleView, BubbleView.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_prefabs.LoadPrefab<BubbleView>())
                .UnderTransformGroup("BubbleViews");
            
            Container.BindMemoryPool<TargetUIElement, TargetUIElement.Pool>()
                .WithInitialSize(5)
                .FromComponentInNewPrefab(_prefabs.LoadPrefab<TargetUIElement>())
                .UnderTransformGroup("TargetUIElement");
        }
        
        private void AddSinglePrefab<T>(bool copyPrefab = false) where T : MonoBehaviour
        {
            var prefab = copyPrefab 
                ? _prefabs.CopyPrefab<T>(transform) 
                : GetComponentInChildren<T>(true);
            
            if (prefab == null) return;
            Container.BindInterfacesAndSelfTo<T>().FromInstance(prefab).AsSingle().NonLazy();
        }
        
        private void InitExecutionOrder()
        {
        }
        
        public void Initialize()
        {
            Container.Resolve<LoadingSystem>().Loading();
        }
        
        public void Tick()
        {
            if (_game.GamePaused) return;
            
            DOTween.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            
            var deltaTime = Time.deltaTime;
            foreach (var element in Container.ResolveAll<ITickableSystem>())
            {
                element.Tick(deltaTime);
            }
            
            InvokeSystem.Tick(deltaTime);
        }

        public void LateTick()
        {
            if (_game.GamePaused) return;

            var deltaTime = Time.deltaTime;
            foreach (var element in Container.ResolveAll<ILateTickableSystem>())
            {
                element.LateTick(deltaTime);
            }
        }
    }
}