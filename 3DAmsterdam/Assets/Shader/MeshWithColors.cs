﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MeshWithColors : MonoBehaviour
{
	private ParticleSystem particles;
	protected StreamReader reader = null;

	private bool first = true;
	private Vector3 position;
	private Color color;

	void Start()
	{
		particles = GetComponent<ParticleSystem>();

		Mesh mesh = new Mesh();

		gameObject.GetComponent<MeshFilter>().mesh = mesh;

		string[] lines = File.ReadAllLines("C:/Users/Sam/Desktop/tree.txt");


		List<Vector3> verts = new List<Vector3>();
		List<Color> colors = new List<Color>();

		foreach (string line in lines)
		{

			double[] values = Array.ConvertAll(line.Split(' '), double.Parse);

			verts.Add(ConvertCoordinates.CoordConvert.RDtoUnity(
				new ConvertCoordinates.Vector3RD(
					(float)values[0],
					(float)values[1],
					(float)values[2])));

			colors.Add(new Color((float)values[3], (float)values[4], (float)values[5], 1.0f));
		}

		mesh.vertices = verts.ToArray();
		mesh.colors = colors.ToArray();
	}
}