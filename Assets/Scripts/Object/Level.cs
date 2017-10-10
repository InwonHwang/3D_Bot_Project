using UnityEngine;
using System.Collections;
using JsonFx.Json;
using System.Collections.Generic;

using Robot.Singleton;

public class Level : MonoBehaviour
{
    readonly string[] namesOfObjects = { "Box", "BoringMachine", "Desert_Cliff", "Desert_Grass01", "Desert_Grass02", "Desert_L_Pipe01",
                                         "Desert_L_Pipe02", "Desert_Main", "Desert_Monsterleg01", "Desert_Monsterleg02", "Desert_N_Pipe",
                                         "Desert_Oasis", "Desert_Pollution_L_Pipe01", "Desert_Pollution_L_Pipe02", "Desert_Pollution_N_Pipe",
                                         "Desert_Pollution_Oasis", "Desert_Stone01", "Desert_Stone02", "Desert_Stone03", "Desert_Stone04",
                                         "Desert_Stone05", "Desert_Tong01", "Desert_Tong02", "Desert_Tree01", "Desert_Tree02", "Desert_Star"};

    EntityType.MapData2 mapData;
    Graph graph;

    internal GraphNode StartNode { get; private set; }
    internal int Count { get; set; }
    internal Vector3 Left { get; private set; }
    internal Vector3 Right { get; private set; }
    internal Vector3 Up { get; private set; }
    internal Vector3 Down { get; private set; }
    internal Vector3 Size { get; private set; }

    internal void setLevel(string fileName)
    {
        Count = 0;
        graph.clear();
        setMapData(fileName);
        activateObjectsByMapData();
        for (int i = 0; i < mapData.ListOfBoxData.Count; i++)
        {
            if (mapData.ListOfBoxData[i].Traversable == false) continue;

            var newNode = new GraphNode();
            newNode.BoxData = mapData.ListOfBoxData[i];

            if (mapData.ListOfBoxData[i].Name.Contains("pollution"))
            {
                mapData.ListOfBoxData[i].Traversable = false;

                Count++;
            }

            graph.add(newNode);

            if (newNode.BoxData.Start) StartNode = newNode;
        }

        graph.buildGraph();

        var objectParent = GameObjectAgent.Instance.findChild("Level", "ToBeActivated/Objects");

        for(int  i = 0; i < objectParent.transform.childCount; i++)
        {
            var obj = objectParent.transform.GetChild(i);

            if (obj.name.Contains("BoringMachine"))
            {
                Count++;
                obj.position += new Vector3(0, 0.3f, 0);
                obj.rotation = Quaternion.Euler(0, obj.eulerAngles.y, 0);
                for (int j = 0; j < mapData.ListOfBoxData.Count; j++)
                {
                    if (mapData.ListOfBoxData[j].Position.x == obj.position.x &&
                        mapData.ListOfBoxData[j].Position.z == obj.position.z)
                        mapData.ListOfBoxData[j].Traversable = false;
                }
            }
        }




        GameObjectAgent.Instance.getComponent<Robot.GUI.Ingame>("UI", "3_Ingame").countOfBase = mapData.LimitedCount[0];
        GameObjectAgent.Instance.getComponent<Robot.GUI.Ingame>("UI", "3_Ingame").countOfG1 = mapData.LimitedCount[1];
        GameObjectAgent.Instance.getComponent<Robot.GUI.Ingame>("UI", "3_Ingame").countOfG2 = mapData.LimitedCount[2];

        var orthographicCam = GameObjectAgent.Instance.findChild("Camera", "Orthographic Camera");
        orthographicCam.GetComponent<Camera>().orthographicSize = mapData.CameraSize;
        orthographicCam.transform.position = mapData.CameraPos + new Vector3(0, -0.2f, 0);

        var buttonX = GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Panel/PanelX");
        var mainP = GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Base/MainX").transform;
        var g1P = GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Base/G1X").transform;
        var g2P = GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Base/G2X").transform;

        for (int i = mainP.transform.childCount - 1; i > -1; i--)
        {
            var child = mainP.transform.GetChild(i);
            child.SetParent(GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Panel/PanelX").transform);
            child.transform.position = new Vector2(0, -1200);
        }

        for (int i = g1P.transform.childCount - 1; i > -1; i--)
        {
            var child = g1P.transform.GetChild(i);
            child.SetParent(buttonX.transform);
            child.transform.position = new Vector2(0, -1200);
        }

        for (int i = g2P.transform.childCount - 1; i > -1; i--)
        {
            var child = g2P.transform.GetChild(i);
            child.SetParent(buttonX.transform);
            child.transform.position = new Vector2(0, -1200);
        }

        for (int i = 0; i < 16 - mapData.LimitedCount[0]; i++)
            buttonX.transform.GetChild(0).SetParent(mainP);

        for (int i = 0; i < 8 - mapData.LimitedCount[1]; i++)
            buttonX.transform.GetChild(0).SetParent(g1P);

        for (int i = 0; i < 8 - mapData.LimitedCount[2]; i++)
            buttonX.transform.GetChild(0).SetParent(g2P);
    }

    //internal void setLevel(string fileName)
    //{
    //    var boringmachine = GameObjectAgent.Instance.findChild("Object", "Boringmachine");
    //    GameObjectAgent.Instance.setActive(boringmachine, true);
    //    boringmachine.GetComponent<Boringmachine>().reset();
    //    Count = 0;
    //    graph.clear();
    //    setMapData(fileName);
    //    activateObjectsByMapData();
    //    for (int i = 0; i < mapData.ListOfBoxData.Count; i++)
    //    {
    //        if (mapData.ListOfBoxData[i].Traversable == false) continue;

    //        var newNode = new GraphNode();
    //        newNode.BoxData = mapData.ListOfBoxData[i];

    //        if (mapData.ListOfBoxData[i].Name.Contains("greentile_side"))
    //        {
    //            mapData.ListOfBoxData[i].Traversable = false;
    //            boringmachine.transform.GetChild(Count).position = mapData.ListOfBoxData[i].Position + new Vector3(0, 1.5f, 0);
    //            Count++;
    //        }
    //        if (mapData.ListOfBoxData[i].Name.CompareTo("box_grass_top2_04") == 0)
    //        {
    //            mapData.ListOfBoxData[i].Traversable = false;
    //            Count++;
    //        }

    //        graph.add(newNode);

    //        if (newNode.BoxData.Start) StartNode = newNode;
    //    }
    //    graph.buildGraph();
    //    findCenter();
    //}

    void Awake()
    {

        graph = new Graph();
        createObjects();
    }

    void createObjects()
    {
        var prefabs = ResourcesManager.Instance.getValues<GameObject>(ResourcesManager.Instance.prefabs, "object");

        for (int i = 0; i < prefabs.Length; i++)
        {
            var prefab = prefabs[i];
            var parentOfObject = findParent(prefab.name);
            for (int j = 0; j < 200; j++)
            {
                var obj = Instantiate<GameObject>(prefab);

                obj.transform.position = new Vector3(0, 1000, 0);
                obj.name = prefab.name;
                obj.transform.SetParent(parentOfObject.transform);
                obj.SetActive(false);
            }

        }
    }

    void setMapData(string fileName)
    {
        if (fileName == null) return;

        string data = StreamAgent.Instance.readFile("Level", fileName);
        mapData = Converter.Deserialize<EntityType.MapData2>(data);
    }

    void activateObjectsByMapData()
    {
        clear();

        var parent = GameObjectAgent.Instance.findChild("Level", "Objects/Box");
        var newParent = GameObjectAgent.Instance.findChild("Level", "ToBeActivated/Boxes");
        for (int i = 0; i < mapData.ListOfBoxData.Count; i++)
        {
            if (parent.transform.childCount == 0) break;

            var box = parent.transform.GetChild(0);
            box.gameObject.SetActive(true);
            box.position = mapData.ListOfBoxData[i].Position;
            box.rotation = mapData.ListOfBoxData[i].Quaternion;
            box.GetComponent<Renderer>().material.mainTexture = ResourcesManager.Instance.sprites[mapData.ListOfBoxData[i].Name].texture;
            box.SetParent(newParent.transform);
        }

        var toBeActivated = GameObjectAgent.Instance.findChild("Level", "ToBeActivated");
        newParent = GameObjectAgent.Instance.findChild("Level", "ToBeActivated/Objects");
        for (int i = 0; i < mapData.ListOfObjectData.Count; i++)
        {
            parent = findParent(mapData.ListOfObjectData[i].Name);

            if (parent.transform.childCount == 0) continue;

            var obj = parent.transform.GetChild(0);
            obj.gameObject.SetActive(true);            
            obj.position = mapData.ListOfObjectData[i].Position;
            obj.rotation = mapData.ListOfObjectData[i].Quaternion;
            obj.SetParent(newParent.transform);
        }
    }

    internal void clear()
    {
        var parentOfBox = GameObjectAgent.Instance.findChild("Level", "ToBeActivated/Boxes");
        var parentOfObject = GameObjectAgent.Instance.findChild("Level", "ToBeActivated/Objects");
        var newParentOfBox = GameObjectAgent.Instance.findChild("Level", "Objects/Box");

        for (int i = parentOfBox.transform.childCount - 1; i > -1; i--)
        {
            var box = parentOfBox.transform.GetChild(i);
            box.SetParent(newParentOfBox.transform);
            box.gameObject.SetActive(false);
        }

        var newParentOfObject = parentOfObject;
        for (int i = parentOfObject.transform.childCount - 1; i > -1; i--)
        {
            var obj = parentOfObject.transform.GetChild(i);

            newParentOfObject = findParent(obj.name);
            if (newParentOfObject) obj.SetParent(newParentOfObject.transform);
            obj.gameObject.SetActive(false);
        }
    }

    GameObject findParent(string name)
    {
        GameObject newParentOfObject = null;
        for (int j = 0; j < namesOfObjects.Length; j++)
        {
            if (!name.Contains(namesOfObjects[j]))
                continue;

            newParentOfObject = GameObjectAgent.Instance.findChild("Level", "Objects/" + namesOfObjects[j]);
        }

        return newParentOfObject;
    }

    void findCenter()
    {
        Left = new Vector3(1000, 0, 1000);
        Right = new Vector3(-1000, 0, -1000);
        Up = new Vector3(1000, 0, -1000);
        Down = new Vector3(-1000, 0, 1000);

        float minX = 1000;
        float minY = 1000;
        float minZ = 1000;
        float maxX = 0;
        float maxY = 0;
        float maxZ = 0;

        for (int i = 0; i < mapData.ListOfBoxData.Count; i++)
        {
            if (mapData.ListOfBoxData[i].Position.x + mapData.ListOfBoxData[i].Position.z < Left.x + Left.z)
                Left = mapData.ListOfBoxData[i].Position;

            if (mapData.ListOfBoxData[i].Position.x + mapData.ListOfBoxData[i].Position.z > Right.x + Right.z)
                Right = mapData.ListOfBoxData[i].Position;

            if (mapData.ListOfBoxData[i].Position.z - mapData.ListOfBoxData[i].Position.x < Down.z - Down.x)
                Down = mapData.ListOfBoxData[i].Position;

            if (mapData.ListOfBoxData[i].Position.z - mapData.ListOfBoxData[i].Position.x > Up.z - Up.x)
                Up = mapData.ListOfBoxData[i].Position;

            if (mapData.ListOfBoxData[i].Position.x < minX) minX = mapData.ListOfBoxData[i].Position.x;
            if (mapData.ListOfBoxData[i].Position.y < minY) minY = mapData.ListOfBoxData[i].Position.y;
            if (mapData.ListOfBoxData[i].Position.z < minZ) minZ = mapData.ListOfBoxData[i].Position.z;

            if (mapData.ListOfBoxData[i].Position.x > maxX) maxX = mapData.ListOfBoxData[i].Position.x;
            if (mapData.ListOfBoxData[i].Position.y > maxY) maxY = mapData.ListOfBoxData[i].Position.y;
            if (mapData.ListOfBoxData[i].Position.z > maxX) maxZ = mapData.ListOfBoxData[i].Position.z;
        }

        if (Up.z - Up.y < 0 || Down.z - Up.y > 0) Up = (Left + Right) / 2;
        
        //Size = new Vector3(sizeX, sizeY, sizeZ);
        //Size = new Vector3(down.x - up.x, sizeY, up.z - down.z);

    }
}
