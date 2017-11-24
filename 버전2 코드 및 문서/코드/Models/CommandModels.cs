using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace PuzzleMaker
{
    public interface IJsonable
    {
        JSONNode ToJson();
        void FromJson(JSONNode json_node);
    }

    public abstract class CommandModel : MonoBehaviour
    {        
        public PlayableModel Playable { get; set; }
        public int ID { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }
        public int Condition { get; set; }

        public void Init(int condition, PlayableModel playable)
        {
            Condition = condition;
            Playable = playable;
        }
    }

    public class SingleCommandModel : CommandModel, IJsonable
    {
        public JSONNode ToJson()
        {
            var command_info = new JSONObject();

            command_info["id"] = ID;
            command_info["name"] = gameObject.name;
            command_info["playable_entity_id"] = Playable.GetComponent<PuzzleEntityModel>().ID;
            command_info["condition"] = Condition;

            return command_info;
        }

        public void FromJson(JSONNode json_node)
        {
            var command_info = json_node.AsObject;

            ID = command_info["id"].AsInt;
            Condition = command_info["condition"].AsInt;
            gameObject.name = command_info["name"].Value;

            var name = gameObject.name.Split('_');
            Major = name[0];
            Minor = name.Length > 1 ? name[1] : null;
        }
    }    

    public class CommandGroupModel : CommandModel, IJsonable
    {
        public SingleCommandModel ConditionalCommand { get; set; }
        public List<CommandModel> CommandsSequence { get; private set; }

        void Awake()
        {
            CommandsSequence = new List<CommandModel>();            
        }

        public void Init(int condition, PlayableModel playable, SingleCommandModel conditional_command = null)
        {
            base.Init(condition, playable);            
            ConditionalCommand = conditional_command;
        }

        public JSONNode ToJson()
        {
            var command_group_info = new JSONObject();
            var command_ids = new JSONArray();

            command_ids.Add(ConditionalCommand ? ConditionalCommand.ID : -1);

            foreach(var command in CommandsSequence) {
                if (command is SingleCommandModel)
                    command_ids.Add(command.ID);
                else
                    command_ids.Add(command.ID * -1 - 1);
            }

            command_group_info["id"] = ID;
            command_group_info["name"] = gameObject.name;            
            command_group_info["playable_entity_id"] = Playable.GetComponent<PuzzleEntityModel>().ID;
            command_group_info["command_ids"] = command_ids;
            command_group_info["condition"] = Condition;

            return command_group_info;
        }

        public void FromJson(JSONNode json_node)
        {
            var command_group_info = json_node.AsObject;

            ID = command_group_info["id"].AsInt;
            Condition = command_group_info["condition"].AsInt;
            gameObject.name = command_group_info["name"].Value;

            //int r = (Condition >> 24) & 255;
            //int g = (Condition >> 16) & 255;
            //int b = (Condition >> 8) & 255;
            //int a = Condition & 255;
            //GetComponent<Renderer>().material.color = new Color(r, g, b, a);

            var name = gameObject.name.Split('.');
            Major = name[0];
            Minor = name.Length > 1 ? name[1] : null;

            if (Major == "function" && Minor == "main") Playable.Main = this;
            else Playable.CommandGroups.Add(this);
        }
    }
}