using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event System.Action OnWin;

    [Header ("Movement")]
    public float speed = 5;
    public float turnSpeed = 5;
    public float smoothMoveTime = .1f;

    float angleSmooth;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    Rigidbody rigidbody;
    bool disabled;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        disabled = false;
        Guard.OnSpotted += Disable;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveInput = Vector3.zero;
        if (!disabled)
        {
            moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        float inputMagnitude = moveInput.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float angle = 90 - Mathf.Atan2(moveInput.z, moveInput.x) * Mathf.Rad2Deg;
        angleSmooth = Mathf.LerpAngle(angleSmooth, angle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * speed * smoothInputMagnitude;
    }

    private void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angleSmooth));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider hitCollider)
    {
        print(hitCollider);
        if (hitCollider.tag == "Finish")
        {
            Disable();
            if (OnWin != null)
            {
                OnWin();
            }
        }
    }

    void Disable()
    {
        disabled = true;
        Guard.OnSpotted -= Disable;
    }
}
