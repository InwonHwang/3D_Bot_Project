using UnityEngine;
using System.Collections.Generic;

namespace EntityType
{
    public class StickerInfo
    {
        public string ParentName { get; set; }
        public string BoneName { get; set; }
        public string GParentName { get; set; }
        public string Name { get; set; }
        public List<Vector3> BufVertices { get; set; }
        public List<Vector3> BufNormals { get; set; }
        public List<int> BufIndices { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public StickerInfo() { }
        public StickerInfo(GameObject sticker, Mesh mesh, Sprite sprite)
        {

            BufVertices = new List<Vector3>();
            BufNormals = new List<Vector3>();
            BufIndices = new List<int>();
            set(sticker, mesh, sprite);
        }

        public void set(GameObject sticker, Mesh mesh, Sprite sprite)
        {
            BufVertices.Clear();
            BufNormals.Clear();
            BufIndices.Clear();
            ParentName = "";
            GParentName = "";
            BoneName = "";

            List<string> parentName = new List<string>();

            var parent = sticker.transform.parent;
            while (parent && parent.name.Contains("bot") == false)
            {
                parentName.Insert(0, parent.name);
                parent = parent.parent;
            }

            for (int i = 0; i < parentName.Count; i++)
                BoneName += parentName[i] + "/";

            ParentName = parent.name;
            GParentName = parent.parent.name;       

            Name = sprite.name;
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                BufVertices.Add(mesh.vertices[i]);
                BufNormals.Add(mesh.normals[i]);
            }

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                for (int j = 0; j < mesh.GetIndices(i).Length; j++)                
                    BufIndices.Add(mesh.GetIndices(i)[j]);                
            }

            Position = sticker.transform.position;
            Rotation = sticker.transform.rotation;
        }
    } // class StrickerInfo

    public class Part
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Rotation { get; set; }

        public Part() { }
        public Part(string name, Color color, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            Name = name;
            Color = color;
            Position = position;
            Scale = scale;
            Rotation = rotation;
        }

    }

    public class BotInfo
    {      
       
        public Part[] Parts { get; private set; }
        public List<StickerInfo> StickerMesh { get; private set; }

        public BotInfo() { }
        public BotInfo(GameObject bot)
        {
            set(bot);
        }

        public void set(GameObject bot)
        { 
            StickerMesh = new List<StickerInfo>();
            Parts = new Part[6];
            StickerMesh.Clear();            
            int index = 0;
            var activePart = bot.GetComponent<PartsManager>().ActiveParts;
            for (int i = 0; i < activePart.Length; i++)
            {
                var part = activePart[i];

                if (part == null || part.gameObject.activeSelf == false) continue;

                for (int j = 0; j < Constants.stickerParent.Length; j++)
                {
                    var stickerParent = part.transform.FindChild(Constants.stickerParent[j]);
                    for (int k = 0; k < stickerParent.childCount; k++)
                    {
                        var sticker = stickerParent.GetChild(k);
                        if (sticker.name.Contains("Sticker") == false) continue;

                        StickerMesh.Add(sticker.GetComponent<DecalSystem.Decal>().Info);
                    }
                }

                var color = part.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color;
                Parts[index] = new Part(part.transform.parent.name + "/" + part.transform.name, color, part.transform.localPosition, part.transform.lossyScale, part.transform.localRotation);
                index++;
            }
        }
    } // class BotInfo

    //ingame
    public class MapData
    {
        public List<BoxData> ListOfBoxData { get; set; }
        public List<ObjectData> ListOfObjectData { get; set; }

        public MapData()
        {
            ListOfBoxData = new List<BoxData>();
            ListOfObjectData = new List<ObjectData>();
        }

    } // MapData Class

    public class MapData2
    {
        public List<BoxData> ListOfBoxData { get; set; }
        public List<ObjectData> ListOfObjectData { get; set; }
        public int[] LimitedCount { get; set; }
        public Vector3 CameraPos { get; set; }
        public float CameraSize { get; set; }

        public MapData2()
        {
            CameraSize = 0;
            CameraPos = Vector3.zero;
            LimitedCount = new int[3];
            ListOfBoxData = new List<BoxData>();
            ListOfObjectData = new List<ObjectData>();
        }

    } // MapData Class

    public class ObjectData
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Quaternion { get; set; }

        public ObjectData() { }
        public ObjectData(Transform obj) { set(obj); }

        public void set(Transform obj)
        {
            Name = obj.name;
            Position = obj.position;
            Quaternion = obj.rotation;
        }
    } // ObjectData Class

    public class BoxData
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Quaternion { get; set; }
        public bool Start { get; set; }
        public bool Traversable { get; set; }
        public bool Contaminated { get; set; }
        public int? WarpId { get; set; }

        public BoxData() { }
    } // BoxData Class

} // namespcae EntityType