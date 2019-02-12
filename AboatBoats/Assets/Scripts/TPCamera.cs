using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPCamera : MonoBehaviour
{
	public Camera tpCamera;
	public Transform cameraFocus;
	public Transform target;
	public float xSensitivity = 1f;
	public float ySensitivity = -1;
	public float moveSentivity = 1;
	public float bobbing = 1;
	private Vector3 playerChange;
	private float xLook;
	private float yLook;
	private float xMove;
	private float zMove;
	private float rotateSpeed;
	private float moveSpeed;
	public bool mouseLock = true;
	private bool rotateCamera = false;
	private float yRotationDif = 0;
	private float scrollPosition;
	private float scrollGoal;

	void Start()
	{
		scrollPosition = 21;
		scrollGoal = 15;
		cameraFocus.position = target.position;
		transform.position = cameraFocus.position - (cameraFocus.position - new Vector3(cameraFocus.transform.position.x, cameraFocus.transform.position.y + 1f, cameraFocus.transform.position.z - 1f)).normalized * 10 / scrollPosition;
		cameraFocus.LookAt(Camera.main.transform);
		tpCamera.transform.LookAt(tpCamera.transform.position - cameraFocus.position);
		playerChange = cameraFocus.transform.position;
		rotateSpeed = 20f;
		yRotationDif = transform.rotation.eulerAngles.y - yRotationDif;
		cameraFocus.transform.Rotate(Vector3.up, yRotationDif);
		yRotationDif = transform.rotation.eulerAngles.y;
		Cursor.visible = false;
		rotateCamera = true;
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		//camera move input
		
		if (rotateCamera)
		{
			xLook += xSensitivity * Input.GetAxis("Mouse X");
			yLook += ySensitivity * Input.GetAxis("Mouse Y");
			if (xLook > 180f)
				xLook -= 360f;
			if (xLook < -180f)
				xLook += 360f;
			if (yLook >= 90f)
				yLook = 89f;
			if (yLook < -1f)
				yLook = -1f;
		}
		if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.LeftAlt))
		{
			if (rotateCamera)
			{
				Cursor.visible = true;
				rotateCamera = false;
				Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				Cursor.visible = false;
				rotateCamera = true;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

		transform.eulerAngles = new Vector3(0, xLook, 0);
		tpCamera.transform.localEulerAngles = new Vector3(yLook, 0);
		transform.position = cameraFocus.position - tpCamera.transform.forward * scrollPosition * .6f;
		cameraFocus.transform.eulerAngles = new Vector3(0.0f, xLook, 0.0f);
	}

	void FixedUpdate()
	{
		float x = Mathf.Lerp(cameraFocus.position.x, target.position.x, 10 * Time.deltaTime);
		float z = Mathf.Lerp(cameraFocus.position.z, target.position.z, 10 * Time.deltaTime);
		float bob = Mathf.Clamp01((2 - (bobbing - Mathf.Abs(cameraFocus.position.y - target.position.y))));
		float y = Mathf.Lerp(cameraFocus.position.y, target.position.y, 20 * bob * Time.deltaTime);

		cameraFocus.position = new Vector3(x, y, z);
	}
}
