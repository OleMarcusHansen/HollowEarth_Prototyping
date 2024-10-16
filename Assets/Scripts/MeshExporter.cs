using UnityEngine;
using System.IO;

public class MeshExporter : MonoBehaviour
{
    public static void ExportMesh(Mesh mesh, string path)
    {
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.Write("o " + mesh.name + "\n");
            foreach (Vector3 v in mesh.vertices)
            {
                sw.Write(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sw.Write("\n");
            foreach (Vector3 n in mesh.normals)
            {
                sw.Write(string.Format("vn {0} {1} {2}\n", n.x, n.y, n.z));
            }
            sw.Write("\n");
            foreach (Vector2 uv in mesh.uv)
            {
                sw.Write(string.Format("vt {0} {1}\n", uv.x, uv.y));
            }
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                sw.Write("\n");
                int[] triangles = mesh.GetTriangles(i);
                for (int j = 0; j < triangles.Length; j += 3)
                {
                    sw.Write(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[j] + 1, triangles[j + 1] + 1, triangles[j + 2] + 1));
                }
            }
        }
    }
}