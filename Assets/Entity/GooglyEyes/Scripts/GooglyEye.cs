using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GooglyEye : MonoBehaviour
{
    public Transform Eye;
    [Range(0.5f, 10f)]
    public float Speed = 1f;
    [Range(0f, 5f)]
    public float GravityMultiplier = 1f;
    [Range(0.01f, 0.98f)]
    public float Bounciness = 0.4f;

    [SerializeField] private Camera mainCam;
    [SerializeField] private float moveToMouseSpeed;
    [SerializeField] private float minVelocity = 0.2f;
    private Vector3 _origin;
    private Vector3 _velocity;
    private Vector3 _lastPosition;
    private Vector3 _inputPos;

    private Vector3 _eyeVelocitySmoothDamp;

    void Start()
    {
        _origin = Eye.localPosition;
        _lastPosition = transform.position;
    }

    void Update()
    {
        var mouseWorld = mainCam.ScreenToWorldPoint(_inputPos);
        mouseWorld.z = transform.position.z;
        var mouseDir = mouseWorld - transform.position;
        
        const float maxDistance = 0.25f;

        var currentPosition = transform.position;

        var gravity = transform.InverseTransformDirection(Physics2D.gravity);

        _velocity += gravity * GravityMultiplier * Time.deltaTime;
        _velocity += transform.InverseTransformVector((_lastPosition - currentPosition)) * 500f * Time.deltaTime;
        _velocity.z = 0f;


        var position = Eye.localPosition;

        position += _velocity * Speed * Time.deltaTime;

        var direction = new Vector2(position.x, position.y);
        var angle = Mathf.Atan2(direction.y, direction.x);

        if(direction.magnitude > maxDistance)
        {
            var normal = -direction.normalized;

            _velocity = Vector2.Reflect(new Vector2(_velocity.x, _velocity.y), normal) * Bounciness;
            
            position = new Vector3(
                Mathf.Cos(angle) * maxDistance,
                Mathf.Sin(angle) * maxDistance,
                0f
            );
        }
        if (_velocity.sqrMagnitude < minVelocity * minVelocity)
            position = Vector3.SmoothDamp(position, Quaternion.Euler(0,0,-transform.eulerAngles.z) * mouseDir.normalized * maxDistance, ref _eyeVelocitySmoothDamp, moveToMouseSpeed);

        position.z = Eye.localPosition.z;
        
        Eye.localPosition = position;
        
        _lastPosition = transform.position;
    }

    public void GetMousePosition(InputAction.CallbackContext ctx)
    {
        _inputPos = ctx.ReadValue<Vector2>();
        _inputPos.z = 0;
    }
}
