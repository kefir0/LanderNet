﻿using System;
using System.Linq;
using LanderNet.Game.GameObject;

namespace LanderNet.Game.Components
{
    public class CollisionComponent : ITimedComponent
    {
        #region public static methods

        public static bool AreCollided(IGameObject x, IGameObject y, double threshold = 0)
        {
            var xPos = x.GetComponent<PositionComponent>();
            var yPos = y.GetComponent<PositionComponent>();
            var xSize = x.GetComponent<SizeComponent>();
            var ySize = y.GetComponent<SizeComponent>();

            if (xPos == null || yPos == null || xSize == null || ySize == null) return false;

            // This order of checks is statistically proven to be more effective
            // That is, first check is most likely to return false (coefficients are 3.3, 3, 2.3, 1.9, respectively)
            if (yPos.Y + ySize.Height - threshold < xPos.Y) return false;
            if (xPos.X + xSize.Width - threshold < yPos.X) return false;
            if (yPos.X + ySize.Width - threshold < xPos.X) return false;
            if (xPos.Y + xSize.Height - threshold < yPos.Y) return false;

            return true;
        }


        #endregion

        #region public constructors

        public CollisionComponent(IGameObject parentObject, LanderGame parentGame, double threshold, Action<IGameObject> onCollision, params Type[] collidesWith)
        {
            if (parentObject == null)
                throw new ArgumentNullException(nameof(parentObject));

            if (parentGame == null)
                throw new ArgumentNullException(nameof(parentGame));

            if (onCollision == null)
                throw new ArgumentNullException(nameof(onCollision));

            if (!collidesWith.Any())
                throw new ArgumentException();

            _parentObject = parentObject;
            _parentGame = parentGame;
            _threshold = threshold;
            _onCollision = onCollision;
            _collidesWith = collidesWith;
        }

        #endregion

        #region public methods

        public void UpdateOnTime(double seconds)
        {
            foreach (var gameObj in _parentGame.GameObjects.AsParallel().Where(DetectCollision).ToArray())
            {
                _onCollision(gameObj);
            }
        }

        #endregion

        #region private methods

        private bool DetectCollision(IGameObject obj)
        {
            if (obj == _parentObject) return false;

            var type = obj.GetType();

            // Micro-optimization: use for loop instead of Contains
            var collidesWithGivenObject = false;
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _collidesWith.Length; i++)
            {
                if (ReferenceEquals(_collidesWith[i], type))
                {
                    collidesWithGivenObject = true;
                    break;
                }
            }

            return collidesWithGivenObject && AreCollided(_parentObject, obj, _threshold);
        }

        #endregion

        #region private fields

        private readonly Type[] _collidesWith;
        private readonly IGameObject _parentObject;
        private readonly LanderGame _parentGame;
        private readonly double _threshold;
        private readonly Action<IGameObject> _onCollision;

        #endregion
    }
}