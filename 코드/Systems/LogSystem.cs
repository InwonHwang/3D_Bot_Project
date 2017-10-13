using UnityEngine;
using System.Collections;
using System.IO;
using SimpleJSON;

namespace PuzzleMaker
{
    public interface ILoggableSystem
    {        
        void Log(string message);
    }

    public interface ICommandLogSystem : ILoggableSystem
    {
        void Log(CommandModel command);
    }

    public class PuzzlePlayLogSystem : Singleton<PuzzlePlayLogSystem>, ILoggableSystem
    {
        public string FileName { get; set; }
        private string path;

        void Awake()
        {
            path = Application.dataPath + "/Resources/";           
        }

        public void Log(string message)
        {
            var stream = new FileStream(path + FileName, FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.Write);
            var writer = new StreamWriter(stream, System.Text.Encoding.Unicode);
            
            writer.WriteLine(message);

            writer.Close();
            stream.Close();
        }

        public void Log(CommandModel command)
        {
            var json = parse();            
            var key = command is SingleCommandModel ? "commands" : "command_groups";

            json[key].Add(command.ID);

            Log(json.ToString(4));
        }

        public void Clear()
        {
            var stream = new FileStream(path + FileName, FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.ReadWrite);
            stream.Flush();
            stream.Close();
        }

        private JSONNode parse()
        {
            var stream = new FileStream(path + FileName, FileMode.OpenOrCreate, FileAccess.Read);
            var reader = new StreamReader(stream, System.Text.Encoding.Unicode);
            var json = JSON.Parse(reader.ReadToEnd());

            if (json == null) {
                json = new JSONObject();
                json.Add("commands", new JSONArray());
                json.Add("command_groups", new JSONArray());
            }

            reader.Close();
            stream.Close();

            return json;
        }
    }
}

