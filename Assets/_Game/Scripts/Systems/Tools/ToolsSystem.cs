using UnityEngine;

namespace _Game.Scripts.Systems.Tools
{
    public static class ToolsSystem
    {
        // private static Camera _camera;
        //
        // public static Camera Camera
        // {
        //     get
        //     {
        //         if (_camera == null) _camera = Camera.main;
        //         return _camera;
        //     }
        // }
        
        public static Vector2 GetWorldPositionOfCanvasElement(Camera camera, RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, camera, out var result);
            return result;
        }
    }
}