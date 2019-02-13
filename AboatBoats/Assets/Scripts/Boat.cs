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
	private bool onGround;
	private float seaHeight;
	public AudioSource sa;
	public AudioSource sb;
	public AudioSource sc;
	public AudioSource ding;
	private bool underwater;

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
		onGround = false;
		underwater = false;
    }
	
    void Update()
    {
		print(transform.position);
		seaHeight = sea.transform.position.y + seaHeightMod;
		velocity = Vector3.zero;
		control();
		water();
		gravity();

		if (boatBody.velocity.y < -5 && !underwater && transform.position.y <= seaHeight)
		{
			underwater = true;
			float r = Random.value * 3;
			if (r < 1)
			{
				sa.Play();
			}
			else if (r < 2)
			{
				sb.Play();
			}
			else
			{
				sc.Play();
			}
		}
		else if (transform.position.y > seaHeight)
		{
			underwater = false;
		}


		//boatBody.velocity += speed * Time.deltaTime;
		//boatBody.velocity = Vector3.ClampMagnitude(boatBody.velocity, speedLimit);
		//velocity += waterPlusGravity;
	}

	void FixedUpdate()
	{
		phys();

		//transform.position += velocity * Time.deltaTime;

		//transform.Rotate(new Vector3(0, -speed.magnitude * 0.05f * Vector3.SignedAngle(transform.forward, motor.transform.up, transform.up) * Time.deltaTime));

		//TOTO : For polish, make it so it rotates correctly when backing up
		if (Vector3.Dot(boatBody.velocity.normalized, -transform.forward) < 0)
			transform.Rotate(new Vector3(0, -boatBody.velocity.magnitude * 0.05f * Vector3.SignedAngle(transform.forward, motor.transform.up, transform.up) * Time.deltaTime));
		else
			transform.Rotate(new Vector3(0, boatBody.velocity.magnitude * 0.05f * Vector3.SignedAngle(transform.forward, motor.transform.up, transform.up) * Time.deltaTime));
	}

	private void control()
	{
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.I))
		{
			boatBody.velocity += motor.transform.up * hossPower * Time.deltaTime;
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
		if (transform.position.y <= seaHeight)
		{
			Vector3 bFor = Vector3.Project(boatBody.velocity, transform.forward);
			Vector3 oldVelocity = boatBody.velocity;
			boatBody.velocity -= boatBody.velocity * Time.deltaTime / 0.7f;
			//boatBody.velocity += bFor * Time.deltaTime / (0.7f + oldVelocity.magnitude - bFor.magnitude);
			boatBody.velocity += oldVelocity.magnitude * transform.forward * Time.deltaTime * 1.25f;// / (0.7f + oldVelocity.magnitude - bFor.magnitude);

			xRotationMod = -bFor.magnitude / 1f;
			zRotationMod = (Vector3.SignedAngle(transform.forward, oldVelocity, transform.up) / 7f) * Mathf.Clamp01(oldVelocity.magnitude / 5);
			boatBody.velocity += Vector3.up * buoyancy * Time.deltaTime * (seaHeight - transform.position.y);
		}
		else
		{
			Vector3 bFor = Vector3.Project(boatBody.velocity, transform.forward);
			Vector3 oldVelocity = boatBody.velocity;

			xRotationMod = -bFor.magnitude / 1f;
			zRotationMod = (Vector3.SignedAngle(transform.forward, oldVelocity, transform.up) / 7f) * Mathf.Clamp01(oldVelocity.magnitude / 5);
		}
	}

	private void gravity()
	{
		//if (boatBody.velocity.y > 0)
		//	boatBody.velocity -= Vector3.up * Mathf.Lerp(25f, 9.8f, (5 - boatBody.velocity.y) / 5) * Time.deltaTime;
		//else
		//boatBody.velocity -= Vector3.up * Mathf.Clamp(Mathf.Abs(9.8f + transform.position.y - seaHeight), 0, 25) * Mathf.Clamp(Mathf.Abs(transform.position.y - seaHeight), 0, 25) * Time.deltaTime;
		//if (transform.position.y < seaHeight + 3f)
			//boatBody.velocity -= Vector3.up * 9.8f * Time.deltaTime;
		//else
			//boatBody.velocity -= Vector3.up * (9.8f + (transform.position.y - seaHeight + 3f) * 3f) * Time.deltaTime;
	}

	private void phys()
	{
		if (transform.position.y <= seaHeight)
		{
			boatBody.angularVelocity = Vector3.Lerp(boatBody.angularVelocity, Vector3.zero, 0.5f);
			if (!(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.K)))
			{
				transform.rotation = Quaternion.Slerp(boatBody.rotation, Quaternion.Euler(xRotationStart + xRotationMod, boatBody.rotation.eulerAngles.y, zRotationStart + zRotationMod), Time.deltaTime * 3);
			}
			else
			{
				transform.rotation = Quaternion.Slerp(boatBody.rotation, Quaternion.Euler(xRotationStart, boatBody.rotation.eulerAngles.y, zRotationStart), Time.deltaTime * 3);
			}
		}
		else
		{
			transform.rotation = Quaternion.Slerp(boatBody.rotation, Quaternion.Euler(xRotationStart, boatBody.rotation.eulerAngles.y, zRotationStart), Time.deltaTime / 3);
		}

		Vector3 clampVec = Vector3.ClampMagnitude(new Vector3(boatBody.velocity.x, 0, boatBody.velocity.z), speedLimit);
		//boatBody.velocity = new Vector3(clampVec.x, boatBody.velocity.y, clampVec.z);
	}

	public void playDing()
	{
		ding.Play();
	}

	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag == "Ramp")
		{
			onRamp = true;
		}
		if (c.gameObject.tag == "Ground")
		{
			onGround = true;
		}
	}

	void OnCollisionExit(Collision c)
	{
		if (c.gameObject.tag == "Ramp")
		{
			onRamp = false;
		}
		if (c.gameObject.tag == "Ground")
		{
			onGround = true;
		}
	}
}
