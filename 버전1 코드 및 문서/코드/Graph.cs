using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

internal class GraphNode
{
    public List<GraphNode> Neighbour { get; set; }
    public EntityType.BoxData BoxData { get; set; }

    internal GraphNode()
    {
        Neighbour = new List<GraphNode>();
        BoxData = new EntityType.BoxData();
    }
}

internal class Graph
{
    List<GraphNode> listOfNode = new List<GraphNode>();

    internal void add(GraphNode node)
    {
        if (node == null) return;

        listOfNode.Add(node);
    }

    internal void buildGraph()
    {
        addNeighbourNode();
    }

    internal void clear()
    {
        listOfNode.Clear();
    }


    //void addNeighbourNode()
    //{
    //    for (int i = 0; i < listOfNode.Count; i++)
    //    {
    //        var v = listOfNode[i];
    //        for (int j = 0; j < listOfNode.Count; j++)
    //        {
    //            var c = listOfNode[j];
    //            if (v.Equals(c)) continue;

    //            if (v.BoxData.WarpId != null &&
    //                c.BoxData.WarpId != null &&
    //                v.BoxData.WarpId == c.BoxData.WarpId)
    //                v.Neighbour.Add(c);

    //            if (v.BoxData.Position == c.BoxData.Position + new Vector3(-1, 0, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(-1, -1, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(-1, 1, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(1, 0, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(1, -1, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(1, 1, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, 0, -1) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, -1, -1) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, 1, -1) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, 0, 1) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, -1, 1) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, 1, 1))
    //                v.Neighbour.Add(c);
    //        }

    //    }
    //}

    //void addNeighbourNode()
    //{
    //    for (int i = 0; i < listOfNode.Count; i++)
    //    {
    //        var v = listOfNode[i];
    //        for (int j = 0; j < listOfNode.Count; j++)
    //        {
    //            if (v.Equals(c)) continue;

    //            if (v.BoxData.Name.CompareTo(c.BoxData.Name) == 0)
    //                v.Neighbour.Add(c);

    //            if (v.BoxData.Position == c.BoxData.Position + new Vector3(-1, 0, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(-1, -1, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(-1, 1, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(1, 0, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(1, -1, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(1, 1, 0) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, 0, -1) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, -1, -1) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, 1, -1) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, 0, 1) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, -1, 1) ||
    //                v.BoxData.Position == c.BoxData.Position + new Vector3(0, 1, 1))
    //                v.Neighbour.Add(c);
    //        }

    //    }
    //}

    void addNeighbourNode()
    {
        for (int i = 0; i < listOfNode.Count; i++)
        {
            var v = listOfNode[i];

            for (int j = 0; j < listOfNode.Count; j++)
            {
                var c = listOfNode[j];
                if (v == c) continue;

                if (v.BoxData.Name.Contains("warp") && v.BoxData.Name.CompareTo(c.BoxData.Name) == 0)
                    v.Neighbour.Add(c);

                if (v.BoxData.Position == c.BoxData.Position + new Vector3(-0.25f, 0, 0) ||
                    v.BoxData.Position == c.BoxData.Position + new Vector3(-0.25f, -0.25f, 0) ||
                    v.BoxData.Position == c.BoxData.Position + new Vector3(-0.25f, 0.25f, 0) ||
                    v.BoxData.Position == c.BoxData.Position + new Vector3(0.25f, 0, 0) ||
                    v.BoxData.Position == c.BoxData.Position + new Vector3(0.25f, -0.25f, 0) ||
                    v.BoxData.Position == c.BoxData.Position + new Vector3(0.25f, 0.25f, 0) ||
                    v.BoxData.Position == c.BoxData.Position + new Vector3(0, 0, -0.25f) ||
                    v.BoxData.Position == c.BoxData.Position + new Vector3(0, -0.25f, -0.25f) ||
                    v.BoxData.Position == c.BoxData.Position + new Vector3(0, 0.25f, -0.25f) ||
                    v.BoxData.Position == c.BoxData.Position + new Vector3(0, 0, 0.25f) ||
                    v.BoxData.Position == c.BoxData.Position + new Vector3(0, -0.25f, 0.25f) ||
                    v.BoxData.Position == c.BoxData.Position + new Vector3(0, 0.25f, 0.25f))
                    v.Neighbour.Add(c);
            }
        }
    }
}