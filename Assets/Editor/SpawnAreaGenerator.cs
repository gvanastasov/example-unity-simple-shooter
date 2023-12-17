using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static class SpawnAreaGenerator
{
    private static string meshSavePath = "Assets/Meshes/SpawnAreaMesh_g.asset";

    [MenuItem("Simple Shooter/Genera Spawn Area")]
    public static void Execute()
    {
        GameObject meshObject = new GameObject("SpawnAreaMesh_g");
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();

        meshFilter.mesh = GenerateMesh();

        meshObject.transform.position = Vector3.zero;
        meshObject.transform.rotation = Quaternion.identity;
        meshObject.transform.localScale = Vector3.one;
    }

    private static Mesh GenerateMesh()
    {
        var triangles = NavMesh.CalculateTriangulation();

        // Create a new mesh
        Mesh mesh = new Mesh();
        mesh.vertices = triangles.vertices;
        mesh.triangles = triangles.indices;

        // Save the mesh to a file
        SaveMesh(mesh, meshSavePath);

        return mesh;
    }

    private static void SaveMesh(Mesh mesh, string path)
    {
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
    }
}
