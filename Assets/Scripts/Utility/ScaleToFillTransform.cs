using UnityEngine;

namespace Tetrified.Scripts.Utility
{
    public class ScaleToFillTransform : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _transformToFill;

        public void RescaleToFillTransform()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null) return;

            transform.localScale = new Vector3(1, 1, 1);

            float width = sr.size.x / sr.sprite.bounds.size.x;
            float height = sr.size.y / sr.sprite.bounds.size.y;

            float parentWidth = _transformToFill.sizeDelta.x;
            float parentHeight = _transformToFill.sizeDelta.y;

            Vector2 newScale = new Vector2();
            newScale.x = parentWidth / width;
            newScale.y = parentHeight / height;

            transform.localScale = newScale;
        }
    }
}

