﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PolyTerrain : MonoBehaviour
{
	public Material terrainmat;
	//public Material linemat;
	public int polyscale = 10;
	public float heightscale = 2;
	public float sizescale = 1;
	public float perlinscale = .75f;
	public float perlinoffset = 100;
	//public float linewidth = .04f;
	//public float lineheight = .02f;
	public bool deform = false;
	public float deformamount = .1f;
	public int deformseed = 15;
	private Vector3[,] areaMap;
	private Vector3[] terrainVertices;
	//private Vector3[] lineVertices;
	private GameObject polyTerrainMesh;
	//private GameObject polyLineMesh;

	void Start()
	{
		areaMap = new Vector3[polyscale, polyscale];
		for (int z = 0; z < polyscale; z++)
		{
			for (int x = 0; x < polyscale; x++)
			{
				areaMap[z, x] = new Vector3(x * sizescale, 0, z * sizescale);
			}
		}
		polyTerrainMesh = new GameObject("Plane");
		polyTerrainMesh.layer = 9;
		polyTerrainMesh.AddComponent<MeshFilter>();
		polyTerrainMesh.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		polyTerrainMesh.AddComponent<MeshRenderer>();
		polyTerrainMesh.GetComponent<MeshRenderer>().material = terrainmat;
		polyTerrain();
	}

	void Update()
	{

	}

	public void polyTerrain()
	{
		terrainVertices = new Vector3[polyscale * polyscale + (polyscale - 1) * (polyscale - 1)];    //areamap in the first polyscale square, the inbetweens in the second
		int[] terrainTriangles = new int[(polyscale - 1) * (polyscale - 1) * 12];

		for (int z = 0; z < polyscale; z++)
		{
			for (int x = 0; x < polyscale; x++)
			{
				terrainVertices[z * polyscale + x] = areaMap[x, z];
			}
		}

		for (int z = 0; z < polyscale - 1; z++)
		{
			for (int x = 0; x < polyscale - 1; x++)
			{
				float y = (areaMap[x, z].y + areaMap[x + 1, z].y + areaMap[x, z + 1].y + areaMap[x + 1, z + 1].y) / 4;
				terrainVertices[polyscale * polyscale + ((polyscale - 1) * z) + x] = new Vector3(areaMap[x, z].x + .5f * sizescale, y, areaMap[x, z].z + .5f * sizescale);
			}
		}

		int cur = 0;
		for (int z = 0; z < polyscale - 1; z++)
		{
			for (int x = 0; x < polyscale - 1; x++)
			{
				//setting triangles
				terrainTriangles[cur + 2] = z * polyscale + x;
				terrainTriangles[cur + 1] = polyscale * polyscale + ((polyscale - 1) * z) + x;
				terrainTriangles[cur + 0] = z * polyscale + x + 1;
				terrainTriangles[cur + 3] = polyscale * polyscale + ((polyscale - 1) * z) + x;
				terrainTriangles[cur + 4] = z * polyscale + x + 1;
				terrainTriangles[cur + 5] = (z + 1) * polyscale + x + 1;
				terrainTriangles[cur + 8] = z * polyscale + x;
				terrainTriangles[cur + 7] = (z + 1) * polyscale + x;
				terrainTriangles[cur + 6] = polyscale * polyscale + ((polyscale - 1) * z) + x;
				terrainTriangles[cur + 9] = (z + 1) * polyscale + x;
				terrainTriangles[cur + 10] = polyscale * polyscale + ((polyscale - 1) * z) + x;
				terrainTriangles[cur + 11] = (z + 1) * polyscale + x + 1;
				//increment
				cur += 12;
			}
		}

		polyTerrainMesh.GetComponent<MeshFilter>().mesh.vertices = terrainVertices;
		polyTerrainMesh.GetComponent<MeshFilter>().mesh.triangles = terrainTriangles;
		polyTerrainMesh.GetComponent<MeshFilter>().mesh.RecalculateNormals();

		Vector2[] terrainUV = new Vector2[terrainVertices.Length];

		for (int z = 0; z < polyscale; z++)
		{
			for (int x = 0; x < polyscale; x++)
			{
				terrainUV[z * polyscale + x] = new Vector2(areaMap[x, z].z / polyscale / 5, areaMap[x, z].x / polyscale / 5);
			}
		}

		for (int z = 0; z < polyscale - 1; z++)
		{
			for (int x = 0; x < polyscale - 1; x++)
			{
				terrainUV[polyscale * polyscale + ((polyscale - 1) * z) + x] = new Vector2((areaMap[x, z].z + .5f * sizescale) / polyscale / 5, (areaMap[x, z].x + .5f * sizescale) / polyscale / 5);
			}
		}

		polyTerrainMesh.GetComponent<MeshFilter>().mesh.uv = terrainUV;
	}
}
