using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[AddComponentMenu("Camera-Control/Smooth Mouse Look")]
[AddComponentMenu("Camera/Simple Smooth Mouse Look ")]
public class PlayerController : MonoBehaviour
{
	float speed;
	float distanceToHold;

	bool isHolding;

	public Camera cam;
	GameObject currentHeld;

	/*
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationX = 0F;
	float rotationY = 0F;

	private List<float> rotArrayX = new List<float>();
	float rotAverageX = 0F;

	private List<float> rotArrayY = new List<float>();
	float rotAverageY = 0F;

	public float frameCounter = 20;

	Quaternion originalRotation;
	*/

	
	Vector2 _mouseAbsolute;
	Vector2 _smoothMouse;

	public Vector2 clampInDegrees = new Vector2(360, 180);
	public bool lockCursor;
	public Vector2 sensitivity = new Vector2(2, 2);
	public Vector2 smoothing = new Vector2(3, 3);
	public Vector2 targetDirection;
	public Vector2 targetCharacterDirection;

	// Assign this if there's a parent object controlling motion, such as a Character Controller.
	// Yaw rotation will affect this object instead of the camera if set.
	public GameObject characterBody;
	

	// Use this for initialization
	void Start ()
	{
		speed = 15.0f;
		distanceToHold = 5.0f;

		isHolding = false;

		/*
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb)
		{
			rb.freezeRotation = true;
		}

		originalRotation = transform.localRotation;
		*/

		
		targetDirection = transform.localRotation.eulerAngles;

		// Set target direction for the character body to its inital state.
		if (characterBody)
		{
			targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
		}
		

		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Walk();
		MouseLook();
		MouseCheck();
	}

	void Walk ()
	{
		float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
		float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;

		transform.Translate(x, 0, z);
	}

	void MouseLook()
	{
		/*if (axes == RotationAxes.MouseXAndY)
		{
			rotAverageY = 0f;
			rotAverageX = 0f;

			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;

			rotArrayY.Add(rotationY);
			rotArrayX.Add(rotationX);

			if (rotArrayY.Count >= frameCounter)
			{
				rotArrayY.RemoveAt(0);
			}
			if (rotArrayX.Count >= frameCounter)
			{
				rotArrayX.RemoveAt(0);
			}

			for (int j = 0; j < rotArrayY.Count; j++)
			{
				rotAverageY += rotArrayY[j];
			}
			for (int i = 0; i < rotArrayX.Count; i++)
			{
				rotAverageX += rotArrayX[i];
			}

			rotAverageY /= rotArrayY.Count;
			rotAverageX /= rotArrayX.Count;

			rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);
			rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);

			Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
			Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

			transform.localRotation = originalRotation * xQuaternion * yQuaternion;
		}
		else if (axes == RotationAxes.MouseX)
		{
			rotAverageX = 0f;

			rotationX += Input.GetAxis("Mouse X") * sensitivityX;

			rotArrayX.Add(rotationX);

			if (rotArrayX.Count >= frameCounter)
			{
				rotArrayX.RemoveAt(0);
			}
			for (int i = 0; i < rotArrayX.Count; i++)
			{
				rotAverageX += rotArrayX[i];
			}
			rotAverageX /= rotArrayX.Count;

			rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);

			Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);
			transform.localRotation = originalRotation * xQuaternion;
		}
		else
		{
			rotAverageY = 0f;

			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

			rotArrayY.Add(rotationY);

			if (rotArrayY.Count >= frameCounter)
			{
				rotArrayY.RemoveAt(0);
			}
			for (int j = 0; j < rotArrayY.Count; j++)
			{
				rotAverageY += rotArrayY[j];
			}
			rotAverageY /= rotArrayY.Count;

			rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);

			Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
			transform.localRotation = originalRotation * yQuaternion;
		}*/

		
		// Ensure the cursor is always locked when set
		if (lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		// Allow the script to clamp based on a desired target value.
		var targetOrientation = Quaternion.Euler(targetDirection);
		var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

		// Get raw mouse input for a cleaner reading on more sensitive mice.
		var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		// Scale input against the sensitivity setting and multiply that against the smoothing value.
		mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

		// Interpolate mouse movement over time to apply smoothing delta.
		_smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
		_smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

		// Find the absolute mouse movement value from point zero.
		_mouseAbsolute += _smoothMouse;

		// Clamp and apply the local x value first, so as not to be affected by world transforms.
		if (clampInDegrees.x < 360)
			_mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

		// Then clamp and apply the global y value.
		if (clampInDegrees.y < 360)
			_mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

		transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

		// If there's a character body that acts as a parent to the camera
		if (characterBody)
		{
			var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
			characterBody.transform.localRotation = yRotation * targetCharacterOrientation;
		}
		else
		{
			var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
			transform.localRotation *= yRotation;
		}
	}

	void MouseCheck()
	{
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, distanceToHold))
		{
			switch (hit.transform.tag)
			{
				case "Bouncer":
					if (Input.GetButtonDown("Fire1"))
					{
						if (!isHolding)
						{
							currentHeld = hit.collider.gameObject;
							currentHeld.transform.position = this.transform.position + this.transform.forward;
							//currentHeld.transform.parent = this.transform;

							isHolding = true;
						}
						else
						{
							//currentHeld.transform.parent = null;
							currentHeld = null;

							isHolding = false;
						}
					}

					if (isHolding)
					{
						currentHeld.transform.position = (this.transform.position + new Vector3(0, 0.5f, 0)) + (this.transform.forward * 2);
						//currentHeld.transform.rotation = new Quaternion(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
						/*if (this.transform.rotation.y < 0)
						{
							currentHeld.transform.rotation = new Quaternion(currentHeld.transform.rotation.x, this.transform.rotation.y, currentHeld.transform.rotation.z, currentHeld.transform.rotation.w);
						}
						else
						{*/
							//currentHeld.transform.rotation = new Quaternion(currentHeld.transform.rotation.x, -this.transform.rotation.y, currentHeld.transform.rotation.z, currentHeld.transform.rotation.w);
						//}
						currentHeld.transform.rotation = this.transform.rotation;

						print("held item rotation :" + currentHeld.transform.rotation);
						print("player rotation :" + this.transform.rotation);
					}
					break;

				default:

					break;
			}
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		angle = angle % 360;
		if ((angle >= -360F) && (angle <= 360F))
		{
			if (angle < -360F)
			{
				angle += 360F;
			}
			if (angle > 360F)
			{
				angle -= 360F;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}
}
