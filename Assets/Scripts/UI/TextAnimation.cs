using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TextAnimation : MonoBehaviour
{
    private Transform _transform;
    private Vector3 _startScale;
    private float _additionalScaleCurrentTime;

    private IEnumerator _growScale;
    private IEnumerator _decreaseScale;

    [SerializeField] private AnimationCurve _additionalScale;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _startScale = _transform.localScale;
    }

    public void OnMouseEnter()
    {
        StartGrowScale();
        Debug.Log("asd");
    }

    public void OnMouseExit()
    {
        StartDecreaseScale();
    }

    private void StartGrowScale()
    {
        StopAllCoroutines();
        _growScale = GrowScale();
        StartCoroutine(GrowScale());
    }

    private void StartDecreaseScale()
    {
        StopAllCoroutines();
        _decreaseScale = DecreaseScale();
        StartCoroutine(_decreaseScale);
    }

    private IEnumerator DecreaseScale()
    {
        float totalTime = 0;

        while (_additionalScaleCurrentTime > totalTime)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            float newScaleXYZ = _additionalScale.Evaluate(_additionalScaleCurrentTime);
            _transform.localScale = new Vector3(newScaleXYZ + _startScale.x, newScaleXYZ + _startScale.y, newScaleXYZ + _startScale.z);
            _additionalScaleCurrentTime -= Time.deltaTime;
        }
    }

    private IEnumerator GrowScale()
    {
        float totalTime = _additionalScale.keys[_additionalScale.length - 1].time;

        while (_additionalScaleCurrentTime < totalTime)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            float newScaleXYZ = _additionalScale.Evaluate(_additionalScaleCurrentTime);
            _transform.localScale = new Vector3(newScaleXYZ + _startScale.x, newScaleXYZ + _startScale.y, newScaleXYZ + _startScale.z);
            _additionalScaleCurrentTime += Time.deltaTime;
        }
    }
}
