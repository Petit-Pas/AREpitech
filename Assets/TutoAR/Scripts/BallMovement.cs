using System;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float speed;
    public FixedJoystick joystick;
    public GameObject ball;
    public GameObject text;
    public GameObject joystickGO;
    public GameObject restart;

    public void FixedUpdate()
    {
        Vector3 movement = RotateWithView(GetJoystickDirection());

        ball.GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    private Vector3 GetJoystickDirection()
    {
        Vector3 dir = Vector3.zero;

        dir.x = joystick.Horizontal;
        dir.z = joystick.Vertical;
        if (dir.magnitude > 1) dir.Normalize();
        return dir;
    }

    private Vector3 RotateWithView(Vector3 direction)
    {
        Vector3 dir = Camera.main.transform.TransformDirection(direction);
        dir.Set(dir.x, 0f, dir.z);
        return dir.normalized * direction.magnitude;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FIXED_END") || other.CompareTag("END"))
        {
            restart.SetActive(true);
            text.SetActive(true);
            joystickGO.SetActive(false);
        }
    }
}