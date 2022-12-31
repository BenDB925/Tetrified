using Tetromino.Scripts.Gameplay;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class TetrisBoardLogicManager : MonoBehaviour
    {
        [SerializeField]
        TetrisGridData _gridData;

        [SerializeField]
        TetrisGridRenderer _gridRenderer;

        private Tetromino _fallingPiece;

        // the higher the value, the faster the tetronimo falls
        [Range(0, 10)]
        public float _tetrominoFallSpeed;

        /// <summary>
        /// the percentage of the time between grid drops that the lerp takes - 0 = no lerp, 1 = always lerping
        /// </summary>
        [Range(0, 1)]
        private float _lerpSpeed = 0.5f;

        private float _timeSinceFall;

        private bool _paused;

        public delegate void TetrisBoardEvent();
        public static event TetrisBoardEvent GameOverEvent;

        public Tetromino FallingPiece
        {
            get
            {
                if (_fallingPiece == null)
                {
                    _fallingPiece = new Tetromino(_gridData);
                }
                return _fallingPiece;
            }
            set => _fallingPiece = value;
        }

        private void Start()
        {
            FallingPiece.CantPlaceTetromino += OnGameOver;
            FallingPiece.TetronimoLanded += OnTetronimoLanded;
        }

        private void OnDestroy()
        {
            FallingPiece.CantPlaceTetromino -= OnGameOver;
            FallingPiece.TetronimoLanded -= OnTetronimoLanded;
        }

        private void Update()
        {
            if (_paused == false)
            {
                _timeSinceFall += Time.deltaTime;
            }

            const float NormalFallSpeed = 1.0f;
            float adjustedSpeed = Mathf.Max(0, NormalFallSpeed - ((0.1f * _tetrominoFallSpeed) * NormalFallSpeed));
            _gridRenderer.SetLerpSpeed(adjustedSpeed * _lerpSpeed);

            if (_timeSinceFall > adjustedSpeed)
            {
                FallingPiece.MoveDown();
                _timeSinceFall = 0;
            }
        }

        public void TakeInput(InputManager.Action action)
        {
            switch (action)
            {
                case InputManager.Action.MoveTetronimoLeft:
                    _fallingPiece.MoveLeft();
                    break;
                case InputManager.Action.MoveTetronimoRight:
                    _fallingPiece.MoveRight();
                    break;
                case InputManager.Action.RotateClockwise:
                    _fallingPiece.Rotate();
                    break;
                default:
                    break;
            }
        }

        private void OnGameOver()
        {
            _paused = true;
            GameOverEvent?.Invoke();
        }

        private void OnTetronimoLanded()
        {
            _fallingPiece.NewPiece();
            bool didCompleteRow = _gridData.RemoveCompletedRows();

            if (didCompleteRow)
            {
                _gridRenderer.UpdateGameBoardRendering();
            }
        }

        public void SetBoardSelected(bool selected)
        {
            _gridRenderer.SetBoardSelected(selected);
        }

        public void UpdateSize()
        {
            _gridRenderer.UpdateSize();
        }

        public void SetPaused(bool paused)
        {
            _paused = paused;
        }
    }
}
