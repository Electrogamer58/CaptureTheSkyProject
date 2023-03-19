using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class TimerCountdown : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public CircleSlider _circleSlider;
    [SerializeField] public PostProcessVolume _postProcessVolume;
    [SerializeField] public PlayerScore _player1;
    [SerializeField] public PlayerScore _player2;
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
            _circleSlider.UpdateSlider(val);
        }
        if ((val <= 0.2f) && (val > 0))
        {
            _chromaticAberration.intensity.value += 0.0003f;
        }

        if (val <= 0)
        {
            //time is up!
            StartCoroutine(EndGame());
        }
    }

    IEnumerator EndGame()
    {
        _chromaticAberration.intensity.value += 1;
        _lensDistortion.intensity.value -= 10;


        yield return new WaitForSeconds(1);
        _gameOverUI.enabled = true;

        yield return new WaitForSeconds(1);
        _endText.enabled = true;

        yield return new WaitForSeconds(1);

        float _tempScore = _player1._score;
        _score1.enabled = true;
        _score1.text = _tempScore.ToString();

        yield return new WaitForSeconds(1);
        foreach (Flag _planet in _player1._flagList)
        {
            _planet._myParticleSystem.Play();

            yield return new WaitForSeconds(1);

            _tempScore += Mathf.CeilToInt(_planet._pointValue);
            _score1.text = _tempScore.ToString();
            _planet.gameObject.SetActive(false);

            yield return new WaitForSeconds(1);
        }

        float _tempScore2 = _player2._score;
        _score2.enabled = true;
        _score2.text = _tempScore2.ToString();

        yield return new WaitForSeconds(1);
        foreach (Flag _planet in _player2._flagList)
        {
            _planet._myParticleSystem.Play();

            yield return new WaitForSeconds(1);

            _tempScore += Mathf.CeilToInt(_planet._pointValue);
            _score2.text = _tempScore2.ToString();
            _planet.gameObject.SetActive(false);

            yield return new WaitForSeconds(1);
        }

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
