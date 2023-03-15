using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

	public float acceleration = 10;
	public float maxSpeed = 8;

	private Rigidbody unityRigidbody;

	public void Awake()
	{
		unityRigidbody = GetComponent<Rigidbody>();
	}


	void FixedUpdate()
	{

		unityRigidbody.AddForce(new Vector3(Input.GetAxis("Horizontal") * acceleration, 0, 0));
		unityRigidbody.AddForce(new Vector3(0, Input.GetAxis("Vertical") * acceleration, 0));
		unityRigidbody.velocity = Vector3.ClampMagnitude(unityRigidbody.velocity, maxSpeed);
	}
}
