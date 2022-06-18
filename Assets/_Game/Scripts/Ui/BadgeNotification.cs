using _Game.Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.Ui
{
    public enum BadgeNotificationType
    {
        None
    }
    
    public class BadgeNotification : MonoBehaviour
    {
        [SerializeField] private BadgeNotificationType _type;
        [SerializeField] private Image _complete;
        [SerializeField] private Image _active;
        [SerializeField] private TextMeshProUGUI _text;

        public BadgeNotificationType Type => _type;

        public void Redraw()
        {
            var count = GetCount();
            if (count == 0)
            {
                Hide();
                return;
            }

            this.Activate();

            switch (_type)
            {
                default:
                    _text.text = count.ToString();
                    break;
            }
        }

        private void Hide()
        {
            this.Deactivate();
        }
        
        private int GetCount()
        {
            switch (_type)
            {
                default:
                    return 0;
            }
        }
    }
}