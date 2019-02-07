using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
	public float hossPower;
	public float turnPower;
	public float speedLimit;
	public float buoyancy;
	public GameObject motor;
	public GameObject pin;
	public GameObject spinny;
	public GameObject sea;
	private Vector3 speed;
	private Vector3 waterPlusGravity;	//yes I know this variable is named terribly, represents water and gravity counteracting eachother
	private Vector3 velocity;
	private Rigidbody boatBody;
	private float xRotationStart;
	private float xRotationMod;
	private float zRotationStart;
	private float zRotationMod;

    void Start()
    {
		speed = Vector3.zero;
		waterPlusGravity = Vector3.zero;
		velocity = Vector3.zero;
		boatBody = GetComponent<Rigidbody>();
		xRotationStart = transform.rotation.eulerAngles.x;
		xRotationMod = 0;
		zRotationStart = transform.rotation.eulerAngles.z;
		zRotationMod = 0;
    }
	
    void Update()
    {
		velocity = Vector3.zero;
		control();
		water();
		gravity();

		velocity += speed;
		velocity += waterPlusGravity;
	}

	void FixedUpdate()
	{
		phys();

		speed = Vector3.ClampMagnitude(speed, speedLimit);
		transform.position += velocity * Time.deltaTime;

		transform.Rotate(new Vector3(0, -speed.magnitude * 0.05f * Vector3.SignedAngle(transform.forward, motor.transform.up, transform.up) * Time.deltaTime));
	}

	private void control()
	{
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.I))
		{
			speed += motor.transform.up * hossPower * Time.deltaTime;
		}
		else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.K))
		{
			speed -= motor.transform.up * hossPower * Time.deltaTime / 10;
		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.J))
		{
			if (motor.transform.localRotation.eulerAngles.z < 45 || motor.transform.localRotation.eulerAngles.z > 180)
			{
				motor.transform.RotateAround(pin.transform.position, transform.up, turnPower * Time.deltaTime);
			}
		}
		else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.L))
		{
			if (motor.transform.localRotation.eulerAngles.z < 180 || motor.transform.localRotation.eulerAngles.z > 315)
			{
				motor.transform.RotateAround(pin.transform.position, transform.up, -turnPower * Time.deltaTime);
			}
		}
	}

	private void water()	//this represents the water's effects on the boat, converting semi-forward speed into full forward speed over time
	{
		Vector3 bFor = Vector3.Project(speed, transform.forward);
		Vector3 oldVelocity = speed;
		speed -= speed * Time.deltaTime / 1.2f;
		speed += bFor * Time.deltaTime / (1.4f + oldVelocity.magnitude - bFor.magnitude);

		xRotationMod = -bFor.magnitude / 1.55f;
		zRotationMod = Vector3.SignedAngle(transform.forward, oldVelocity, transform.up) / 3f;

		if (transform.position.y < sea.transform.position.y)
		{
			waterPlusGravity += Vector3.up * buoyancy * Time.deltaTime;
		}
	}

	private void gravity()
	{
		waterPlusGravity -= Vector3.up * 5f * Time.deltaTime;
	}

	private void phys()
	{
		boatBody.velocity = Vector3.zero;
		boatBody.angularVelocity = Vector3.Lerp(boatBody.angularVelocity, Vector3.zero, 0.5f);
		transform.rotation = Quaternion.Slerp(boatBody.rotation, Quaternion.Euler(xRotationStart + xRotationMod, boatBody.rotation.eulerAngles.y, zRotationStart + zRotationMod), 0.1f);
	}
}
