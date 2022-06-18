using System;
using UnityEngine;

namespace _Game.Scripts.Systems.Save.SaveStructures
{
    [Serializable]
    public struct CameraData
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }
}