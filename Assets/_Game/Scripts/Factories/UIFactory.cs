using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Core;
using _Game.Scripts.Enums;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui;
using _Game.Scripts.UI.WorldSpace;
using _Game.Scripts.Ui.Yacht;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Factories
{
    public class UIFactory
    {
        public enum EmotionType
        {
            Angry,
            Sad,
            Waiting,
            TouchReaction
        }
        
        [Inject] private DiContainer _container;
        [Inject] private SceneData _sceneData;
        [Inject] private WindowsSystem _windows;
        [Inject] private GameResources _resources;
     
        private readonly MessageUI.Pool _messagePool;
        private readonly List<MessageUI> _messages = new();
        
        private readonly ResourceBubbleUI.Pool _resourceBubblesPool;
        private readonly List<ResourceBubbleUI> _resourceBubbles = new();
        
        private readonly Bubble.Pool _bubblePool;
        private readonly List<Bubble> _bubbles = new();
        
        private readonly ProgressBarView.Pool _progressBarsPool;
        private readonly List<ProgressBarView> _progressBars = new();

        private readonly BubbleView.Pool _bubbleViewPool;
        private readonly List<BubbleView> _bubbleViews = new();

        private readonly TargetUIElement.Pool _targetPool;
        private readonly List<TargetUIElement> _targets = new();

        private BadgeNotification[] _badges;

        public UIFactory(Bubble.Pool bubblePool, 
            ProgressBarView.Pool progressBarsPool,
            MessageUI.Pool messagePool,
            ResourceBubbleUI.Pool resourceBubblesPool,
            BubbleView.Pool bubbleViewPool,
            TargetUIElement.Pool targetPool)
        {
            _bubblePool = bubblePool;
            _progressBarsPool = progressBarsPool;
            _messagePool = messagePool;
            _resourceBubblesPool = resourceBubblesPool;
            _bubbleViewPool = bubbleViewPool;
            _targetPool = targetPool;
        }

        public void Init()
        {
            _badges = _sceneData.UI.GetComponentsInChildren<BadgeNotification>();
            foreach (var badge in _badges)
            {
                _container.BindInstance(badge);
            }
            UpdateBadges();
        }

        public Bubble SpawnBubble()
        {
            var bubble = _bubblePool.Spawn();
            _bubbles.Add(bubble);
            bubble.SetParent(_sceneData.WorldSpaceCanvas).Init();
            return bubble;
        }

        private void DrawEmotion()
        {

        }

        public void RemoveBubble(Bubble bubble)
        {
            bubble.Deactivate();
            _bubblePool.Despawn(bubble);
            _bubbles.Remove(bubble);
        }

        public ProgressBarView SpawnProgressBar(Vector3 position, int type, GameProgress progress, bool updatable = true)
        {
            var progressBar = _progressBarsPool.Spawn(_sceneData.WorldSpaceCanvas, position, type, progress, updatable);
            progressBar.DespawnedEvent += RemoveProgressBar;
            _progressBars.Add(progressBar);
            return progressBar;
        }

        public void RemoveProgressBar(ProgressBarView progress)
        {
            progress.DespawnedEvent -= RemoveProgressBar;
            _progressBarsPool.Despawn(progress);
            _progressBars.Remove(progress);
        }

        public void SpawnMessage(string text)
        {
            var i = _messages.Count;
            foreach (var message in _messages)
            {
                message.Move(120 * (i + 1));
                i--;
            }
            
            var newMessage = _messagePool.Spawn(_sceneData.UI, text);
            newMessage.OnFinished += RemoveMessage;
            _messages.Add(newMessage);
        }

        private void RemoveMessage(MessageUI messageUI)
        {
            messageUI.OnFinished -= RemoveMessage;
            _messagePool.Despawn(messageUI);
            _messages.Remove(messageUI);
        }

        public void SpawnResourceBubble(GameParamType gameParam, float count, Vector3 pos = default)
        {
            GamePlayElement type;
            switch (gameParam)
            {
                case GameParamType.Soft:
                    type = GamePlayElement.Soft;
                    break;
                
                case GameParamType.Hard:
                    type = GamePlayElement.Hard;
                    break;
                
                default:
                    return;
            }
            
            if (count == 0) return;
            
            var setup = _sceneData.ResourceBubbleSetups.Find(s => s.Type == type);
            if (setup == null) return;

            var startPos = pos == default ? Input.mousePosition : pos;
            var targetPos = _windows.GetGamePlayElement(type)?.GetComponent<RectTransform>().position?? Vector3.zero;
            if (targetPos == Vector3.zero) return;
            
            count = Mathf.Clamp((int) (count / setup.Conversion), 1, setup.Max);
            
            var isFirst = true;
            var delay = 0f;
            const float animDelay = 0.01f;

            while (count-- > 0)
            {
                var bubble = _resourceBubblesPool.Spawn(_sceneData.UI, startPos, targetPos, setup.Icon, delay);
                bubble.OnFinished += RemoveResourceBubble;
                _resourceBubbles.Add(bubble);
                
                delay += animDelay;

                if (!isFirst) continue;
                isFirst = false;
            }
        }

        private void RemoveResourceBubble(ResourceBubbleUI bubble)
        {
            bubble.OnFinished -= RemoveResourceBubble;
            _resourceBubblesPool.Despawn(bubble);
            _resourceBubbles.Remove(bubble);
        }
        
        public BubbleView SpawnBubbleView(Vector3 position)
        {
            var bubbleView = _bubbleViewPool.Spawn(position);
            _bubbleViews.Add(bubbleView);
            bubbleView.Init();

            return bubbleView;
        }

        private void RemoveOrderUI(BubbleView bubbleView)
        {
            _bubbleViewPool.Despawn(bubbleView);
            _bubbleViews.Remove(bubbleView);
        }

        public TargetUIElement SpawnTargetUI()
        {
            var target = _targetPool.Spawn();
            _targets.Add(target);

            return target;
        }

        public void RemoveTargetUI(TargetUIElement targetUIElement)
        {
            _targetPool.Despawn(targetUIElement);
            _targets.Remove(targetUIElement);
        }

        public List<TargetUIElement> GetTargetUIElements()
        {
            return _targets;
        }

        public void UpdateBadges(BadgeNotificationType type = BadgeNotificationType.None)
        {
            if (type == BadgeNotificationType.None)
            {
                foreach (var badge in _badges)
                {
                    badge.Redraw();
                }
                return;
            }

            var element = _badges.FirstOrDefault(b => b.Type == type);
            element?.Redraw();
        }
    }
}