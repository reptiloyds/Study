using System;
using System.Collections.Generic;
using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Tools;
using _Game.Scripts.View.Line;
using _Game.Scripts.View.Points;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Systems
{
    public class InputDragSystem : ITickableSystem
    {
         [Inject] private GameCamera _gameCamera;
     
             private ParallelCheck _parallelCheck;
     
             private bool _drag;
             private bool _blockInput;
             private Vector3 _offset;
             private float _zCoord;
             private Color _currentColor;
     
             private List<CollisionListener> _collisionListeners = new ();
     
             public event Action PointDown;
             public event Action PointUp;
     
             public void InputOff()
             {
                 _blockInput = true;
             }
     
             public void InputOn()
             {
                 _blockInput = false;
             }
             
             public void Tick(float deltaTime)
             {
                 if(_blockInput) return;
     
                 CheckInput();
     
                 if (_drag)
                 {
                     FindClosestTile();
                 }
             }
     
             private void CheckInput()
             {			
                 if (Input.GetMouseButtonDown(0))
                 {
                     PointerDown();
                 }
                 if (Input.GetMouseButtonUp(0))
                 {
                     PointerUp();
                 }
             }
             
             private void PointerDown()
             {
                 if(Physics.Raycast(_gameCamera.UnityCam.ScreenPointToRay(Input.mousePosition), out var raycastHit, 100, GameLayers.COLLISION_LISTENER_MASK))
                 {
                     var collisionListener = raycastHit.transform.GetComponent<CollisionListener>();
                     if ((collisionListener != null || !collisionListener.Block) && collisionListener.CanDrawLine)
                     {
                         _collisionListeners.Add(collisionListener);
                         _currentColor = collisionListener.Color;
                         collisionListener.ActivateLineView(LineType.Point);
                         collisionListener.SetLineColor(_currentColor);
     
                         _drag = true;
                     }
                 }
                 
                 PointDown?.Invoke();
             }
     
             private void FindClosestTile()
             {
                 // if(Physics.Raycast(_gameCamera.UnityCam.ScreenPointToRay(Input.mousePosition), out var raycastHit, 100, GameLayers.COLLISION_LISTENER_MASK))
                 // {
                 //     var collisionListener = raycastHit.transform.GetComponent<CollisionListener>();
                 //     if (collisionListener == null || _collisionListeners.Contains(collisionListener) ||
                 //         collisionListener.Block || collisionListener.HavePeople) return;
                 //     
                 //     
                 //     var lastCollision = _collisionListeners.LastValue();
                 //     var alignment = lastCollision.MyGridItem.CalculateNeighborAlignment(collisionListener.MyGridItem);
                 //     if(alignment == NeighborAlignment.None) return;
                 //
                 //     var neighborAlignment = new List<NeighborAlignment> {alignment};
                 //
                 //     var parallel = true;
                 //         
                 //     if (_collisionListeners.Count > 1)
                 //     {
                 //         var preLastCollision = _collisionListeners[^2];
                 //         var preLastAlignment = lastCollision.MyGridItem.CalculateNeighborAlignment(preLastCollision.MyGridItem);
                 //         neighborAlignment.Add(preLastAlignment);
                 //         parallel = _parallelCheck.Check(neighborAlignment);
                 //     }
                 //
                 //     switch (lastCollision.CurrentLineView.LineType)
                 //     {
                 //         case LineType.Point:
                 //             lastCollision.ActivateLineView(LineType.PointLine);
                 //             lastCollision.Rotate(neighborAlignment);
                 //             break;
                 //         case LineType.PointLine:
                 //             if (parallel)
                 //             {
                 //                 lastCollision.ActivateLineView(LineType.SimpleLine);
                 //                 neighborAlignment.RemoveAt(neighborAlignment.Count - 1);
                 //                 lastCollision.Rotate(neighborAlignment);   
                 //             }
                 //             else
                 //             {
                 //                 lastCollision.ActivateLineView(LineType.RotationLine);
                 //                 lastCollision.Rotate(neighborAlignment); 
                 //             }
                 //             break;
                 //         case LineType.RotationLine:
                 //             break;
                 //         case LineType.SimpleLine:
                 //             break;
                 //     }
                 //
                 //     var newNeighborAlignment = new List<NeighborAlignment>
                 //         {collisionListener.MyGridItem.CalculateNeighborAlignment(lastCollision.MyGridItem)};
                 //         
                 //     _collisionListeners.Add(collisionListener);
                 //     collisionListener.ActivateLineView(LineType.PointLine);
                 //     collisionListener.SetLineColor(_currentColor);
                 //     collisionListener.Rotate(newNeighborAlignment);
                 // }
             }
     
             private void PointerUp()
             {
                 // _drag = false;
                 // if (_collisionListeners.Count < 1) return;
                 //
                 // var movePeople = _collisionListeners.FirstValue().Peoples
                 //     .FirstOrDefault(item => item.Alignment is PeopleAlignment.First or PeopleAlignment.Last);
                 //
                 // if (movePeople == null)
                 // {
                 //     Debug.LogError($"First collision hasn`t main people");
                 //     return;
                 // }
                 //
                 // if (_collisionListeners.Count > 1)
                 // {
                 //     _collisionListeners.Reverse();
                 //     var trainView = (movePeople.Owner as TrainView);
                 //     if (trainView != null)
                 //     {
                 //         var successful = trainView.MovePeople(_collisionListeners, movePeople.Alignment);
                 //         if (!successful)
                 //         {
                 //             ClearCollisionListeners();
                 //         }
                 //         else
                 //         {
                 //             _collisionListeners.Clear();      
                 //         }
                 //     }
                 // }
                 // else
                 // {
                 //     ClearCollisionListeners();
                 // }
                 //
                 // PointUp?.Invoke();
             }
     
             private void ClearCollisionListeners()
             {
                 foreach (var collisionListener in _collisionListeners)
                 {
                     collisionListener.DeactivateAllLine();
                 }
                 _collisionListeners.Clear();
             }
    }

    public struct ParallelCheck
    {
        private static readonly List<Alignment> _verticalParallels = new (){Alignment.Forward, Alignment.Backward};
        private static readonly List<Alignment> _horizontalParallels = new (){Alignment.Right, Alignment.Left};

        public bool Check(List<Alignment> neighborAlignments)
        {
            var result = false;
            if (neighborAlignments.Count != 2) return result;

            var firstAlignment = neighborAlignments.FirstValue();
            if (firstAlignment is Alignment.Right or Alignment.Left)
            {
                foreach (var neighbor in neighborAlignments)
                {
                    if (!_horizontalParallels.Contains(neighbor))
                    {
                        result = false;
                        return result;
                    }
                    result = true;
                }
            }
            else
            {
                foreach (var neighbor in neighborAlignments)
                {
                    if (!_verticalParallels.Contains(neighbor))
                    {
                        result = false;
                        return result;
                    }
                    result = true;
                }
            }

            return result;
        }
    }
}