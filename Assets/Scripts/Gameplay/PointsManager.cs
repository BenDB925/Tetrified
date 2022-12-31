using Tetrified.Scripts.Utility;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class PointsManager : Singleton<PointsManager>
    {
        private int _points;
        private bool _earningPoints;

        [SerializeField]
        private float _timeToEarnPoint = 1;
        private float _timeSinceEarnedPoint;

        private void Update()
        {
            if (_earningPoints)
            {
                _timeSinceEarnedPoint += Time.deltaTime;

                if (_timeSinceEarnedPoint > _timeToEarnPoint)
                {
                    _timeSinceEarnedPoint = 0;
                    _points++;
                    UIManager.Instance.SetScore(_points);
                }
            }
        }

        public void SetEarningPoints(bool earningPoints)
        {
            _earningPoints = earningPoints;
        }
    }
}