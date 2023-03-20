using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class TimerCountdown : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private RectTransform _clockHandParent;
    [SerializeField] public CircleSlider _circleSlider;
    [SerializeField] public PostProcessVolume _postProcessVolume;
    [SerializeField] public PlayerScore _player1;
    [SerializeField] public PlayerScore _player2;
    [SerializeField] public AudioSource _tickingNoise;
    public bool _gameStart;
    public float _roundTime = 120f;
    private float val = 1;

    [Header("End of Game UI")]
    [SerializeField] TMP_Text _gameOverUI;
    [SerializeField] TMP_Text _endText;
    [SerializeField] TMP_Text _score1;
    [SerializeField] TMP_Text _score2;
    [SerializeField] TMP_Text _winnerText;

    public Bloom _bloom;
    public ChromaticAberration _chromaticAberration;
    public LensDistortion _lensDistortion;


    void Start()
    {
        if (!_circleSlider)
            _circleSlider = FindObjectOfType<CircleSlider>();

        _postProcessVolume.profile.TryGetSettings(out _chromaticAberration);
        _postProcessVolume.profile.TryGetSettings(out _lensDistortion);
        _postProcessVolume.profile.TryGetSettings(out _bloom);
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
        if ((val <= 0.2f) && (val > 0))
        {
            _chromaticAberration.intensity.value += 0.0001f;
            _tickingNoise.Play();//BUG: wont play??
        }

        if (val <= 0)
        {
            //time is up!
            //_tickingNoise.Stop();
            StartCoroutine(EndGame());
        }
    }

    IEnumerator EndGame()
    {
        _gameStart = false;
        //_tickingNoise.Stop();
        _chromaticAberration.intensity.value += 1;
        _lensDistortion.intensity.value -= 10;


        yield return new WaitForSeconds(1);
        _gameOverUI.enabled = true;

        yield return new WaitForSeconds(1);
        _endText.enabled = true;

        yield return new WaitForSeconds(1);

        int _tempScore = Mathf.CeilToInt(_player1._score);
        _score1.enabled = true;
        _score1.text = _tempScore.ToString();

        yield return new WaitForSeconds(1);
        foreach (Flag _planet in _player1._flagList)
        {
            _planet._myParticleSystem.Play();

            yield return new WaitForSeconds(3);

            //_tempScore += Mathf.CeilToInt(_planet._pointValue);
            _planet.EndCollect(_player1);
            _score1.text = _tempScore.ToString();
            _planet.gameObject.SetActive(false);

            yield return new WaitForSeconds(3);
        }

        int _tempScore2 = Mathf.CeilToInt(_player2._score);
        _score2.enabled = true;
        _score2.text = _tempScore2.ToString();

        yield return new WaitForSeconds(1);
        foreach (Flag _planet in _player2._flagList)
        {
            _planet._myParticleSystem.Play();

            yield return new WaitForSeconds(3);

            //_tempScore += Mathf.CeilToInt(_planet._pointValue);
            _planet.EndCollect(_player1);
            _score2.text = _tempScore2.ToString();
            _planet.gameObject.SetActive(false);

            yield return new WaitForSeconds(3);
        }


        yield return new WaitForSeconds(1);
        if (_tempScore > _tempScore2)
        {
            _winnerText.text = "Sun Wins!";
        } else if (_tempScore < _tempScore2)
        {
            _winnerText.text = "Moon Wins!";
        } else if (_tempScore == _tempScore2)
        {
            _winnerText.text = "DRAW";
        }
        _winnerText.enabled = true;

        yield return new WaitForSeconds(10);

    }
}
