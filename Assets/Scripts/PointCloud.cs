using UnityEngine;

public class PointCloud : MonoBehaviour
{
    Mesh m_mesh;

    [HideInInspector]
    public Vector3[] m_vertices;
    [HideInInspector]
    public Vector2[] m_uvs;
    [HideInInspector]
    public int[] m_triangles;

    Material m_material;
    Texture2D m_texture;
    
    public void UpdateCloud(Texture2D texture)
    {
        m_material = GetComponent<MeshRenderer>().material;

        m_mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = m_mesh;
        m_mesh.vertices = m_vertices;
        m_mesh.uv = m_uvs;
        m_mesh.triangles = m_triangles;
        m_mesh.RecalculateNormals();

        m_texture = texture;
        m_material.SetTexture("_MainTex", m_texture);
    }
}
