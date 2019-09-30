using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float _lerpSpeed;

    [SerializeField]
    private Transform _target;

    private Transform _transform;
    void Start()
    {
        _transform = transform; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var position = Vector3.Lerp(_transform.position, _target.position, _lerpSpeed * Time.deltaTime);
        position.z = -10.0f;

        _transform.position = position;
    }
}
