using Tetrified.Scripts.Gameplay;
using Tetrified.Scripts.Utility;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class TetrisBoardLogicManager : MonoBehaviour
    {
        private Tetromino _fallingPiece;

        // the higher the value, the faster the tetronimo falls
        [SerializeField]
        [Range(0, 10)]
        private float _tetrominoFallSpeed;

        private float _timeSinceFall;

        public Tetromino FallingPiece
        {
            get
            {
                if (_fallingPiece == null)
                {
                    _fallingPiece = new Tetromino();
                }
                return _fallingPiece;
            }
            set => _fallingPiece = value;
        }

        private void Update()
        {
            _timeSinceFall += Time.deltaTime;

            const float NormalFallSpeed = 1.0f;
            float adjustedSpeed = Mathf.Max(0, NormalFallSpeed - ((0.1f * _tetrominoFallSpeed) * NormalFallSpeed));

            if (_timeSinceFall > adjustedSpeed)
            {
                FallingPiece.MoveDown();
                _timeSinceFall = 0;
            }
        }
    }
}
