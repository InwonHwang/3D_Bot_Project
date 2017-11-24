using UnityEngine;
using System.Collections.Generic;

// 싱글톤
public class MeshBuilder : Singleton<MeshBuilder>
{
    class Polygon
    {
        public List<Vector3> vertices = new List<Vector3>(9);

        public Polygon(params Vector3[] vts)
        {
            vertices.AddRange(vts);
        }        
    } // class Polygon

    List<Vector3> vertice = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> texCoords = new List<Vector2>();
    List<int> indice = new List<int>();

    public Mesh BuildMesh(Vector3 position, Vector3 size, Sprite sprite, LayerMask layer_mask, float push_dist = 0.01f)
    {
        Bounds bounds = new Bounds(position, size);
        
        var affected_objects = getAffectedObjects(bounds, layer_mask);

        foreach (GameObject obj in affected_objects)
        {
            buildMeshForObject(obj, position, size, sprite);
        }

        push(push_dist);

        Mesh mesh = createMesh();

        return mesh;
    }
    
    public Mesh BuildMesh(Vector3[] vertice, Vector3[] nomrals, Vector2[] tex_coords, int[] indice)
    {
        return null;
    }

    void buildMeshForObject(GameObject affectedObject, Vector3 position, Vector3 size, Sprite sprite) // 버텍스, 노멀, 텍스쳐 좌표 추출.
    {
        var renderer = affectedObject.GetComponent<SkinnedMeshRenderer>();
        var mesh_filter = affectedObject.GetComponent<MeshFilter>();

        Mesh affected_mesh = renderer == null ? mesh_filter.sharedMesh : renderer.sharedMesh;
        if (affected_mesh == null) return;
                
        float max_angle = 180f;

        Plane right = new Plane(Vector3.right, position + size);
        Plane left = new Plane(-Vector3.right, position - size);

        Plane top = new Plane(Vector3.up, position + size);
        Plane bottom = new Plane(-Vector3.up, position - size);

        Plane front = new Plane(Vector3.forward, position + size);
        Plane back = new Plane(-Vector3.forward, position - size);


        Vector3[] vertices = affected_mesh.vertices;
        int[] triangles = affected_mesh.triangles;
        int startVertexCount = vertice.Count;
                
        Matrix4x4 matrix = affectedObject.transform.localToWorldMatrix;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            Vector3 v1 = matrix.MultiplyPoint(vertices[i1]);
            Vector3 v2 = matrix.MultiplyPoint(vertices[i2]);
            Vector3 v3 = matrix.MultiplyPoint(vertices[i3]);

            Vector3 side1 = v2 - v1;
            Vector3 side2 = v3 - v1;
            Vector3 normal = Vector3.Cross(side1, side2).normalized;
                        
            if (Vector3.Angle(-Camera.main.transform.forward, normal) >= max_angle) continue;

            Polygon poly = new Polygon(v1, v2, v3);

            poly = clipPolygon(poly, right);
            if (poly == null) continue;
            poly = clipPolygon(poly, left);
            if (poly == null) continue;

            poly = clipPolygon(poly, top);
            if (poly == null) continue;
            poly = clipPolygon(poly, bottom);
            if (poly == null) continue;

            poly = clipPolygon(poly, front);
            if (poly == null) continue;
            poly = clipPolygon(poly, back);
            if (poly == null) continue;

            addPolygon(poly, normal);
        }

        generateTexCoords(startVertexCount, sprite);
    }

    void addPolygon(Polygon poly, Vector3 normal)
    {
        int ind1 = addVertex(poly.vertices[0], normal);
        for (int i = 1; i < poly.vertices.Count - 1; i++)
        {
            int ind2 = addVertex(poly.vertices[i], normal);
            int ind3 = addVertex(poly.vertices[i + 1], normal);

            indice.Add(ind1);
            indice.Add(ind2);
            indice.Add(ind3);
        }
    }

    int addVertex(Vector3 vertex, Vector3 normal)
    {
        int index = findVertex(vertex);
        if (index == -1)
        {
            vertice.Add(vertex);
            normals.Add(normal);
            index = vertice.Count - 1;
        }
        else
        {
            Vector3 t = normals[index] + normal;
            normals[index] = t.normalized;
        }
        return (int)index;
    }

    int findVertex(Vector3 vertex)
    {
        for (int i = 0; i < vertice.Count; i++)
        {
            if (Vector3.Distance(vertice[i], vertex) < 0.01f)
            {
                return i;
            }
        }
        return -1;
    }

    bool isLayerContains(LayerMask mask, int layer)
    {
        return (mask.value & 1 << layer) != 0;
    }

    GameObject[] getAffectedObjects(Bounds bounds, LayerMask affectedLayers)
    {
        var renderers = (Renderer[])GameObject.FindObjectsOfType<Renderer>();
        
        List<GameObject> objects = new List<GameObject>();

        foreach (Renderer renderer in renderers)
        {
            if (!renderer.enabled) continue;
            if (!isLayerContains(affectedLayers, renderer.gameObject.layer)) continue;            

            if (bounds.Intersects(renderer.bounds))            
                objects.Add(renderer.gameObject);            
        }

        return objects.ToArray();
    }

    void push(float distance)
    {
        for (int i = 0; i < vertice.Count; i++)
        {
            Vector3 normal = normals[i];
            vertice[i] += normal * distance;
        }
    }

    Mesh createMesh()
    {
        if (indice.Count == 0)
        {
            return null;
        }
        Mesh mesh = new Mesh();

        mesh.vertices = vertice.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = texCoords.ToArray();
        mesh.uv2 = texCoords.ToArray();
        mesh.triangles = indice.ToArray();

        vertice.Clear();
        normals.Clear();
        texCoords.Clear();
        indice.Clear();

        return mesh;
    }

    void generateTexCoords(int start, Sprite sprite)
    {
        Rect rect = sprite.rect;
        rect.x /= sprite.texture.width;
        rect.y /= sprite.texture.height;
        rect.width /= sprite.texture.width;
        rect.height /= sprite.texture.height;

        for (int i = start; i < vertice.Count; i++)
        {
            Vector3 vertex = vertice[i];

            Vector2 uv = new Vector2(vertex.x + 0.5f, vertex.y + 0.5f);
            uv.x = Mathf.Lerp(rect.xMin, rect.xMax, uv.x);
            uv.y = Mathf.Lerp(rect.yMin, rect.yMax, uv.y);

            texCoords.Add(uv);
        }
    }

    Polygon clipPolygon(Polygon polygon, Plane plane)
    {
        bool[] positive = new bool[9];
        int positive_count = 0;

        for (int i = 0; i < polygon.vertices.Count; i++)
        {
            positive[i] = !plane.GetSide(polygon.vertices[i]);
            if (positive[i]) positive_count++;
        }

        if (positive_count == 0) return null;
        if (positive_count == polygon.vertices.Count) return polygon;

        Polygon tempPolygon = new Polygon();

        for (int i = 0; i < polygon.vertices.Count; i++)
        {
            int next = i + 1;
            next %= polygon.vertices.Count;

            if (positive[i])
            {
                tempPolygon.vertices.Add(polygon.vertices[i]);
            }

            if (positive[i] != positive[next])
            {
                Vector3 v1 = polygon.vertices[next];
                Vector3 v2 = polygon.vertices[i];

                Vector3 v = lineCast(plane, v1, v2);
                tempPolygon.vertices.Add(v);
            }
        }

        return tempPolygon;
    }

    Vector3 lineCast(Plane plane, Vector3 a, Vector3 b)
    {
        float dis = 0;
        Ray ray = new Ray(a, b - a);
        plane.Raycast(ray, out dis);

        return ray.GetPoint(dis);
    }


}// class MeshBuilder