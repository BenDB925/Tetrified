using UnityEngine;

namespace Tetrified.Scripts.TetrominoLoading
{
    public struct TetrominoData
    {
        public enum TetrominoName
        {
            T,
            L,
            Square,
            Skew,
            Straight
        }

        public Color _color;
        public int[,] _shape;
        public TetrominoName _name;
    }
}