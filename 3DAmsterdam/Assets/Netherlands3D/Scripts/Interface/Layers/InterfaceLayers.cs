﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Netherlands3D.Interface.Layers
{
	public class InterfaceLayers : MonoBehaviour
	{
		[SerializeField]
		private CustomLayer customObjectLayerPrefab;
		[SerializeField]
		private CustomLayer annotationLayerPrefab;
		[SerializeField]
		private CustomLayer camerasLayerPrefab;

		[SerializeField]
		private RectTransform customLayersContainer;

		[SerializeField]
		private RectTransform annotationsContainer;

		[SerializeField]
		private RectTransform cameraContainer;

		private Animator animator;
		private bool toggledVisible = false;

		[SerializeField]
		private LayerVisuals layerVisualsDialog;
		public LayerVisuals LayerVisuals { get => layerVisualsDialog; }

		void Start()
		{
			animator = GetComponent<Animator>();
			LayerVisuals.gameObject.SetActive(false);
		}

		/// <summary>
		/// Toggle the layer panel
		/// </summary>
		/// <param name="visible"></param>
		public void ToggleVisibility(bool visible)
		{
			toggledVisible = visible;
			animator.SetBool("AnimateIn", toggledVisible);
		}

		/// <summary>
		/// Reset all the layers their colors to their starting values
		/// </summary>
		public void ResetAllLayerMaterialColors()
		{
			InterfaceLayer[] interfaceLayers = GetComponentsInChildren<InterfaceLayer>(true);
			foreach (InterfaceLayer layer in interfaceLayers)
			{
				layer.ResetAllColors();
			}

			layerVisualsDialog.ResetColorsInSelectedMaterials(true);
		}

		/// <summary>
		/// Generate a new interface layer, with a linked object
		/// </summary>
		/// <param name="linkedWorldObject">The GameObject that is linked to this interface layer</param>
		/// <param name="type">The layer/object type</param>

		public CustomLayer AddNewCustomObjectLayer(GameObject linkedWorldObject, LayerType type, bool createdByUser = true)
		{
			CustomLayer newCustomlayer;
			
			if (type == LayerType.ANNOTATION)
			{
				annotationsContainer.gameObject.SetActive(true);
				newCustomlayer = Instantiate(annotationLayerPrefab, annotationsContainer);
				newCustomlayer.Create("Opmerking", linkedWorldObject, type, this);
				newCustomlayer.transform.SetParent(annotationsContainer);

				if (createdByUser)
					linkedWorldObject.GetComponent<Annotation>().PlaceUsingPointer();
			}
			else if (type == LayerType.CAMERA) 
			{
				cameraContainer.gameObject.SetActive(true);
				newCustomlayer = Instantiate(camerasLayerPrefab, customLayersContainer);
				newCustomlayer.Create(linkedWorldObject.name, linkedWorldObject, type, this);
				newCustomlayer.transform.SetParent(cameraContainer);
			}
			else
			{
				newCustomlayer = Instantiate(customObjectLayerPrefab, customLayersContainer);
				newCustomlayer.Create(linkedWorldObject.name, linkedWorldObject, type, this);
			}
			return newCustomlayer;
		}
	}
}
