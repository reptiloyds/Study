using _Game.Scripts.Ui.Base;
using UnityEngine;

namespace _Game.Scripts.Ui.SkillsWindow
{
    public class SkillsWindow : BaseWindow
    {
        [SerializeField] private SkillsWindowItem[] _items;

        public override void Init()
        {
            foreach (var item in _items)
            {
                item.Init();
            }

            base.Init();
        }

        public override void Open(params object[] list)
        {
            foreach (var item in _items)
            {
                item.Redraw();
            }
            base.Open(list);
        }
    }
}