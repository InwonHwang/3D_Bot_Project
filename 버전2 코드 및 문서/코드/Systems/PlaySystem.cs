using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SimpleJSON;

namespace PuzzleMaker
{
    public class PlaySystem : Singleton<PlaySystem>
    {
        private Dictionary<string, ICommandSystem> commandSystems;
        private Dictionary<string, ICommandGroupSystem> commandGroupSystems;
        private Dictionary<PlayableModel, List<CommandModel>> commandQueues;
        private Dictionary<PlayableModel, Coroutine> coroutines;

        void Awake()
        {
            commandQueues = new Dictionary<PlayableModel, List<CommandModel>>();
            coroutines = new Dictionary<PlayableModel, Coroutine>();

            commandSystems = new Dictionary<string, ICommandSystem>();
            commandSystems.Add("action", new ActionCommandSystem());
            commandSystems.Add("move", new MoveCommandSystem());
            commandSystems.Add("jump", new JumpCommandSystem());
            commandSystems.Add("turn", new TurnCommandSystem());

            commandGroupSystems = new Dictionary<string, ICommandGroupSystem>();
            commandGroupSystems.Add("if", new IfGroupSystem());
            commandGroupSystems.Add("while", new WhileGroupSystem());
            commandGroupSystems.Add("do_while", new DoWhileGroupSystem());
            commandGroupSystems.Add("for", new ForGroupSystem());
            commandGroupSystems.Add("function", new FunctionSystem());

            UpdateCommandQueue();
        }

        public void UpdateCommandQueue()
        {
            commandQueues.Clear();
            coroutines.Clear();

            var playables = GameObject.FindObjectsOfType<PlayableModel>();

            foreach (var playable in playables)
            {
                var new_command_list = new List<CommandModel>();
                if(playable.Main) new_command_list.Add(playable.Main);
                commandQueues.Add(playable, new_command_list);                
                coroutines.Add(playable, null);
            }
        }

        public void Play(PlayableModel playable = null)
        {
            if(playable) {
                coroutines[playable] = StartCoroutine(play(playable));
                return;
            }

            StartCoroutine(play());            
        }

        public void PlayOneStep(PlayableModel playable = null)
        {
            if (playable) {
                coroutines[playable] = StartCoroutine(playOneStep(playable));
                return;
            }
            
            foreach(var key in commandQueues.Keys) {
                coroutines[key] = StartCoroutine(playOneStep(key));
            }
        }

        public void PlayBackOneStep(PlayableModel playable = null)
        {
            if (playable)
            {
                coroutines[playable] = StartCoroutine(playBackOneStep(playable));
                return;
            }

            StartCoroutine(playBackOneStep());
        }

        public void Pause(PlayableModel playable = null)
        {
            if (playable)
            {
                if(coroutines[playable] != null) StopCoroutine(coroutines[playable]);
                return;
            }

            foreach (var pair in coroutines)
            {                
                var coroutine = pair.Value;
                if (coroutine != null) StopCoroutine(coroutine);
                
                roundTransformValueToInt(pair.Key);
            }
        }

        IEnumerator play()
        {
            while (true)
            {
                bool is_playable = false;
                foreach (var command_queue in commandQueues.Values)
                {
                    if (!command_queue.Any()) continue;

                    is_playable = true;
                    break;
                }

                if (!is_playable) yield break;

                foreach (var playable in commandQueues.Keys)
                    coroutines[playable] = StartCoroutine(playOneStep(playable));

                foreach (var playable in commandQueues.Keys)
                    yield return coroutines[playable];
            }
        }

        IEnumerator play(PlayableModel playable)
        {
            while(commandQueues[playable].Any())
                yield return StartCoroutine(playOneStep(playable));            
        }

        IEnumerator playOneStep(PlayableModel playable)
        {
            var command = getCommandFromLogFile(playable, "reverted.json", "executed.json");
            if (command) {
                yield return new WaitForSeconds(commandSystems[command.Major].Execute(command));
                yield break;
            }

            while (commandQueues[playable].Any() && commandQueues[playable][0] is CommandGroupModel) {
                processCommand(playable);
            }

            if (!commandQueues[playable].Any()) yield break;

            yield return new WaitForSeconds(processCommand(playable) + 0.3f);

            roundTransformValueToInt(playable);            
        }

        IEnumerator playBackOneStep()
        {
            foreach (var playable in commandQueues.Keys)
                coroutines[playable] = StartCoroutine(playBackOneStep(playable));

            foreach (var playable in commandQueues.Keys)
                yield return coroutines[playable];
            
        }

        IEnumerator playBackOneStep(PlayableModel playable)
        {
            var command = getCommandFromLogFile(playable, "executed.json", "reverted.json");
            
            if (!command) yield break;

            var major = command.Major;

            yield return new WaitForSeconds(commandSystems[major].Revert(command));

            roundTransformValueToInt(playable);
        }

        float processCommand(PlayableModel playable)
        {
            var command = commandQueues[playable][0];
            commandQueues[playable].RemoveAt(0);            

            PuzzlePlayLogSystem.Instance.FileName = "executed.json";
            PuzzlePlayLogSystem.Instance.Log(command);

            if (command is SingleCommandModel)
            {
                if (commandSystems[command.Major].IsExecutable(command)) {
                    //Debug.Log(command.Major + " command was executed!");
                    return commandSystems[command.Major].Execute(command);
                }

                //Debug.Log(command.Major + "command is not executable!");
                return 0.0f;
            }
                        
            var commands = commandGroupSystems[command.Major].GetCommands(command);

            if (commands == null)// {
                // Debug.Log(command.Major + " group's commands sequence is empty!");
                return 0.0f;
            //}

            commandQueues[playable].InsertRange(0, commands);
            // Debug.Log(command.Major + " group's commands sequence is inserted!");            

            return 0.0f;
        }        

        private CommandModel getCommandFromLogFile(PlayableModel playable, string file_name_to_parse, string file_name_to_save)
        {
            var stream = new FileStream(Application.dataPath + "/Resources/" + file_name_to_parse, FileMode.OpenOrCreate, FileAccess.Read);
            var reader = new StreamReader(stream, System.Text.Encoding.Unicode);
            var json = JSON.Parse(reader.ReadToEnd());

            reader.Close();
            stream.Close();

            if (json == null) return null;

            var command_ids = json["commands"].AsArray;            
            var result = from command in GameObject.FindObjectsOfType<SingleCommandModel>()
                         where command.Playable.GetComponent<PuzzleEntityModel>().ID == playable.GetComponent<PuzzleEntityModel>().ID
                         select command;

            var single_commands = result.ToDictionary(x => x.GetComponent<SingleCommandModel>().ID);
            int count = command_ids.Count - 1;

            while (count > -1 && !single_commands.ContainsKey(command_ids[count].AsInt))
                count--;

            if (count == -1) return null;

            var return_value = single_commands[command_ids[count].AsInt];
            command_ids.Remove(command_ids[count]);

            PuzzlePlayLogSystem.Instance.FileName = file_name_to_parse;
            PuzzlePlayLogSystem.Instance.Log(json.ToString(4));

            PuzzlePlayLogSystem.Instance.FileName = file_name_to_save;
            PuzzlePlayLogSystem.Instance.Log(return_value);

            return return_value;
        }

        private void roundTransformValueToInt(PlayableModel playable)
        {
            var position = playable.transform.position;
            var rotation = playable.transform.eulerAngles;
            playable.transform.position = new Vector3(Mathf.RoundToInt(position.x),
                                                            Mathf.RoundToInt(position.y),
                                                            Mathf.RoundToInt(position.z));
            playable.transform.rotation = Quaternion.Euler(Mathf.RoundToInt(rotation.x),
                                                                 Mathf.RoundToInt(rotation.y),
                                                                 Mathf.RoundToInt(rotation.z));
        }
    }
}
