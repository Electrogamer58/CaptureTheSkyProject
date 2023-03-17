using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimerCountdown : MonoBehaviour
{
    // Start is called before the first frame update
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
            _circleSlider.UpdateSlider(val);
        }
        if (val <= 0.3f)
        {
            _chromaticAberration.intensity.value += 0.0004f;
        }
    }
}
