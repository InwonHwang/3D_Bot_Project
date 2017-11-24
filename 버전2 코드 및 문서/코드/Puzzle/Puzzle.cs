using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using SimpleJSON;

namespace PuzzleMaker
{
    public struct Point
    {
        public int x;
        public int y;
        public int z;
    
        public Point(int x, int y, int z) { this.x = x; this.y = y; this.z = z; }
        public Point(int cell_no) { x = (cell_no & 1023); y = (cell_no >> 10) & 1023; z = cell_no >> 20; }
        public int ToCellNo() { return (z << 20) | (y << 10) | x; }
    }

    public class Puzzle
    {
        public enum Direction : int { Current, FrontUp, Front, FrontDown, BehindUp, Behind, BehindDown, Unknown }
                
        private Dictionary<int, PuzzleEntityModel> tiles;

        public Puzzle() {            
            tiles = new Dictionary<int, PuzzleEntityModel>();
        }

        public string Serialize()
        {
            JSONObject puzzle_info = new JSONObject();
            JSONArray puzzle_entity_infos = new JSONArray();
            JSONArray command_infos = new JSONArray();
            JSONArray commnad_group_infos = new JSONArray();

            var command_objects = GameObject.FindObjectsOfType<CommandModel>();

            var puzzle_entitys = from puzzle_entity in GameObject.FindObjectsOfType<PuzzleEntityModel>()
                                 where puzzle_entity.GetComponent<PuzzleEntityModel>()
                                 select puzzle_entity.GetComponent<PuzzleEntityModel>();

            var commands = from command_object in command_objects
                           where command_object.GetComponent<SingleCommandModel>()
                           select command_object.GetComponent<SingleCommandModel>();
            var command_groups = from command_object in command_objects

                                 where command_object.GetComponent<CommandGroupModel>()
                                 select command_object.GetComponent<CommandGroupModel>();

            int id = 0;
            foreach (var puzzle_entity in puzzle_entitys)
            {
                puzzle_entity.ID = id++;
                puzzle_entity_infos.Add(puzzle_entity.ToJson());
            }            

            id = 0;
            foreach (var command in commands) {                
                command.ID = id++;
                command_infos.Add(command.ToJson());
            }

            id = 0;
            foreach (var commnad_group in command_groups) {
                commnad_group.ID = id++;                
            }

            foreach (var commnad_group in command_groups) {
                commnad_group_infos.Add(commnad_group.ToJson());
            }

            puzzle_info["puzzle_entities"] = puzzle_entity_infos;
            puzzle_info["commands"] = command_infos;
            puzzle_info["command_groups"] = commnad_group_infos;

            return puzzle_info.ToString(4);
        }

        public void Deserialize(string json)
        {
            var puzzle_entities = new List<PuzzleEntityModel>();
                       
            var puzzle_info = JSONString.Parse(json);

            var puzzle_entity_infos = puzzle_info["puzzle_entities"].AsArray;
            var command_infos = puzzle_info["commands"].AsArray;
            var command_group_infos = puzzle_info["command_groups"].AsArray;

            foreach (JSONObject puzzle_entity_info in puzzle_entity_infos)
            {                
                var new_puzzle_entity = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/PuzzleEntities/" + puzzle_entity_info["name"].Value));
                new_puzzle_entity.AddComponent<PuzzleEntityModel>().FromJson(puzzle_entity_info);

                puzzle_entities.Add(new_puzzle_entity.GetComponent<PuzzleEntityModel>());
            }

            List<CommandModel> single_commands = new List<CommandModel>();
            foreach (JSONObject command_info in command_infos)
            {
                var new_command = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Commands/" + command_info["name"].Value));
                
                var single_command = new_command.AddComponent<SingleCommandModel>();
                single_command.Playable = puzzle_entities[command_info["playable_entity_id"].AsInt].GetComponent<PlayableModel>();
                single_command.FromJson(command_info);

                single_commands.Add(single_command);
            }

            List<CommandModel> command_groups = new List<CommandModel>();
            foreach (JSONObject command_group_info in command_group_infos)
            {
                var new_command_group = new GameObject();
                var command_group = new_command_group.AddComponent<CommandGroupModel>();
                int playable_id = command_group_info["playable_entity_id"].AsInt;
                int conditional_command_id = command_group_info["command_ids"][0].AsInt;

                command_group.Playable = puzzle_entities[playable_id].GetComponent<PlayableModel>();                
                command_group.ConditionalCommand = conditional_command_id == -1 ?
                                                   null :
                                                   single_commands[conditional_command_id] as SingleCommandModel;

                command_group.FromJson(command_group_info);
                command_groups.Add(command_group);
            }

            for (int i = 0; i < command_groups.Count; ++i)
            {                
                var command_ids = command_group_infos[i]["command_ids"].AsArray;
                
                for (int j = 1; j < command_ids.Count; ++j)
                {
                    int id = command_ids[j].AsInt;

                    var commands = id < 0 ? command_groups : single_commands;

                    id = id < 0 ? (id + 1) * -1 : id;

                    var command_group = command_groups[i] as CommandGroupModel;
                    command_group.CommandsSequence.Add(commands[id]);                    
                }
            }

            var tile_entities = from puzzle_entity in puzzle_entities
                                where puzzle_entity.GetComponent<TileModel>()
                                select puzzle_entity;

            tiles.Clear();
            foreach (var tile_entity in tile_entities) {
                tiles.Add(convertVector3ToCellNo(tile_entity.transform.position), tile_entity.GetComponent<PuzzleEntityModel>());

                var upper_entity = from entity in puzzle_entities
                                   where !entity.GetComponent<TileModel>() &&
                                          entity.transform.position == tile_entity.transform.position + new Vector3(0, 1, 0)
                                   select entity;

                if (upper_entity.Count() == 0) continue;
                    
                tile_entity.GetComponent<TileModel>().PuzzleEntity = upper_entity.ToArray<PuzzleEntityModel>()[0];                
            }
        }        

        public PuzzleEntityModel FindNeighbourTile(PlayableModel playable, Direction direction)
        {
            var transform = playable.transform;            
            var position = transform.position + new Vector3(0, -1, 0);

            switch (direction)
            {
                case Direction.Current:
                    return getTile(position);

                case Direction.FrontUp:
                    return getTile(position + transform.forward + transform.up);

                case Direction.Front:
                    {
                        var front_up = getTile(position + transform.forward + transform.up);
                        if (transform.position.x == 1 && front_up) Debug.Log(position + transform.forward + transform.up);
                        return front_up == null ? getTile(position + transform.forward) : null;
                    }                

                case Direction.FrontDown:
                    {
                        var forward = getTile(position + transform.forward);
                        var front_up = getTile(position + transform.forward + transform.up);

                        return (forward == null && front_up == null) ? getTile(position + transform.forward - transform.up) : null;
                    }

                case Direction.BehindUp:
                    return getTile(position - transform.forward + transform.up);

                case Direction.Behind:
                    {
                        var behind_up = getTile(position - transform.forward + transform.up);
                        return behind_up == null ? getTile(position - transform.forward) : null;
                    }                

                case Direction.BehindDown:
                    {
                        var behind = getTile(position - transform.forward);
                        var behind_up = getTile(position - transform.forward + transform.up);

                        return (behind == null && behind_up == null) ? getTile(position - transform.forward - transform.up) : null;
                    }

                default:
                    return null;
            }
        }        

        private PuzzleEntityModel getTile(Vector3 position)
        {
            int cell_no = convertVector3ToCellNo(position);

            if (!tiles.ContainsKey(cell_no)) return null;

            return tiles[cell_no];
        }

        private int convertVector3ToCellNo(Vector3 position)
        {
            var point = new Point(Mathf.RoundToInt(position.x),
                                  Mathf.RoundToInt(position.y),
                                  Mathf.RoundToInt(position.z));

            return point.ToCellNo();
        }
    
        // test용
        public void WriteData()
        {
            var json = Serialize();           
          
            FileStream fs = new FileStream(Application.dataPath + "/Resources/test.json", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.Unicode);
            writer.WriteLine(json);
            writer.Close();            
        }
        
    }
}