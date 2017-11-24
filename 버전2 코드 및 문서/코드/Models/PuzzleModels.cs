using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace PuzzleMaker
{
    public class PuzzleEntityModel : MonoBehaviour, IJsonable
    {
        public int ID { get; set; }
        public int Condition {
            get {
                var color = GetComponent<Renderer>().material.color;
                return ((int)color.r << 24) | ((int)color.g << 16) | ((int)color.b << 8) | (int)color.a;
            }
            set {

            }
        }

        public JSONNode ToJson()
        {
            var puzzle_info = new JSONObject();
            var model_infos = new JSONArray();
                        
            var position = transform.position;
            var rotation = transform.eulerAngles;
            var components = gameObject.GetComponents<MonoBehaviour>();

            foreach (var component in components) {
                if (component is PuzzleEntityModel) continue;

                model_infos.Add(component.GetType().FullName);
            }

            puzzle_info["id"] = ID;
            puzzle_info["name"] = gameObject.name;            
            puzzle_info["cell_number"] = new Point((int)position.x, (int)position.y, (int)position.z).ToCellNo();
            puzzle_info["rotation"] = new Point((int)rotation.x, (int)rotation.y, (int)rotation.z).ToCellNo();            
            puzzle_info["models"] = model_infos;            

            return puzzle_info;
        }

        public void FromJson(JSONNode json_node)
        {
            var puzzle_info = json_node.AsObject;

            var model_infos = json_node["models"].AsArray;
            var position = new Point(puzzle_info["cell_number"].AsInt);
            var rotation = new Point(puzzle_info["rotation"].AsInt);


            ID = json_node["id"].AsInt;
            gameObject.name = json_node["name"].Value;
            gameObject.transform.position = new Vector3(position.x, position.y, position.z);
            gameObject.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);

            foreach (JSONNode model_info in model_infos) {
                gameObject.AddComponent(System.Type.GetType(model_info.Value));
            }
        }
    }

    public class PlayableModel : MonoBehaviour
    {        
        public CommandGroupModel Main { get; set; }
        public List<CommandGroupModel> CommandGroups { get; set; }        

        void Awake() {
            CommandGroups = new List<CommandGroupModel>();
        }
    }

    public class TileModel : MonoBehaviour
    {
        public PuzzleEntityModel PuzzleEntity { get; set; }

        void Awake()
        {
            // 임시용
            if (GetComponent<TileModel>())
                transform.localScale = new Vector3(4, 4, 4);
        }
    }

    public class WarpableModel : MonoBehaviour
    {        
        public List<WarpableModel> Neighbour { get; set; }

        void Awake() {
            Neighbour = new List<WarpableModel>();
        }
    }

    public class CleanableModel: MonoBehaviour
    {
        public bool IsClean { get; set; }
        public GameObject Plague { get; set; }

        void Awake()
        {
            var plague = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/PuzzleEntities/plague"));
            plague.name = "plague";
            plague.transform.position = transform.position + new Vector3(0f, 1f, 0f);
            plague.transform.localScale = plague.transform.localScale * 2.5f;
            Plague = plague;
            IsClean = false;
        }
    }

    public class DamageableModel : MonoBehaviour
    {
        public int HP { get; set; }
    }

    public class ActionableModel : MonoBehaviour
    {        
        public int Power { get; set; }
    }
}