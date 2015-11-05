using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetSquare : MonoBehaviour {
	
	public float [][] heightMap;
	public int subDegree;

	public int iPos;
	private int xPos;
	public int jPos;
	private int yPos;
	public int kPos;
	private int zPos;
	public int indexSize;

	private bool initialized = false;

	public Planet planet = null;
	public PlanetSquare parent = null;
	public PlanetSquare[] children = null;
	public int childDepth = 0;

	public Vector3 center = Vector3.zero;
	public float subLimit = 0f;
	public float unSubLimit = float.MaxValue;

	private int removeMeshRequests = 0;

	public void FindPlanet () {
		this.planet = this.GetComponentInParent <Planet> ();
	}

	private float delay = 0f;
	private float delta = 0.5f;

	public void Update () {
		if (!initialized) {
			return;
		}
		if (this.childDepth == 2) {
			return;
		}

		delay += Time.deltaTime;
		if (delay < delta) {
			return;
		}
		delay = 0f;
		float sqrDist = (this.planet.SubTarget.position - this.transform.TransformPoint(this.center)).sqrMagnitude / this.transform.localScale.x;

		if (sqrDist < subLimit) {
			if (this.childDepth == 0) {
				this.Subdivide (true, subLimit - sqrDist);
			}
		} 
		else if (sqrDist > unSubLimit) {
			if (this.childDepth == 1) {
				this.UnSubdivide (true);
			}
		}
	}

	public void ComputeSubLimits () {
		float squareSize = Mathf.FloorToInt (Mathf.Pow (2, this.planet.maxSubDegree - subDegree)) * StellarSystem.squareLength * StellarSystem.TileSize;

		float distHalfScreen = squareSize / Mathf.Tan (Mathf.Deg2Rad * 30f);

		this.subLimit = distHalfScreen * 1.5f;
		this.subLimit *= this.subLimit;
		this.unSubLimit = distHalfScreen * 3f;
		this.unSubLimit *= this.unSubLimit;
	}

	public void Initialize () {
		if (this.parent != null) {
			this.parent.RemoveMesh ();
		}

		this.xPos = (this.iPos - 1) * StellarSystem.squareLength * Mathf.FloorToInt (Mathf.Pow (2f,  this.planet.maxSubDegree - subDegree));
		this.yPos = (this.jPos - 1) * StellarSystem.squareLength * Mathf.FloorToInt (Mathf.Pow (2f,  this.planet.maxSubDegree - subDegree));
		this.zPos = (this.kPos - 1) * StellarSystem.squareLength * Mathf.FloorToInt (Mathf.Pow (2f,  this.planet.maxSubDegree - subDegree));

		this.indexSize = Mathf.FloorToInt (Mathf.Pow (2f, subDegree)) + 1;

		this.name = "Square " + this.subDegree + "." + this.iPos + ":" + this.jPos + ":" + this.kPos;

		this.BuildMesh ();

		MeshFilter mf = this.GetComponent <MeshFilter> ();
		this.center = mf.sharedMesh.vertices [mf.sharedMesh.vertexCount / 2];

		if (this.subDegree == this.planet.maxSubDegree) {
			this.GetComponent<MeshCollider> ().sharedMesh = mf.sharedMesh;
		}

		this.ComputeSubLimits ();

		this.initialized = true;
	}

	public void RemoveMesh () {
		this.removeMeshRequests ++;
		if (this.removeMeshRequests >= 4) {
			this.removeMeshRequests = 0;
			Mesh m = this.GetComponent <MeshFilter> ().sharedMesh;
			DestroyImmediate (m);
			this.GetComponent <MeshFilter> ().sharedMesh = null;
		}
	}

	public Vector3 EvaluateVertex (int x, int y, int z) {
		float value = Evaluate (x, y, z);

		float xRad = -45f + 90f * (float) x / (float) this.planet.heightMapRange;
		float yRad = -45f + 90f * (float) y / (float) this.planet.heightMapRange;
		float zRad = -45f + 90f * (float) z / (float) this.planet.heightMapRange;

		xRad = Mathf.Deg2Rad * xRad;
		yRad = Mathf.Deg2Rad * yRad;
		zRad = Mathf.Deg2Rad * zRad;

		return new Vector3 (Mathf.Sin (xRad) / Mathf.Cos (xRad), Mathf.Sin (yRad) / Mathf.Cos (yRad), Mathf.Sin (zRad) / Mathf.Cos (zRad)).normalized * (this.planet.radius + value / 2f * this.planet.heightRange * StellarSystem.TileSize);
	}

	public float Evaluate (int x, int y, int z) {
		float value = 0f;
		int degree =  this.planet.maxSubDegree + StellarSystem.squareLengthPow;

		for (int d = 2; d < degree; d++) {
			int range = Mathf.FloorToInt(Mathf.Pow (2f, degree - d));
			int x0 = (x / range) * range;
			int y0 = (y / range) * range;
			int z0 = (z / range) * range;
			int x1 = x0 + range;
			int y1 = y0 + range;
			int z1 = z0 + range;

			float xd = (float) (x % range) / (float) range;
			float yd = (float) (y % range) / (float) range;
			float zd = (float) (z % range) / (float) range;

			float[][][] f = new float[2][][];
			for (int i = 0; i < 2; i++) {
				f [i] = new float[2][];
				for (int j = 0; j < 2; j++) {
					f [i][j] = new float[2];
				}
			}

			f [0][0][0] = this.planet.Randomizer.Rand (x0 ,y0, z0, d);
			f [0][0][1] = this.planet.Randomizer.Rand (x0 ,y0, z1, d);
			f [0][1][0] = this.planet.Randomizer.Rand (x0 ,y1, z0, d);
			f [0][1][1] = this.planet.Randomizer.Rand (x0 ,y1, z1, d);
			f [1][0][0] = this.planet.Randomizer.Rand (x1 ,y0, z0, d);
			f [1][0][1] = this.planet.Randomizer.Rand (x1 ,y0, z1, d);
			f [1][1][0] = this.planet.Randomizer.Rand (x1 ,y1, z0, d);
			f [1][1][1] = this.planet.Randomizer.Rand (x1 ,y1, z1, d);

			float v00 = f[0][0][0] * (1f - xd) + f [1][0][0] * xd;
			float v10 = f[0][1][0] * (1f - xd) + f [1][1][0] * xd;
			float v01 = f[0][0][1] * (1f - xd) + f [1][0][1] * xd;
			float v11 = f[0][1][1] * (1f - xd) + f [1][1][1] * xd;

			float v0 = v00 * (1f - yd) + v10 * yd;
			float v1 = v01 * (1f - yd) + v11 * yd;

			float v = v0 * (1f - zd) + v1 * zd;
			
			value += v / Mathf.FloorToInt(Mathf.Pow (2f, d));
		}

		return value;
	}

	public int[] GetTrianglesAntiClockWise () {
		List<int> trianglesList = new List<int> ();

		for (int i = 0; i <  StellarSystem.heightMapSize - 1; i++) {
			for (int j = 0; j <  StellarSystem.heightMapSize - 1; j++) {
				trianglesList.Add (i + j * ( StellarSystem.heightMapSize));
				trianglesList.Add ((i + 1) + j * ( StellarSystem.heightMapSize));
				trianglesList.Add ((i + 1) + (j + 1) * ( StellarSystem.heightMapSize));
				trianglesList.Add ((i + 1) + (j + 1) * ( StellarSystem.heightMapSize));
				trianglesList.Add (i + (j + 1) * ( StellarSystem.heightMapSize));
				trianglesList.Add (i + j * ( StellarSystem.heightMapSize));
			}
		}

		return trianglesList.ToArray ();
	}

	public int[] GetTrianglesClockWise () {
		List<int> trianglesList = new List<int> ();
		
		for (int i = 0; i <  StellarSystem.heightMapSize - 1; i++) {
			for (int j = 0; j <  StellarSystem.heightMapSize - 1; j++) {
				trianglesList.Add (i + j * ( StellarSystem.heightMapSize));
				trianglesList.Add ((i + 1) + (j + 1) * ( StellarSystem.heightMapSize));
				trianglesList.Add ((i + 1) + j * ( StellarSystem.heightMapSize));
				trianglesList.Add ((i + 1) + (j + 1) * ( StellarSystem.heightMapSize));
				trianglesList.Add (i + j * ( StellarSystem.heightMapSize));
				trianglesList.Add (i + (j + 1) * ( StellarSystem.heightMapSize));
			}
		}
		
		return trianglesList.ToArray ();
	}

	public void BuildMesh () {

		Mesh m = new Mesh ();
		List<Vector3> verticesList = new List<Vector3> ();
		List<Vector3> normalsList = new List<Vector3> ();
		int step = Mathf.FloorToInt (Mathf.Pow (2,  this.planet.maxSubDegree - subDegree));

		Vector3 [][] bufferVertices = new Vector3[StellarSystem.heightMapSize][];
		for (int i = 0; i < StellarSystem.heightMapSize; i++) {
			bufferVertices [i] = new Vector3[StellarSystem.heightMapSize];
		}

		if (this.kPos == 0) {
			for (int i = 0; i <  StellarSystem.heightMapSize; i++) {
				for (int j = 0; j <  StellarSystem.heightMapSize; j++) {
					verticesList.Add (EvaluateVertex (xPos + i * step, yPos + j * step, 0));
					bufferVertices [i][j] = verticesList [verticesList.Count - 1];
				}
			}

			for (int i = 0; i <  StellarSystem.heightMapSize; i++) {
				for (int j = 0; j <  StellarSystem.heightMapSize; j++) {
					Vector3 vPlusI;
					if (i + 1 < StellarSystem.heightMapSize) {
						vPlusI = bufferVertices [i + 1][j];
					}
					else if (xPos + (i + 1) * step < this.planet.heightMapRange) {
						vPlusI = EvaluateVertex (xPos + (i + 1) * step, yPos + j * step, 0);
					}
					else {
						vPlusI = EvaluateVertex (this.planet.heightMapRange - 1, yPos + j * step, step);
					}
					vPlusI = vPlusI - bufferVertices [i][j];

					Vector3 vMinusI;
					if (i - 1 >= 0) {
						vMinusI = bufferVertices [i - 1][j];
					}
					else if (xPos + (i - 1) * step >= 0) {
						vMinusI = EvaluateVertex (xPos + (i - 1) * step, yPos + j * step, 0);
					}
					else {
						vMinusI = EvaluateVertex (0, yPos + j * step, step);
					}
					vMinusI = vMinusI - bufferVertices [i][j];

					Vector3 vPlusJ;
					if (j + 1 < StellarSystem.heightMapSize) {
						vPlusJ = bufferVertices [i][j + 1];
					}
					else if (yPos + (j + 1) * step < this.planet.heightMapRange) {
						vPlusJ = EvaluateVertex (xPos + i * step, yPos + (j + 1) * step, 0);
					}
					else {
						vPlusJ = EvaluateVertex (xPos + i * step, this.planet.heightMapRange - 1, step);
					}
					vPlusJ = vPlusJ - bufferVertices [i][j];

					Vector3 vMinusJ;
					if (j - 1 >= 0) {
						vMinusJ = bufferVertices [i][j - 1];
					}
					else if (yPos + (j - 1) * step >= 0) {
						vMinusJ = EvaluateVertex (xPos + i * step, yPos + (j - 1) * step, 0);
					}
					else {
						vMinusJ = EvaluateVertex (xPos + i * step, 0, step);
					}
					vMinusJ = vMinusJ - bufferVertices [i][j];

					Vector3 n = Vector3.Cross (vPlusI, vPlusJ) + Vector3.Cross (vPlusJ, vMinusI) + Vector3.Cross (vMinusI, vMinusJ) + Vector3.Cross (vMinusJ, vPlusI);

					normalsList.Add (- n.normalized);
				}
			}

			m.vertices = verticesList.ToArray ();
			m.triangles = GetTrianglesAntiClockWise ();
			m.normals = normalsList.ToArray ();
		} 
		else if (this.jPos == 0) {
			for (int i = 0; i <  StellarSystem.heightMapSize; i++) {
				for (int k = 0; k <  StellarSystem.heightMapSize; k++) {
					verticesList.Add (EvaluateVertex (xPos + i * step, 0, zPos + k * step));
					bufferVertices [i][k] = verticesList [verticesList.Count - 1];
				}
			}

			for (int i = 0; i <  StellarSystem.heightMapSize; i++) {
				for (int k = 0; k <  StellarSystem.heightMapSize; k++) {
					Vector3 vPlusI;
					if (i + 1 < StellarSystem.heightMapSize) {
						vPlusI = bufferVertices [i + 1][k];
					}
					else if (xPos + (i + 1) * step < this.planet.heightMapRange) {
						vPlusI = EvaluateVertex (xPos + (i + 1) * step, 0, zPos + k * step);
					}
					else {
						vPlusI = EvaluateVertex (this.planet.heightMapRange - 1, step, zPos + k * step);
					}
					vPlusI = vPlusI - bufferVertices [i][k];
					
					Vector3 vMinusI;
					if (i - 1 >= 0) {
						vMinusI = bufferVertices [i - 1][k];
					}
					else if (xPos + (i - 1) * step >= 0) {
						vMinusI = EvaluateVertex (xPos + (i - 1) * step, 0, zPos + k * step);
					}
					else {
						vMinusI = EvaluateVertex (0, step, zPos + k * step);
					}
					vMinusI = vMinusI - bufferVertices [i][k];
					
					Vector3 vPlusK;
					if (k + 1 < StellarSystem.heightMapSize) {
						vPlusK = bufferVertices [i][k + 1];
					}
					else if (zPos + (k + 1) * step < this.planet.heightMapRange) {
						vPlusK = EvaluateVertex (xPos + i * step, 0, zPos + (k + 1) * step);
					}
					else {
						vPlusK = EvaluateVertex (xPos + i * step, step, this.planet.heightMapRange - 1);
					}
					vPlusK = vPlusK - bufferVertices [i][k];
					
					Vector3 vMinusK;
					if (k - 1 >= 0) {
						vMinusK = bufferVertices [i][k - 1];
					}
					else if (zPos + (k - 1) * step >= 0) {
						vMinusK = EvaluateVertex (xPos + i * step, 0, zPos + (k - 1) * step);
					}
					else {
						vMinusK = EvaluateVertex (xPos + i * step, step, 0);
					}
					vMinusK = vMinusK - bufferVertices [i][k];
					
					Vector3 n = Vector3.Cross (vPlusI, vPlusK) + Vector3.Cross (vPlusK, vMinusI) + Vector3.Cross (vMinusI, vMinusK) + Vector3.Cross (vMinusK, vPlusI);
					
					normalsList.Add (n.normalized);
				}
			}
			m.vertices = verticesList.ToArray ();
			m.triangles = GetTrianglesClockWise ();
			m.normals = normalsList.ToArray ();
		}
		else if (this.iPos == 0) {
			for (int j = 0; j <  StellarSystem.heightMapSize; j++) {
				for (int k = 0; k <  StellarSystem.heightMapSize; k++) {
					verticesList.Add (EvaluateVertex (0, yPos + j * step, zPos + k * step));
					bufferVertices [j][k] = verticesList [verticesList.Count - 1];
				}
			}
			for (int j = 0; j <  StellarSystem.heightMapSize; j++) {
				for (int k = 0; k <  StellarSystem.heightMapSize; k++) {
					Vector3 vPlusJ;
					if (j + 1 < StellarSystem.heightMapSize) {
						vPlusJ = bufferVertices [j + 1][k];
					}
					else if (yPos + (j + 1) * step < this.planet.heightMapRange) {
						vPlusJ = EvaluateVertex (0, yPos + (j + 1) * step, zPos + k * step);
					}
					else {
						vPlusJ = EvaluateVertex (step, this.planet.heightMapRange - 1, zPos + k * step);
					}
					vPlusJ = vPlusJ - bufferVertices [j][k];
					
					Vector3 vMinusJ;
					if (j - 1 >= 0) {
						vMinusJ = bufferVertices [j - 1][k];
					}
					else if (yPos + (j - 1) * step >= 0) {
						vMinusJ = EvaluateVertex (0, yPos + (j - 1) * step, zPos + k * step);
					}
					else {
						vMinusJ = EvaluateVertex (step, 0, zPos + k * step);
					}
					vMinusJ = vMinusJ - bufferVertices [j][k];
					
					Vector3 vPlusK;
					if (k + 1 < StellarSystem.heightMapSize) {
						vPlusK = bufferVertices [j][k + 1];
					}
					else if (zPos + (k + 1) * step < this.planet.heightMapRange) {
						vPlusK = EvaluateVertex (0, yPos + j * step, zPos + (k + 1) * step);
					}
					else {
						vPlusK = EvaluateVertex (step, yPos + j * step, this.planet.heightMapRange - 1);
					}
					vPlusK = vPlusK - bufferVertices [j][k];
					
					Vector3 vMinusK;
					if (k - 1 >= 0) {
						vMinusK = bufferVertices [j][k - 1];
					}
					else if (zPos + (k - 1) * step >= 0) {
						vMinusK = EvaluateVertex (0, yPos + j * step, zPos + (k - 1) * step);
					}
					else {
						vMinusK = EvaluateVertex (step, yPos + j * step, 0);
					}
					vMinusK = vMinusK - bufferVertices [j][k];
					
					Vector3 n = Vector3.Cross (vPlusJ, vPlusK) + Vector3.Cross (vPlusK, vMinusJ) + Vector3.Cross (vMinusJ, vMinusK) + Vector3.Cross (vMinusK, vPlusJ);
					
					normalsList.Add (- n.normalized);
				}
			}
			m.vertices = verticesList.ToArray ();
			m.triangles = GetTrianglesAntiClockWise ();
			m.normals = normalsList.ToArray ();
		}
		else if (this.kPos == indexSize) {
			for (int i = 0; i <  StellarSystem.heightMapSize; i++) {
				for (int j = 0; j <  StellarSystem.heightMapSize; j++) {
					verticesList.Add (EvaluateVertex (xPos + i * step, yPos + j * step, zPos));
					bufferVertices [i][j] = verticesList [verticesList.Count - 1];
				}
			}

			for (int i = 0; i <  StellarSystem.heightMapSize; i++) {
				for (int j = 0; j <  StellarSystem.heightMapSize; j++) {
					Vector3 vPlusI;
					if (i + 1 < StellarSystem.heightMapSize) {
						vPlusI = bufferVertices [i + 1][j];
					}
					else if (xPos + (i + 1) * step < this.planet.heightMapRange) {
						vPlusI = EvaluateVertex (xPos + (i + 1) * step, yPos + j * step, this.planet.heightMapRange - 1);
					}
					else {
						vPlusI = EvaluateVertex (this.planet.heightMapRange - 1, yPos + j * step, this.planet.heightMapRange - 1 - step);
					}
					vPlusI = vPlusI - bufferVertices [i][j];
					
					Vector3 vMinusI;
					if (i - 1 >= 0) {
						vMinusI = bufferVertices [i - 1][j];
					}
					else if (xPos + (i - 1) * step >= 0) {
						vMinusI = EvaluateVertex (xPos + (i - 1) * step, yPos + j * step, this.planet.heightMapRange - 1);
					}
					else {
						vMinusI = EvaluateVertex (0, yPos + j * step, this.planet.heightMapRange - 1 - step);
					}
					vMinusI = vMinusI - bufferVertices [i][j];
					
					Vector3 vPlusJ;
					if (j + 1 < StellarSystem.heightMapSize) {
						vPlusJ = bufferVertices [i][j + 1];
					}
					else if (yPos + (j + 1) * step < this.planet.heightMapRange) {
						vPlusJ = EvaluateVertex (xPos + i * step, yPos + (j + 1) * step, this.planet.heightMapRange - 1);
					}
					else {
						vPlusJ = EvaluateVertex (xPos + i * step, this.planet.heightMapRange - 1, this.planet.heightMapRange - 1 - step);
					}
					vPlusJ = vPlusJ - bufferVertices [i][j];
					
					Vector3 vMinusJ;
					if (j - 1 >= 0) {
						vMinusJ = bufferVertices [i][j - 1];
					}
					else if (yPos + (j - 1) * step >= 0) {
						vMinusJ = EvaluateVertex (xPos + i * step, yPos + (j - 1) * step, this.planet.heightMapRange - 1);
					}
					else {
						vMinusJ = EvaluateVertex (xPos + i * step, 0, this.planet.heightMapRange - 1 - step);
					}
					vMinusJ = vMinusJ - bufferVertices [i][j];
					
					Vector3 n = Vector3.Cross (vPlusI, vPlusJ) + Vector3.Cross (vPlusJ, vMinusI) + Vector3.Cross (vMinusI, vMinusJ) + Vector3.Cross (vMinusJ, vPlusI);
					
					normalsList.Add (n.normalized);
				}
			}

			m.vertices = verticesList.ToArray ();
			m.triangles = GetTrianglesClockWise ();
			m.normals = normalsList.ToArray ();
		}
		else if (this.jPos == indexSize) {
			for (int i = 0; i <  StellarSystem.heightMapSize; i++) {
				for (int k = 0; k <  StellarSystem.heightMapSize; k++) {
					verticesList.Add (EvaluateVertex (xPos + i * step, yPos, zPos + k * step));
					bufferVertices [i][k] = verticesList [verticesList.Count - 1];
				}
			}

			for (int i = 0; i <  StellarSystem.heightMapSize; i++) {
				for (int k = 0; k <  StellarSystem.heightMapSize; k++) {
					Vector3 vPlusI;
					if (i + 1 < StellarSystem.heightMapSize) {
						vPlusI = bufferVertices [i + 1][k];
					}
					else if (xPos + (i + 1) * step < this.planet.heightMapRange) {
						vPlusI = EvaluateVertex (xPos + (i + 1) * step, this.planet.heightMapRange - 1, zPos + k * step);
					}
					else {
						vPlusI = EvaluateVertex (this.planet.heightMapRange - 1, this.planet.heightMapRange - 1 - step, zPos + k * step);
					}
					vPlusI = vPlusI - bufferVertices [i][k];
					
					Vector3 vMinusI;
					if (i - 1 >= 0) {
						vMinusI = bufferVertices [i - 1][k];
					}
					else if (xPos + (i - 1) * step >= 0) {
						vMinusI = EvaluateVertex (xPos + (i - 1) * step, this.planet.heightMapRange - 1, zPos + k * step);
					}
					else {
						vMinusI = EvaluateVertex (0, this.planet.heightMapRange - 1 - step, zPos + k * step);
					}
					vMinusI = vMinusI - bufferVertices [i][k];
					
					Vector3 vPlusK;
					if (k + 1 < StellarSystem.heightMapSize) {
						vPlusK = bufferVertices [i][k + 1];
					}
					else if (zPos + (k + 1) * step < this.planet.heightMapRange) {
						vPlusK = EvaluateVertex (xPos + i * step, this.planet.heightMapRange - 1, zPos + (k + 1) * step);
					}
					else {
						vPlusK = EvaluateVertex (xPos + i * step, this.planet.heightMapRange - 1 - step, this.planet.heightMapRange - 1);
					}
					vPlusK = vPlusK - bufferVertices [i][k];
					
					Vector3 vMinusK;
					if (k - 1 >= 0) {
						vMinusK = bufferVertices [i][k - 1];
					}
					else if (zPos + (k - 1) * step >= 0) {
						vMinusK = EvaluateVertex (xPos + i * step, this.planet.heightMapRange - 1, zPos + (k - 1) * step);
					}
					else {
						vMinusK = EvaluateVertex (xPos + i * step, this.planet.heightMapRange - 1 - step, 0);
					}
					vMinusK = vMinusK - bufferVertices [i][k];
					
					Vector3 n = Vector3.Cross (vPlusI, vPlusK) + Vector3.Cross (vPlusK, vMinusI) + Vector3.Cross (vMinusI, vMinusK) + Vector3.Cross (vMinusK, vPlusI);
					
					normalsList.Add (- n.normalized);
				}
			}
			m.vertices = verticesList.ToArray ();
			m.triangles = GetTrianglesAntiClockWise ();
			m.normals = normalsList.ToArray ();
		}
		else if (this.iPos == indexSize) {
			for (int j = 0; j <  StellarSystem.heightMapSize; j++) {
				for (int k = 0; k <  StellarSystem.heightMapSize; k++) {
					verticesList.Add (EvaluateVertex (xPos, yPos + j * step, zPos + k * step));
					bufferVertices [j][k] = verticesList [verticesList.Count - 1];
				}
			}
			for (int j = 0; j <  StellarSystem.heightMapSize; j++) {
				for (int k = 0; k <  StellarSystem.heightMapSize; k++) {
					Vector3 vPlusJ;
					if (j + 1 < StellarSystem.heightMapSize) {
						vPlusJ = bufferVertices [j + 1][k];
					}
					else if (yPos + (j + 1) * step < this.planet.heightMapRange) {
						vPlusJ = EvaluateVertex (this.planet.heightMapRange - 1, yPos + (j + 1) * step, zPos + k * step);
					}
					else {
						vPlusJ = EvaluateVertex (this.planet.heightMapRange - 1 - step, this.planet.heightMapRange - 1, zPos + k * step);
					}
					vPlusJ = vPlusJ - bufferVertices [j][k];
					
					Vector3 vMinusJ;
					if (j - 1 >= 0) {
						vMinusJ = bufferVertices [j - 1][k];
					}
					else if (yPos + (j - 1) * step >= 0) {
						vMinusJ = EvaluateVertex (this.planet.heightMapRange - 1, yPos + (j - 1) * step, zPos + k * step);
					}
					else {
						vMinusJ = EvaluateVertex (this.planet.heightMapRange - 1 - step, 0, zPos + k * step);
					}
					vMinusJ = vMinusJ - bufferVertices [j][k];
					
					Vector3 vPlusK;
					if (k + 1 < StellarSystem.heightMapSize) {
						vPlusK = bufferVertices [j][k + 1];
					}
					else if (zPos + (k + 1) * step < this.planet.heightMapRange) {
						vPlusK = EvaluateVertex (this.planet.heightMapRange - 1, yPos + j * step, zPos + (k + 1) * step);
					}
					else {
						vPlusK = EvaluateVertex (this.planet.heightMapRange - 1 - step, yPos + j * step, this.planet.heightMapRange - 1);
					}
					vPlusK = vPlusK - bufferVertices [j][k];
					
					Vector3 vMinusK;
					if (k - 1 >= 0) {
						vMinusK = bufferVertices [j][k - 1];
					}
					else if (zPos + (k - 1) * step >= 0) {
						vMinusK = EvaluateVertex (this.planet.heightMapRange - 1, yPos + j * step, zPos + (k - 1) * step);
					}
					else {
						vMinusK = EvaluateVertex (this.planet.heightMapRange - 1 - step, yPos + j * step, 0);
					}
					vMinusK = vMinusK - bufferVertices [j][k];
					
					Vector3 n = Vector3.Cross (vPlusJ, vPlusK) + Vector3.Cross (vPlusK, vMinusJ) + Vector3.Cross (vMinusJ, vMinusK) + Vector3.Cross (vMinusK, vPlusJ);
					
					normalsList.Add (n.normalized);
				}
			}
			m.vertices = verticesList.ToArray ();
			m.triangles = GetTrianglesClockWise ();
			m.normals = normalsList.ToArray ();
		}

		this.GetComponent<MeshFilter> ().sharedMesh = m;
	}

	public void UnSubdivide (bool usePlanetManager) {
		if (this.childDepth != 1) {
			return;
		}

		foreach (PlanetSquare ps in children) {
			DestroyImmediate (ps.gameObject);
		}

		this.childDepth = 0;
		if (this.parent != null) {
			this.parent.childDepth = 1;
		}

		if (usePlanetManager) {
			StellarSystem.Manager.Add (this, 0f);
		} 
		else {
			this.Initialize ();
		}
	}

	public void Subdivide (bool usePlanetManager, float priority) {
		if (this.childDepth != 0) {
			return;
		}

		if (this.subDegree == this.planet.maxSubDegree) {
			return;
		}

		this.childDepth = 1;
		if (this.parent != null) {
			this.parent.childDepth = 2;
		}
		this.children = new PlanetSquare [4];

		for (int a = 0; a < 2; a++) {
			for (int b = 0; b < 2; b++) {
				GameObject g = GameObject.Instantiate<GameObject> (this.planet.squareTemplate);
				g.transform.parent = this.transform;
				g.transform.localPosition = Vector3.zero;
				g.transform.localRotation = Quaternion.identity;
				g.transform.localScale = Vector3.one;
				PlanetSquare ps = g.GetComponent <PlanetSquare> ();
				ps.subDegree = this.subDegree + 1;

				if ((this.kPos == 0) || (this.kPos == this.indexSize)) {
					ps.iPos = 2 * this.iPos - a;
					ps.jPos = 2 * this.jPos - b;
					ps.kPos = this.kPos;
					if (ps.kPos != 0) {
						ps.kPos = Mathf.FloorToInt (Mathf.Pow (2f, ps.subDegree)) + 1;
					}
				}
				else if ((this.jPos == 0) || (this.jPos == this.indexSize)) {
					ps.iPos = 2 * this.iPos - a;
					ps.jPos = this.jPos;
					if (ps.jPos != 0) {
						ps.jPos = Mathf.FloorToInt (Mathf.Pow (2f, ps.subDegree)) + 1;
					}
					ps.kPos = 2 * this.kPos - b;
				}
				else if ((this.iPos == 0) || (this.iPos == this.indexSize)) {
					ps.iPos = this.iPos;
					if (ps.iPos != 0) {
						ps.iPos = Mathf.FloorToInt (Mathf.Pow (2f, ps.subDegree)) + 1;
					}
					ps.jPos = 2 * this.jPos - b;
					ps.kPos = 2 * this.kPos - a;
				}
				
				ps.parent = this;
				ps.planet = this.planet;
				this.children [b + 2 * a] = ps;
				ps.name = "Square " + ps.subDegree + "." + ps.iPos + ":" + ps.jPos + ":" + ps.kPos;

				if (usePlanetManager) {
					StellarSystem.Manager.Add (ps, priority);
				}
				else {
					ps.Initialize ();
				}
			}
		}
	}
}
