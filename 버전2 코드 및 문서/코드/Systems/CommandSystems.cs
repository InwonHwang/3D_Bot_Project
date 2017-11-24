using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PuzzleMaker
{
    public interface IConditionable
    {
        bool IsExecutable(CommandModel command);
    }

    public interface IExecutable
    {
        float Execute(CommandModel command);
        //float ExecuteImmediately(CommandModel command);
    }

    public interface IRevertable
    {
        //bool IsRevertable(CommandModel command)
        float Revert(CommandModel command);
        void RevertImmediately(CommandModel command);
    }

    public interface IConditionalExecutable : IConditionable, IExecutable
    {
    }

    public interface ICommandSystem : IConditionalExecutable, IRevertable
    {
    }

    public interface ICommandGroupSystem
    {
        IEnumerable<CommandModel> GetCommands(CommandModel command_group);
    }

    public abstract class PuzzleSystem
    {
        protected PuzzleManager puzzleManager;

        public PuzzleSystem()
        {
            puzzleManager = PuzzleManager.Instance;
        }
    }

    public abstract class SingleCommandSystem : PuzzleSystem
    {
        protected Dictionary<string, IActionSystem> actionSystems;
        public SingleCommandSystem() : base()
        {
            actionSystems = new Dictionary<string, IActionSystem>();
        }

        protected bool isExecutable(SingleCommandModel command)
        {
            int current_tile_condition = puzzleManager.ActivePuzzle.FindNeighbourTile(command.Playable, Puzzle.Direction.Current).Condition;

            return (command.Condition == -1 || command.Condition == current_tile_condition);
        }
    }

    public class ActionCommandSystem : SingleCommandSystem, ICommandSystem
    {
        public ActionCommandSystem() : base()
        {
            actionSystems.Add("warp", new WarpSystem());
            actionSystems.Add("attack", new AttackSystem());
            actionSystems.Add("purify", new PurifySystem());
        }

        public bool IsExecutable(CommandModel command)
        {
            return isExecutable(command as SingleCommandModel);
        }

        public float Execute(CommandModel command)
        {
            var playable = command.Playable;

            var current_tile = puzzleManager.ActivePuzzle.FindNeighbourTile(playable, Puzzle.Direction.Current);
            var warpable = current_tile.GetComponent<WarpableModel>();

            if (warpable) {
                if (warpable.Neighbour.Count == 0)

                    return actionSystems["puzzle_clear"].Action(playable, null);

                return actionSystems["warp"].Action(playable, current_tile);
            }

            var front_tile = puzzleManager.ActivePuzzle.FindNeighbourTile(playable, Puzzle.Direction.Front);

            if (!front_tile) return 0.0f;

            var tile_model = front_tile.GetComponent<TileModel>();

            if (tile_model.PuzzleEntity)
                return actionSystems["attack"].Action(playable, front_tile);

            var cleanable = front_tile.GetComponent<CleanableModel>();

            if (cleanable && !cleanable.IsClean)
                return actionSystems["purify"].Action(playable, front_tile);


            return 0.0f;
        }

        public float Revert(CommandModel command) { return 0.0f; }
        public void RevertImmediately(CommandModel command) { }
    }

    public class MoveCommandSystem : SingleCommandSystem, ICommandSystem
    {
        public MoveCommandSystem() : base()
        {
            actionSystems.Add("move", new MoveSystem());
        }

        public bool IsExecutable(CommandModel command)
        {
            return isExecutable(command as SingleCommandModel);
        }

        public float Execute(CommandModel command)
        {
            var front_tile = puzzleManager.ActivePuzzle.FindNeighbourTile(command.Playable, Puzzle.Direction.Front);

            return actionSystems["move"].Action(command.Playable, front_tile);
        }

        public float Revert(CommandModel command)
        {
            return actionSystems["move"].ActionReverse(command.Playable, null);
        }

        public void RevertImmediately(CommandModel command) { }
    }

    public class JumpCommandSystem : SingleCommandSystem, ICommandSystem
    {
        public JumpCommandSystem() : base()
        {
            actionSystems.Add("jump_up", new JumpUpSystem());
            actionSystems.Add("jump_down", new JumpDownSystem());
        }

        public bool IsExecutable(CommandModel command)
        {
            return isExecutable(command as SingleCommandModel);
        }

        public float Execute(CommandModel command)
        {
            var playable = command.Playable;
            var direction = command.Minor == "up" ? Puzzle.Direction.FrontUp : Puzzle.Direction.FrontDown;

            return actionSystems[command.name].Action(playable, puzzleManager.ActivePuzzle.FindNeighbourTile(playable, direction));
        }

        public float Revert(CommandModel command)
        {
            var playable = command.Playable;

            return actionSystems[command.name].ActionReverse(playable, null);
        }
        public void RevertImmediately(CommandModel command) { }
    }

    public class TurnCommandSystem : SingleCommandSystem, ICommandSystem
    {
        public TurnCommandSystem() : base()
        {
            actionSystems.Add("turn_left", new TurnLeftSystem());
            actionSystems.Add("turn_right", new TurnRightSystem());
        }

        public bool IsExecutable(CommandModel command)
        {
            return isExecutable(command as SingleCommandModel);
        }

        public float Execute(CommandModel command)
        {
            var playable = command.Playable;

            return actionSystems[command.name].Action(playable, null);
        }

        public float Revert(CommandModel command)
        {
            var playable = command.Playable;

            return actionSystems[command.name].ActionReverse(playable, null);
        }
        public void RevertImmediately(CommandModel command) { }
    }

    public class IfGroupSystem : PuzzleSystem, ICommandGroupSystem
    {
        public IfGroupSystem() : base() { }

        public IEnumerable<CommandModel> GetCommands(CommandModel command)
        {
            var command_group = command as CommandGroupModel;
            var current_tile_condition = puzzleManager.ActivePuzzle.FindNeighbourTile(command_group.Playable, Puzzle.Direction.Current).Condition;

            var commands_sequence = from command_model in command_group.CommandsSequence
                                    where command_model.Condition == current_tile_condition
                                    select command_model;

            if (commands_sequence.Any())
                return commands_sequence;

            return from command_model in command_group.CommandsSequence
                   where command_model.Condition == -1
                   select command_model;
        }
    }    

    public class WhileGroupSystem : PuzzleSystem, ICommandGroupSystem
    {
        public WhileGroupSystem() : base() { }

        public IEnumerable<CommandModel> GetCommands(CommandModel command)
        {
            var command_group = command as CommandGroupModel;            

            var current_tile_condition = puzzleManager.ActivePuzzle.FindNeighbourTile(command_group.Playable, Puzzle.Direction.Current).Condition;

            if (command_group.ConditionalCommand.Condition != -1 &&
                command_group.ConditionalCommand.Condition != current_tile_condition)
                return null;

            IEnumerable<CommandModel> commands_sequence = command_group.CommandsSequence;            
            
            commands_sequence = commands_sequence.Concat(new CommandModel[] { command_group });

            return commands_sequence;
        }
    }

    public class DoWhileGroupSystem : PuzzleSystem, ICommandGroupSystem
    {
        public DoWhileGroupSystem() : base() {}

        public IEnumerable<CommandModel> GetCommands(CommandModel command)
        {
            var command_group = command as CommandGroupModel;            
            
            var current_tile_condition = puzzleManager.ActivePuzzle.FindNeighbourTile(command_group.Playable, Puzzle.Direction.Current).Condition;
            
            if (command_group.Condition == 0 &&
                command_group.ConditionalCommand.Condition != -1 &&
                command_group.ConditionalCommand.Condition != current_tile_condition)
                return null;

            if (command_group.Condition == 1) command_group.Condition--;
            
            IEnumerable<CommandModel> commands_sequence = command_group.CommandsSequence;

            commands_sequence = commands_sequence.Concat(new CommandModel[] { command_group });

            return commands_sequence;
        }
    }

    public class ForGroupSystem : PuzzleSystem, ICommandGroupSystem
    {
        public ForGroupSystem() : base() { }

        public IEnumerable<CommandModel> GetCommands(CommandModel command)
        {
            var command_group = command as CommandGroupModel;

            if (command_group.Condition-- < 1) return null;            

            IEnumerable<CommandModel> commands_sequence = command_group.CommandsSequence;

            commands_sequence = commands_sequence.Concat(new CommandModel[] { command_group });

            return commands_sequence;
        }      
    }

    public class FunctionSystem : PuzzleSystem, ICommandGroupSystem
    {
        public FunctionSystem() : base() { }

        public IEnumerable<CommandModel> GetCommands(CommandModel command)
        {
            return (command as CommandGroupModel).CommandsSequence;
        }
    }
}