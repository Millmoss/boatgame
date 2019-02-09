using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
	public float hossPower;
	public float turnPower;
	public float speedLimit;
	public float buoyancy;
	public float seaHeightMod;
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
	private Vector3 positionChange;
	private bool onRamp;
	private float seaHeight;

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
		positionChange = transform.position;
		onRamp = false;
    }
	
    void Update()
    {
		print(velocity);
		seaHeight = sea.transform.position.y + seaHeightMod;
		velocity = Vector3.zero;
		control();
		if (transform.position.y <= seaHeight)
			water();
		gravity();

		//boatBody.velocity += speed * Time.deltaTime;
		//boatBody.velocity = Vector3.ClampMagnitude(boatBody.velocity, speedLimit);
		//velocity += waterPlusGravity;
	}

	void FixedUpdate()
	{
		phys();

		//transform.position += velocity * Time.deltaTime;

		//transform.Rotate(new Vector3(0, -speed.magnitude * 0.05f * Vector3.SignedAngle(transform.forward, motor.transform.up, transform.up) * Time.deltaTime));
		transform.Rotate(new Vector3(0, -boatBody.velocity.magnitude * 0.05f * Vector3.SignedAngle(transform.forward, motor.transform.up, transform.up) * Time.deltaTime));
	}

	private void control()
	{
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.I))
		{
			if (transform.position.y <= seaHeight + 0.4f)
				boatBody.velocity += motor.transform.up * hossPower * Time.deltaTime;
			else
				boatBody.velocity += motor.transform.up * hossPower * Time.deltaTime * Mathf.Clamp01(Mathf.Abs(transform.position.y - seaHeight) / 10);
		}
		else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.K))
		{
			if (transform.position.y <= seaHeight + 0.4f)
				boatBody.velocity -= motor.transform.up * hossPower * Time.deltaTime / 10;
			if (transform.position.y <= seaHeight)
			{
				Vector3 slowVec = Vector3.Lerp(new Vector3(boatBody.velocity.x, 0, boatBody.velocity.z), Vector3.zero, Time.deltaTime / 4);
				boatBody.velocity = new Vector3(slowVec.x, boatBody.velocity.y, slowVec.z);
			}
		}
		else
		{
			if (transform.position.y <= seaHeight)
			{
				Vector3 slowVec = Vector3.Lerp(new Vector3(boatBody.velocity.x, 0, boatBody.velocity.z), Vector3.zero, Time.deltaTime);
				boatBody.velocity = new Vector3(slowVec.x, boatBody.velocity.y, slowVec.z);
			}
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
		/*Vector3 bFor = Vector3.Project(speed, transform.forward);
		Vector3 oldVelocity = speed;
		speed -= speed * Time.deltaTime / 1.2f;
		speed += bFor * Time.deltaTime / (0.7f + oldVelocity.magnitude - bFor.magnitude);

		xRotationMod = -bFor.magnitude / 1.55f;
		zRotationMod = (Vector3.SignedAngle(transform.forward, oldVelocity, transform.up) / 7f) * Mathf.Clamp01(oldVelocity.magnitude / 5);

		waterPlusGravity += Vector3.up * buoyancy * Time.deltaTime * (seaHeight - transform.position.y) * 3f;*/

		if (!(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.K)))
		{
			Vector3 bFor = Vector3.Project(boatBody.velocity, transform.forward);
			Vector3 oldVelocity = boatBody.velocity;
			boatBody.velocity -= boatBody.velocity * Time.deltaTime / 1.2f;
			boatBody.velocity += bFor * Time.deltaTime / (1f + oldVelocity.magnitude - bFor.magnitude);

			xRotationMod = -bFor.magnitude / 1.55f;
			zRotationMod = (Vector3.SignedAngle(transform.forward, oldVelocity, transform.up) / 7f) * Mathf.Clamp01(oldVelocity.magnitude / 5);
		}
		boatBody.velocity += Vector3.up * buoyancy * Time.deltaTime * (seaHeight - transform.position.y) * 3f;
	}

	private void gravity()
	{
		//if (boatBody.velocity.y > 0)
		//	boatBody.velocity -= Vector3.up * Mathf.Lerp(25f, 9.8f, (5 - boatBody.velocity.y) / 5) * Time.deltaTime;
		//else
		boatBody.velocity -= Vector3.up * Mathf.Clamp(Mathf.Abs(9.8f + transform.position.y - seaHeight), 0, 25) * Mathf.Clamp(Mathf.Abs(transform.position.y - seaHeight), 0, 25) * Time.deltaTime;
	}

	private void phys()
	{
		if (transform.position.y <= seaHeight)
		{
			boatBody.angularVelocity = Vector3.Lerp(boatBody.angularVelocity, Vector3.zero, 0.5f);
			transform.rotation = Quaternion.Slerp(boatBody.rotation, Quaternion.Euler(xRotationStart + xRotationMod, boatBody.rotation.eulerAngles.y, zRotationStart + zRotationMod), 0.1f);
		}

		Vector3 clampVec = Vector3.ClampMagnitude(new Vector3(boatBody.velocity.x, 0, boatBody.velocity.z), speedLimit);
		//boatBody.velocity = new Vector3(clampVec.x, boatBody.velocity.y, clampVec.z);
	}

	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag == "Ramp")
		{
			onRamp = true;
		}
	}

	void OnCollisionExit(Collision c)
	{
		if (c.gameObject.tag == "Ramp")
		{
			onRamp = false;
		}
	}
}
