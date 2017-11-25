using UnityEngine;
using System.Collections.Generic;

//internal 사용 하기
//Static 함수들....
//Singleton을 사용하는 거 생각해보기
//어차피 프로그램이 시작되고 종료될때까지 남아있기는 하지만
namespace DecalSystem
{
    internal class DecalPolygon
    {
        internal List<Vector3> vertices = new List<Vector3>(9);

        internal DecalPolygon(params Vector3[] vts)
        {
            vertices.AddRange(vts);
        }

        internal static DecalPolygon ClipPolygon(DecalPolygon polygon, Plane plane)
        {
            bool[] positive = new bool[9];
            int positiveCount = 0;

            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                positive[i] = !plane.GetSide(polygon.vertices[i]);
                if (positive[i]) positiveCount++;
            }

            if (positiveCount == 0) return null;
            if (positiveCount == polygon.vertices.Count) return polygon;

            DecalPolygon tempPolygon = new DecalPolygon();

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

                    Vector3 v = LineCast(plane, v1, v2);
                    tempPolygon.vertices.Add(v);
                }
            }

            return tempPolygon;
        }

        internal static Vector3 LineCast(Plane plane, Vector3 a, Vector3 b)
        {
            float dis = 0;
            Ray ray = new Ray(a, b - a);
            plane.Raycast(ray, out dis);

            return ray.GetPoint(dis);
        }

    }
}