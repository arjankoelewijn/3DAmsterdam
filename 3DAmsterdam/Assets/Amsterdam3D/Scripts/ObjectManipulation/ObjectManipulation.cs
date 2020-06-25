﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManipulation : MonoBehaviour
{
	[SerializeField]
	private Vector3 axis;
	public Vector3 Axis { get => axis; }

	public static bool manipulatingObject = false;

	public float screenSize = 10.0f;

	public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float planeWorldY)
	{
		var ray = Camera.main.ScreenPointToRay(screenPosition);

		var planeNormal = Vector3.up;
		if (axis == Vector3.up)
		{
			//Up handle uses a plane looking at camera, flattened on the Y 
			planeNormal = Camera.main.transform.position - this.transform.position;
			planeNormal.y = 0;
		}

		var worldPlane = new Plane(planeNormal, new Vector3(transform.position.x, planeWorldY, transform.position.z));
		worldPlane.Raycast(ray, out float distance);
		return ray.GetPoint(distance);
	}

	private void Update()
	{
		if(screenSize > 0)
			this.transform.localScale = Vector3.one * Vector3.Distance(Camera.main.transform.position, transform.position) * screenSize;
	}

	public virtual void OnMouseDown()
	{
		manipulatingObject = true;
	}
	public virtual void OnMouseUp()
	{
		manipulatingObject = false;
	}
}