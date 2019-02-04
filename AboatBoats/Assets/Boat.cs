using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
	public float hossPower;
	public float turnPower;
	public float speedLimit;
	public GameObject motor;
	public GameObject pin;
	public GameObject spinny;
	private Vector3 velocity;

    void Start()
    {
		velocity = Vector3.zero;
    }
	
    void Update()
    {
		control();
		water();
		gravity();
	}

	void FixedUpdate()
	{
		transform.position += velocity * Time.deltaTime;
		velocity = Vector3.ClampMagnitude(velocity, speedLimit);

		transform.Rotate(new Vector3(0, -velocity.magnitude * 0.05f * Vector3.SignedAngle(transform.forward, motor.transform.up, transform.up) * Time.deltaTime));
	}

	private void control()
	{
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.I))
		{
			velocity += motor.transform.up * hossPower * Time.deltaTime;
		}
		else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.K))
		{
			velocity -= motor.transform.up * hossPower * Time.deltaTime / 10;
		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.J))
		{
			if (motor.transform.localRotation.eulerAngles.y < 215)
			{
				motor.transform.RotateAround(pin.transform.position, Vector3.up, turnPower * Time.deltaTime);
			}
		}
		else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.L))
		{
			if (motor.transform.localRotation.eulerAngles.y > 145)
			{
				motor.transform.RotateAround(pin.transform.position, Vector3.up, -turnPower * Time.deltaTime);
			}
		}
	}

	private void water()
	{
		Vector3 mFor = Vector3.Project(velocity, motor.transform.up);
		velocity -= mFor * Time.deltaTime;
		velocity += transform.forward * mFor.magnitude * Time.deltaTime;
	}

	private void gravity()
	{

	}
}
