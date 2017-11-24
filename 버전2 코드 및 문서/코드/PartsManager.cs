using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsManager : Singleton<PartsManager>
{ 
    public enum eType : int { head, body, left_arm, left_leg, right_arm, right_leg }

    Dictionary<string, GameObject> parts;
    Dictionary<string, Matrix4x4[]> matrices;
    Dictionary<string, Matrix4x4[]> recencyMatrices;
    Dictionary<string, Transform> bones;

    public PartsManager()
    {
        parts = new Dictionary<string, GameObject>()
        {            
            { "black_head",     Resources.Load<GameObject>("models/bot/black/black_head") },
            { "black_body",     Resources.Load<GameObject>("models/bot/black/black_body") },
            { "black_left_arm", Resources.Load<GameObject>("models/bot/black/black_left_arm") },
            { "black_left_leg", Resources.Load<GameObject>("models/bot/black/black_left_leg") },
            { "black_right_arm",Resources.Load<GameObject>("models/bot/black/black_right_arm") },
            { "black_right_leg",Resources.Load<GameObject>("models/bot/black/black_right_leg") },
            { "blue_head",      Resources.Load<GameObject>("models/bot/blue/blue_head") },
            { "blue_body",      Resources.Load<GameObject>("models/bot/blue/blue_body") },
            { "blue_right_arm", Resources.Load<GameObject>("models/bot/blue/blue_right_arm") },
            { "blue_left_arm",  Resources.Load<GameObject>("models/bot/blue/blue_left_arm") },
            { "blue_left_leg",  Resources.Load<GameObject>("models/bot/blue/blue_left_leg") },
            { "blue_right_leg", Resources.Load<GameObject>("models/bot/blue/blue_right_leg") },
            { "green_head",     Resources.Load<GameObject>("models/bot/green/green_head") },
            { "green_body",     Resources.Load<GameObject>("models/bot/green/green_body") },
            { "green_left_arm", Resources.Load<GameObject>("models/bot/green/green_left_arm") },
            { "green_left_leg", Resources.Load<GameObject>("models/bot/green/green_left_leg") },
            { "green_right_arm",Resources.Load<GameObject>("models/bot/green/green_right_arm") },
            { "green_right_leg",Resources.Load<GameObject>("models/bot/green/green_right_leg") },
            { "red_head",       Resources.Load<GameObject>("models/bot/red/red_head") },
            { "red_body",       Resources.Load<GameObject>("models/bot/red/red_body") },
            { "red_left_arm",   Resources.Load<GameObject>("models/bot/red/red_left_arm") },
            { "red_left_leg",   Resources.Load<GameObject>("models/bot/red/red_left_leg") } ,
            { "red_right_arm",  Resources.Load<GameObject>("models/bot/red/red_right_arm") },
            { "red_right_leg",  Resources.Load<GameObject>("models/bot/red/red_right_leg") }            
        };

        matrices = new Dictionary<string, Matrix4x4[]>();
        recencyMatrices = new Dictionary<string, Matrix4x4[]>();

        foreach (var part in parts)
        {   
            var key = part.Key;

            var mesh = part.Value.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            var value = new Matrix4x4[mesh.bindposes.Length];

            for (int i = 0; i < mesh.bindposes.Length; ++i)
            {
                var m = mesh.bindposes[i];

                var t = m.GetColumn(3);
                var r = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
                var s = Vector3.one;

                value[i] = Matrix4x4.TRS(t, r, s);
            }

            mesh.bindposes = value;
            matrices.Add(key, value);
            recencyMatrices.Add(key, value);
            
        }
    }

    public void SetBot(GameObject bot)
    {
        bones = getBones(bot.transform);
    }
    
    // 파츠 변경
    public void ChangePart(GameObject part, string name)
    {
        var renderer = part.GetComponent<SkinnedMeshRenderer>();
        //var bones = getBones(part.transform.parent);
        part.name = name; // 파츠의 메쉬마다 연결되있는 뼈대가 다름.

        linkBoneAndMesh(renderer, parts[name], bones);

        part.GetComponent<MeshCollider>().sharedMesh = renderer.sharedMesh;
    }

    // Scale변경
    public void ChangeScale(GameObject part, Vector3 scale)
    {
        // mesh scale 키우기
        var mesh = part.GetComponent<SkinnedMeshRenderer>().sharedMesh;        
        var mats = new Matrix4x4[matrices[part.name].Length];

        for (int i = 0; i < matrices[part.name].Length; ++i)
        {            
            var m = matrices[part.name][i];

            var t = m.GetColumn(3);
            var r = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));            

            mats[i] = Matrix4x4.TRS(t, r, scale * 0.42f); // 0.42f를 곱해주는 이유: 메쉬와 본의 크기가 다름.
        }

        mesh.bindposes = mats;

        // 로봇 스케일 키우기 MeshCollider의 크기를 키우기 위함. 뼈대도 따로 맞춰줘야함.
        part.transform.localScale = scale;
                
        matchJoint(part, scale);
                
        part.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // Color 변경
    public void ChangeColor(GameObject part, Color color_A, Color color_B, Color color_C)
    {
        var renderer = part.GetComponent<SkinnedMeshRenderer>();

        renderer.material.SetColor("", color_A);
        renderer.material.SetColor("", color_B);
        renderer.material.SetColor("", color_C);
    }

    Dictionary<string, Transform> getBones(Transform bot)
    {
        var root_bone = bot.Find("Bip001");

        Dictionary<string, Transform> bones = new Dictionary<string, Transform>();

        Stack<Transform> buffer = new Stack<Transform>();
        buffer.Push(root_bone);

        while (buffer.Count != 0)
        {
            var bone = buffer.Pop();

            bones.Add(bone.name, bone);

            for (int i = 0; i < bone.childCount; ++i)
                buffer.Push(bone.GetChild(i));
        }

        return bones;
    }

    void linkBoneAndMesh(SkinnedMeshRenderer renderer, GameObject prafab, Dictionary<string, Transform> bones)
    {
        var prefab_renderer = prafab.GetComponentInChildren<SkinnedMeshRenderer>();

        Transform rootBone;
        bones.TryGetValue(prefab_renderer.rootBone.name, out rootBone);
        renderer.rootBone = rootBone;

        Transform[] new_bones = new Transform[prefab_renderer.bones.Length];

        for (int i = 0; i < new_bones.Length; ++i)
        {
            bones.TryGetValue(prefab_renderer.bones[i].name, out new_bones[i]);
        }

        renderer.bones = new_bones;
        renderer.sharedMesh = prefab_renderer.sharedMesh;
        renderer.sharedMaterials = prefab_renderer.sharedMaterials;
    }

    void matchJoint(GameObject part, Vector3 scale)
    {        
        Matrix4x4[] mats = null;
        Mesh mesh = null;
        Vector3 pos;

        if (part.name.Contains("body"))
        {
            Transform child = null;
            var bot = part.transform.parent;

            for (int i = 0; i < bot.childCount; ++i)
            {
                child = bot.GetChild(i);
                var renderer = child.GetComponent<SkinnedMeshRenderer>();

                if (!renderer || child.name.Contains("body")) continue;

                mesh = renderer.sharedMesh;
                mats = new Matrix4x4[recencyMatrices[child.name].Length];

                for (int j = 0; j < recencyMatrices[child.name].Length; ++j)
                {
                    var rm = recencyMatrices[child.name][j];                                 

                    var t = rm.GetColumn(3);                    
                    var r = Quaternion.LookRotation(rm.GetColumn(2), rm.GetColumn(1));                    
                    var s = new Vector3(rm.GetColumn(0).magnitude, rm.GetColumn(1).magnitude, rm.GetColumn(2).magnitude);

                    t = new Vector4(t.x / scale.x, t.y / scale.y, t.z / scale.z, t.w);

                    mats[j] = Matrix4x4.TRS(t, r, s);
                }

                mesh.bindposes = mats;
                matrices[child.name] = mats;

                pos = child.transform.position;
                
                child.transform.position = new Vector3(pos.x / scale.x, pos.y / scale.y, pos.z / scale.z);
            }

            return;
        }

        mesh = part.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        mats = new Matrix4x4[mesh.bindposes.Length];

        for (int i = 0; i < mesh.bindposes.Length; ++i)
        {            
            var m = mesh.bindposes[i];
            var t = m.GetColumn(3);
            var r = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
            var s = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);

            t = new Vector4(scale.x * t.x, scale.y * t.y, scale.z * t.z, t.w);

            mats[i] = Matrix4x4.TRS(t, r, s);
        }

        mesh.bindposes = mats;
        recencyMatrices[part.name] = mats;        
    }    
}
