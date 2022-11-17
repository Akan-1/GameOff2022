using System.Collections;
using UnityEngine;
public class FadeChanger : MonoBehaviour
{

    [SerializeField] private SpriteRenderer _blackForeground;
    [SerializeField] private float _changeAlphaPerTick = 0.01f;
    private int _currentSceneIndex;
    private IEnumerator _fadeOutBlack;
    private IEnumerator _fadeInBlack;

    private void Awake()
    {
        _blackForeground.color = Color.black;
        StartFadeOut();
    }
    public void StartFadeIn()
    {
        StopFadeIn();
        StopFadeOut();

        _fadeInBlack = FadeInBlack();
        StartCoroutine(_fadeInBlack);
    }

    public void StartFadeInAndChangeScene(int index)
    {
        StopFadeIn();
        StopFadeOut();

        _fadeInBlack = FadeInBlack(index);
        StartCoroutine(_fadeInBlack);
    }

    public void StartFadeOut()
    {
        StopFadeIn();
        StopFadeOut();

        _fadeOutBlack = FadeOutBlack();
        StartCoroutine(_fadeOutBlack);
    }

    private void StopFadeIn()
    {
        if (_fadeInBlack != null)
        {
            StopCoroutine(_fadeInBlack);
        }
    }

    private void StopFadeOut()
    {
        if (_fadeOutBlack != null)
        {
            StopCoroutine(_fadeOutBlack);
        }
    }

    private IEnumerator FadeInBlack(int index = -1)
    {
        while (_blackForeground.color.a < 1)
        {
            _blackForeground.color = new Color(0, 0, 0, _blackForeground.color.a + _changeAlphaPerTick);
            yield return new WaitForSeconds(.01f);
        }

        if (index != -1)
        {
            SceneLoader.LoadByIndex(index);
        }


    }

    private IEnumerator FadeOutBlack()
    {
        while (_blackForeground.color.a > 0)
        {
            _blackForeground.color = new Color(0, 0, 0, _blackForeground.color.a - _changeAlphaPerTick);
            yield return new WaitForSeconds(.01f);
        }
    }
}