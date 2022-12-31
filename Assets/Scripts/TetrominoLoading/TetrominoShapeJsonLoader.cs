using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Tetrified.Scripts.TetrominoLoading
{
    public class TetrominoShapeJsonLoader : Utility.Singleton<TetrominoShapeJsonLoader>
    {
        private TetrominoShapeJsonLoader() { }

        [SerializeField]
        private string _jsonFilePath;

        private TetrominoData[] _allPotentialShapes;

        [Serializable]
        public class TetrominoShapeList
        {
            public TetrominoShape[] tetrominoShapes;
        }

        [Serializable]
        public class TetrominoShape
        {
            public List<int[]> shape;
            public string color;
        }

        // Loads the Tetris piece shapes from the specified JSON file
        private void LoadShapesFromJSON(string fileName)
        {
            // Read the JSON file and parse the contents
            string json = File.ReadAllText(fileName);
            TetrominoShapeList tetrominoShapeList = Newtonsoft.Json.JsonConvert.DeserializeObject<TetrominoShapeList>(json);

            _allPotentialShapes = new TetrominoData[tetrominoShapeList.tetrominoShapes.Length];

            for (int i = 0; i < tetrominoShapeList.tetrominoShapes.Length; i++)
            {
                TetrominoShape shape = tetrominoShapeList.tetrominoShapes[i];

                //note: this assumes that all columns have the same amount of rows
                int[,] convertedShape = new int[shape.shape.Count, shape.shape[0].Length];

                for (int j = 0; j < shape.shape.Count; j++)
                {
                    for (int k = 0; k < shape.shape[j].Length; k++)
                    {
                        convertedShape[j, k] = shape.shape[j][k];
                    }
                }

                Color color;
                bool didParse = ColorUtility.TryParseHtmlString(shape.color, out color);

                if (didParse == false)
                {
                    Debug.LogError("error reading color value from " + fileName + ", color attempted to read: " + shape.color);
                }

                TetrominoData data = new TetrominoData()
                {
                    _color = color,
                    _shape = convertedShape,
                };

                _allPotentialShapes[i] = data;
            }
        }

        public TetrominoData GetRandomShapeFromJson()
        {
            if (_allPotentialShapes == null)
            {
                LoadShapesFromJSON(_jsonFilePath);
            }

            int randomShapeIndex = UnityEngine.Random.Range(0, _allPotentialShapes.Length);
            return _allPotentialShapes[randomShapeIndex];
        }
    }
}