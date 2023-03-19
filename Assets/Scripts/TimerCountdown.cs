using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimerCountdown : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private RectTransform _clockHandParent;
    [SerializeField] public CircleSlider _circleSlider;
    [SerializeField] public PostProcessVolume _postProcessVolume;
    public bool _gameStart;
    public float _roundTime = 120f;
    private float val = 1;

    public ChromaticAberration _chromaticAberration;


    void Start()
    {
        if (!_circleSlider)
            _circleSlider = FindObjectOfType<CircleSlider>();

        _postProcessVolume.profile.TryGetSettings(out _chromaticAberration);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_gameStart)
        {
            float timeElapsed = Time.deltaTime;
            float timeToSubtract = 1.0f / _roundTime;
            val -= timeElapsed * timeToSubtract;
            val = Mathf.Clamp(val, 0.0f, 1.0f);
            //_circleSlider.UpdateSlider(val);

            //convert float to degrees
            float deg = val * 360;
            deg = Mathf.FloorToInt(deg % 360);

            //only update every second (should be deg % 6 if 60 second timer)
            if(deg % 3 == 0)
            {
                //_circleSlider.UpdateSlider(deg / 360);

                Quaternion rot = new Quaternion();
                rot.eulerAngles = new Vector3(_clockHandParent.rotation.eulerAngles.x, _clockHandParent.rotation.eulerAngles.y, deg);
                _clockHandParent.SetPositionAndRotation(_clockHandParent.transform.position, rot);
            }

        }
        if (val <= 0.3f)
        {
            _chromaticAberration.intensity.value += 0.0004f;
        }
    }
}
