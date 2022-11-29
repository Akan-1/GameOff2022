using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform _camera;
    [SerializeField, Range(-1f, 1f)] private float _paralax_multiply;
    Vector3 _prev_pos;
    void Start()
    {
        _camera = Camera.main.transform;
    }

    void Update()
    {
        Vector3 delta = _camera.position - _prev_pos;

        _prev_pos = _camera.position;
        transform.position += new Vector3(delta.x * _paralax_multiply, delta.y * _paralax_multiply, 0);


    }
}
