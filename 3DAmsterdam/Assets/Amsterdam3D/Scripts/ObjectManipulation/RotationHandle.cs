﻿using Amsterdam3D.FreeShape;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHandle : ObjectManipulation
{
	private GameObject rotationOrigin;
	private FreeShape parentFreeShape;
	private Vector3 dragStartLocation;

	private void Start()
	{
		parentFreeShape = GetComponentInParent<FreeShape>();
	}

	public override void OnMouseDown()
	{
		base.OnMouseDown();
		rotationOrigin = new GameObject();
		rotationOrigin.name = "CustomShapeRotationOrigin";
		rotationOrigin.transform.position = parentFreeShape.FloorOrigin.position;

		dragStartLocation = GetWorldPositionOnPlane(Input.mousePosition, this.transform.position.y);
		parentFreeShape.transform.SetParent(rotationOrigin.transform);

	}
	public override void OnMouseUp()
	{
		base.OnMouseUp();
		this.transform.parent.SetParent(null);
		Destroy(rotationOrigin);
	}


	private void OnMouseDrag()
	{
		RotateParentOverOrigin();
	}

	private void RotateParentOverOrigin()
	{
		var dragTargetPosition = GetWorldPositionOnPlane(Input.mousePosition, this.transform.position.y);

		var startNormal = (dragStartLocation - rotationOrigin.transform.position).normalized;
		var targetNormal = (dragTargetPosition - rotationOrigin.transform.position).normalized;

		var angle = Vector3.SignedAngle(targetNormal, startNormal,Vector3.up);
		rotationOrigin.transform.eulerAngles = new Vector3(0.0f, -angle, 0.0f);
	}
}