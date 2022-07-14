using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedCollisionHelper : MonoBehaviour
{
    private CWeightList[] nodeWeights;
    private Vector3[] newVert;
    private Mesh mesh;
    private MeshCollider collide;

    public float time;
    public float maxtime;
    void Start()
    {
        time = 10f;
        SkinnedMeshRenderer rend = GetComponent<SkinnedMeshRenderer>();
        collide = GetComponent<MeshCollider>();

        if (collide != null && rend != null)
        {
            Mesh baseMesh = rend.sharedMesh;
            mesh = new Mesh();
            mesh.vertices = baseMesh.vertices;
            mesh.uv = baseMesh.uv;
            mesh.triangles = baseMesh.triangles;
            newVert = new Vector3[baseMesh.vertices.Length];
            short i;
            nodeWeights = new CWeightList[rend.bones.Length];

            for (i = 0; i < rend.bones.Length; i++)
            {
                nodeWeights[i] = new CWeightList();
                nodeWeights[i].transform = rend.bones[i];
            }

            Vector3 localPt;
            for (i = 0; i < baseMesh.vertices.Length; i++)
            {
                BoneWeight bw = baseMesh.boneWeights[i];
                if (bw.weight0 != 0.0f)
                {
                    localPt = baseMesh.bindposes[bw.boneIndex0].MultiplyPoint3x4(baseMesh.vertices[i]);
                    nodeWeights[bw.boneIndex0].weights.Add(new CVertexWeight(i, localPt, bw.weight0));
                }
                if (bw.weight1 != 0.0f)
                {
                    localPt = baseMesh.bindposes[bw.boneIndex1].MultiplyPoint3x4(baseMesh.vertices[i]);
                    nodeWeights[bw.boneIndex1].weights.Add(new CVertexWeight(i, localPt, bw.weight1));
                }
                if (bw.weight2 != 0.0f)
                {
                    localPt = baseMesh.bindposes[bw.boneIndex2].MultiplyPoint3x4(baseMesh.vertices[i]);
                    nodeWeights[bw.boneIndex2].weights.Add(new CVertexWeight(i, localPt, bw.weight2));
                }
                if (bw.weight3 != 0.0f)
                {
                    localPt = baseMesh.bindposes[bw.boneIndex3].MultiplyPoint3x4(baseMesh.vertices[i]);
                    nodeWeights[bw.boneIndex3].weights.Add(new CVertexWeight(i, localPt, bw.weight3));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            time = maxtime;
            UpdateCollisionMesh();
        }
    }

    public void UpdateCollisionMesh()
    {
        if (mesh != null)
        {
            for (int i = 0; i < newVert.Length; i++)
            {
                newVert[i] = new Vector3(0, 0, 0);
            }

            foreach (CWeightList wList in nodeWeights)
            {
                foreach (CVertexWeight vw in wList.weights)
                {
                    newVert[vw.index] += wList.transform.localToWorldMatrix.MultiplyPoint3x4(vw.localPosition) *
                                         vw.weight;
                }
            }

            for (int i = 0; i < newVert.Length; i++)
            {
                newVert[i] = transform.InverseTransformPoint(newVert[i]);
            }

            mesh.vertices = newVert;
            mesh.RecalculateBounds();
            collide.sharedMesh = mesh;
        }
    }
}

class CVertexWeight
{
    public int index;
    public Vector3 localPosition;
    public float weight;

    public CVertexWeight(int i, Vector3 p, float w)
    {
        index = i;
        localPosition = p;
        weight = w;
    }
}

class CWeightList
{
    public Transform transform;
    public ArrayList weights;

    public CWeightList()
    {
        weights = new ArrayList();
    }
}