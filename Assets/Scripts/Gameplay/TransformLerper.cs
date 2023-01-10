using UnityEngine;

public class TransformLerper : MonoBehaviour
{
    private Vector3 _targetPosition;

    private float _lerpTime = 1f;

    private Vector3 _startPosition;
    private float _currentLerpTime;

    public void SetDestination(Vector3 pos, float timeForLerp)
    {
        if(float.IsNaN(pos.sqrMagnitude))
        {
            return;
        }

        // Store the starting position
        _startPosition = transform.localPosition;

        _targetPosition = pos;
        _lerpTime = timeForLerp;
        _currentLerpTime = 0;
    }

    void Update()
    {
        // Increment the lerp time
        _currentLerpTime += Time.deltaTime;
        if (_currentLerpTime > _lerpTime)
        {
            _currentLerpTime = _lerpTime;
        }

        // Calculate the lerp value
        float lerp = _currentLerpTime / _lerpTime;

        // Lerp the position
        transform.localPosition = Vector3.Lerp(_startPosition, _targetPosition, lerp);
    }
}
