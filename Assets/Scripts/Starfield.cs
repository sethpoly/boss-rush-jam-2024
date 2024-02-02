using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Starfield : MonoBehaviour
{
    [SerializeField] private int _maxStars = 100;
    [SerializeField] private float _starSize = 0.1f;
    [SerializeField] private float _starSizeRange = 0.5f;
    [SerializeField] private float _fieldWidth = 10f;
    [SerializeField] private float _fieldHeight = 25f;
    [SerializeField] private bool _colorize = false;
    private float _xOffset;
    private float _yOffset;
    private Transform _bgCamera;
    public ParticleSystem _particles;
    private ParticleSystem.Particle[] _stars;
    public float _speed = 2f;

    void Awake()
    {
        _bgCamera = GameObject.FindWithTag("MainCamera").transform;
        _stars = new ParticleSystem.Particle[_maxStars];

        _xOffset = _fieldWidth * 0.5f;
        _yOffset = _fieldHeight * 0.5f;
        for (int i = 0; i < _maxStars; i++)
        {
            float randsize = Random.Range(_starSizeRange, _starSizeRange + 1f);
            float scaledColor = (true == _colorize) ? randsize - _starSizeRange : 1f;

            _stars[i].position = GetRandomInRectangle(_fieldWidth, _fieldHeight) + transform.position;
            _stars[i].startSize = _starSize * randsize;
            _stars[i].startColor = (Color32)new Color(1f, 1f, 1f, 1f);//scaledColor, scaledColor, 1f);
        }
        _particles.SetParticles(_stars, _stars.Length);
    }

    void Update()
    {
        CalculateMovement();
    }

    private Vector3 GetRandomInRectangle(float width, float height)
    {
        float x = Random.Range(0, width);
        float y = Random.Range(0, height);
        return new Vector3(x - _xOffset, y - _yOffset, 0);
    }

    void CalculateMovement()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y  - _speed * Time.deltaTime, transform.position.z);
        for (int i = 0; i < _maxStars; i++)
        {
            Vector3 pos = _stars[i].position + transform.position;
            if (pos.y < (_bgCamera.position.y - _yOffset))
            {
                pos.y += _fieldHeight;
            }
            _stars[i].position = pos - transform.position;
        }
        _particles.SetParticles(_stars, _stars.Length);
    }
}
