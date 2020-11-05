using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using B83.Win32;
using UnityEditor;

public class ImageRGBViewer : MonoBehaviour
{
    public UnityEngine.UI.RawImage m_rawImage;
    
    float m_pointSize = 0.007f;

    public GameObject m_pointcloudPrefab;
    List<GameObject> m_pointclouds;
    
    public Texture2D m_texture;

    // Important to keep the instance alive while the hook is active.
    UnityDragAndDropHook m_hook;

    void Start()
    {
        m_texture = new Texture2D(512, 512);
        m_pointclouds = new List<GameObject>();
    }
    
    void LoadFromPath(string path) {
        if (path.Length != 0) {
            byte[] fileContent = File.ReadAllBytes(path);
            m_texture.LoadImage(fileContent);
        }
        
        m_rawImage.texture = m_texture;

        GeneratePoints();
    }

    void GeneratePoints() {
        for (int i = 0; i < m_pointclouds.Count; i++) {
            Destroy(m_pointclouds[i]);
        }
        m_pointclouds.Clear();
                
        GameObject first = Instantiate(m_pointcloudPrefab);
        first.transform.parent = this.transform;
        m_pointclouds.Add(first);
        PointCloud currentPC = first.GetComponent<PointCloud>();

        int size = m_texture.width * m_texture.height;
        int nbPoints = Mathf.Min(size, 65536/3);

        currentPC.m_vertices = new Vector3[nbPoints*3];
        currentPC.m_uvs = new Vector2[nbPoints*3];
        currentPC.m_triangles = new int[nbPoints*3];

        int go = 0;
        int index = 0;
        for (int i = 0; i < m_texture.width; i++) {
            for (int j = 0; j < m_texture.height; j++) {
                Color color = m_texture.GetPixel(i, j);
                
                // First face
                currentPC.m_vertices[index] = new Vector3(color.r, color.g, color.b);
                currentPC.m_uvs[index] = new Vector2(i / (float)m_texture.width, j / (float)m_texture.height);
                currentPC.m_triangles[index] = index;
                index++;

                currentPC.m_vertices[index] = new Vector3(color.r + m_pointSize, color.g, color.b);
                currentPC.m_uvs[index] = new Vector2(i / (float)m_texture.width, j / (float)m_texture.height);
                currentPC.m_triangles[index] = index;
                index++;

                currentPC.m_vertices[index] = new Vector3(color.r + m_pointSize/2.0f, color.g + m_pointSize, color.b);
                currentPC.m_uvs[index] = new Vector2(i / (float)m_texture.width, j / (float)m_texture.height);
                currentPC.m_triangles[index] = index;
                index++;

                if(index >= 65534) {
                    currentPC.UpdateCloud(m_texture);

                    GameObject next = Instantiate(m_pointcloudPrefab);
                    next.transform.parent = this.transform;
                    m_pointclouds.Add(next);

                    go++;
                    currentPC = m_pointclouds[go].GetComponent<PointCloud>();

                    index = 0;
                    nbPoints = Mathf.Min(size - (go * (65536 / 3)), 65536 / 3);

                    currentPC.m_vertices = new Vector3[nbPoints*3];
                    currentPC.m_uvs = new Vector2[nbPoints*3];
                    currentPC.m_triangles = new int[nbPoints*3];
                }
            }
        }
        currentPC.UpdateCloud(m_texture);
    }

#if UNITY_EDITOR
    public void ButtonLoad() {
        string path = EditorUtility.OpenFilePanel("", "", "");
        LoadFromPath(path);
    }
#endif

    // Drag image
    void OnEnable() {
        // must be created on the main thread to get the right thread id.
        m_hook = new UnityDragAndDropHook();
        m_hook.InstallHook();
        m_hook.OnDroppedFiles += OnFiles;
    }

    void OnDisable() {
        m_hook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos) {
        // do something with the dropped file names. aPos will contain the 
        // mouse position within the window where the files has been dropped.
        Debug.Log("Dropped " + aFiles.Count + " files at: " + aPos + "\n" +
            aFiles.Aggregate((a, b) => a + "\n" + b));

        LoadFromPath(aFiles[0]);
    }
}
