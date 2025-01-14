﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netherlands3D.ObjectInteraction
{
	public class VisualGrid : MonoBehaviour
	{
		public static VisualGrid Instance;

		[SerializeField]
		private Material gridMaterial;

		private float gridPlaneMeshSize = 10.0f;

		[SerializeField]
		private float cellSize = 100.0f;
		public float CellSize { get => cellSize; }

		[SerializeField]
		private float largeCellSize = 1000.0f;
		public float LargeCellSize { get => largeCellSize; }

		void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			Hide();
		}

		public void Show()
		{
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}