using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace PuzzleMaker
{
    public interface IActionSystem
    {
        float Action(PlayableModel playable, PuzzleEntityModel entity_1);
        float ActionReverse(PlayableModel playable, PuzzleEntityModel entity_1);
    }

    public class PuzzleClearSystem : IActionSystem
    {
        public PuzzleClearSystem() : base() { }

        public float Action(PlayableModel playable, PuzzleEntityModel entity_1)
        {
            PlaySystem.Instance.Pause();            

            // 카메라 회전.
            // 로봇 클리어 애니메이션 재생.

            SceneManager.LoadScene("MissionClearScene");

            return 0.0f;
        }

        public float ActionReverse(PlayableModel playable, PuzzleEntityModel entity_1) { return 0.0f; }
    }

    public class WarpSystem : IActionSystem
    {
        public WarpSystem() : base() { }

        public float Action(PlayableModel character, PuzzleEntityModel warpable_tile)
        {
            if (!warpable_tile) return 0.0f;

            var warpable = warpable_tile.GetComponent<WarpableModel>();
            int random_index = Random.Range(0, warpable.Neighbour.Count);
            var destination_tile = warpable.Neighbour[random_index];

            var cleanable = destination_tile.GetComponent<CleanableModel>();

            if (cleanable && !cleanable.IsClean) return 0.0f;

            // Warp Animation 재생

            character.transform.position = destination_tile.transform.position;

            return character.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
        }

        public float ActionReverse(PlayableModel playable, PuzzleEntityModel entity_1) { return 0.0f; }
    }

    public class AttackSystem : IActionSystem
    {
        public AttackSystem() : base() { }

        public float Action(PlayableModel attacker, PuzzleEntityModel front_tile)
        {
            if (!front_tile) return 0.0f;

            var target = front_tile.GetComponent<TileModel>().PuzzleEntity;            

            var actionable = attacker.GetComponent<ActionableModel>();
            var damageable = target.GetComponent<DamageableModel>();

            if (!actionable || !damageable) return 0.0f;

            front_tile.GetComponent<TileModel>().PuzzleEntity = null;

            for (int i = 0; i < attacker.transform.childCount; ++i)
                attacker.transform.GetChild(i).GetComponent<Animator>().CrossFade("attack", 0.1f, -1, 0);

            damageable.HP -= actionable.Power;

            if (damageable.HP <= 0)
            {
                
            }

            float time = actionable.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;

            CoroutineUtility.Instance.DelayAction(time * 0.8f, () => {
                damageable.GetComponent<Animator>().CrossFade("damaged", 0.1f, -1, 0);
                CoroutineUtility.Instance.DelayAction(3f, () => {
                    damageable.gameObject.SetActive(false);
                });
            });            

            return actionable.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
        }

        public float ActionReverse(PlayableModel attacker, PuzzleEntityModel front_tile)            
        {
            return 0.0f;
        }
    }

    public class PurifySystem : IActionSystem
    {
        public PurifySystem() : base() { }

        public float Action(PlayableModel character, PuzzleEntityModel polluted_tile)
        {
            if (!polluted_tile) return 0.0f;

            var cleanable = polluted_tile.GetComponent<CleanableModel>();

            if (cleanable.IsClean) return 0.0f;

            for (int i = 0; i < character.transform.childCount; ++i) {
                character.transform.GetChild(i).GetComponent<Animator>().speed = 1;
                character.transform.GetChild(i).GetComponent<Animator>().CrossFade("spell", 0.0f, -1, 0);
            }

            cleanable.IsClean = true;

            CoroutineUtility.Instance.DelayAction(1.5f, () => {
                cleanable.Plague.SetActive(false);
            });

            return character.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
        }

        public float ActionReverse(PlayableModel character, PuzzleEntityModel polluted_tile)
        {
            if (!polluted_tile) return 0.0f;

            var cleanable = polluted_tile.GetComponent<CleanableModel>();

            if (cleanable.IsClean) return 0.0f;

            for (int i = 0; i < character.transform.childCount; ++i)
            {
                character.transform.GetChild(i).GetComponent<Animator>().speed = -1;
                character.transform.GetChild(i).GetComponent<Animator>().CrossFade("spell", 0.0f, -1, 0);
            }

            cleanable.IsClean = true;

            CoroutineUtility.Instance.DelayAction(1.5f, () => {
                cleanable.Plague.SetActive(false);
            });

            return character.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
        }
    }

    public class MoveSystem : IActionSystem
    {
        public MoveSystem() : base() { }

        public float Action(PlayableModel entity, PuzzleEntityModel front_tile)
        {
            if (!front_tile) return 0.0f;

            var cleanable = front_tile.GetComponent<CleanableModel>();

            if (cleanable && !cleanable.IsClean) return 0.0f;

            entity.GetComponent<Animator>().SetFloat("Speed", 1);
            entity.GetComponent<Animator>().Play("walk", -1, 0);
            for (int i = 0; i < entity.transform.childCount; ++i) {
                entity.transform.GetChild(i).GetComponent<Animator>().SetFloat("Speed", 1);
                entity.transform.GetChild(i).GetComponent<Animator>().CrossFade("walk", 0.1f, -1, 0);
            }

            return entity.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
        }

        public float ActionReverse(PlayableModel entity, PuzzleEntityModel behind_tile)
        {
            
            entity.GetComponent<Animator>().SetFloat("Speed", -1);
            entity.GetComponent<Animator>().Play("walk", -1, 1);
            for (int i = 0; i < entity.transform.childCount; ++i) {
                entity.transform.GetChild(i).GetComponent<Animator>().SetFloat("Speed", -1);
                entity.transform.GetChild(i).GetComponent<Animator>().CrossFade("walk", 0.1f, -1, 1);
            }

            return entity.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
        }
    }

    public class JumpUpSystem : IActionSystem
    {
        public JumpUpSystem() : base() { }

        public float Action(PlayableModel entity, PuzzleEntityModel front_up_tile)
        {
            if (!front_up_tile) return 0.0f;

            var cleanable = front_up_tile.GetComponent<CleanableModel>();

            if (cleanable && !cleanable.IsClean) return 0.0f;

            entity.GetComponent<Animator>().SetFloat("Speed", 1);
            entity.GetComponent<Animator>().Play("jump_up", -1, 0);
            for (int i = 0; i < entity.transform.childCount; ++i) {
                entity.transform.GetChild(i).GetComponent<Animator>().SetFloat("Speed", 1);
                entity.transform.GetChild(i).GetComponent<Animator>().CrossFade("jump_up", 0.1f, -1, 0);
            }

            return 2.0f;
        }

        public float ActionReverse(PlayableModel entity, PuzzleEntityModel behind_down_tile)
        {
            entity.GetComponent<Animator>().SetFloat("Speed", -1);
            entity.GetComponent<Animator>().Play("jump_up", -1, 1);
            for (int i = 0; i < entity.transform.childCount; ++i) {
                entity.transform.GetChild(i).GetComponent<Animator>().SetFloat("Speed", -1);
                entity.transform.GetChild(i).GetComponent<Animator>().CrossFade("jump_up", 0.1f, -1, 1);
            }

            return 2.0f;
        }
    }

    public class JumpDownSystem : IActionSystem
    {
        public float Action(PlayableModel entity, PuzzleEntityModel front_down_tile)
        {
            if (!front_down_tile) return 0.0f;

            var cleanable = front_down_tile.GetComponent<CleanableModel>();

            if (cleanable && !cleanable.IsClean) return 0.0f;

            entity.GetComponent<Animator>().SetFloat("Speed", 1);
            entity.GetComponent<Animator>().Play("jump_down", -1, 0);
            for (int i = 0; i < entity.transform.childCount; ++i) {
                entity.transform.GetChild(i).GetComponent<Animator>().SetFloat("Speed", 1);
                entity.transform.GetChild(i).GetComponent<Animator>().CrossFade("jump_down", 0.1f, -1, 0);
            }

            return 2.0f;
        }

        public float ActionReverse(PlayableModel entity, PuzzleEntityModel behind_up_tile)
        {
            entity.GetComponent<Animator>().SetFloat("Speed", -1);
            entity.GetComponent<Animator>().Play("jump_down", -1, 1);
            for (int i = 0; i < entity.transform.childCount; ++i) {
                entity.transform.GetChild(i).GetComponent<Animator>().SetFloat("Speed", 1);
                entity.transform.GetChild(i).GetComponent<Animator>().CrossFade("jump_down", 0.1f, -1, 1);
            }

            return 2.0f;
        }
    }

    public class TurnLeftSystem : IActionSystem
    {
        public TurnLeftSystem() : base() { }

        public float Action(PlayableModel entity, PuzzleEntityModel dummy)
        {
            entity.GetComponent<Animator>().SetFloat("Speed", 1);
            entity.GetComponent<Animator>().Play("turn_left", -1, 0);

            for (int i = 0; i < entity.transform.childCount; ++i) {
                entity.transform.GetChild(i).GetComponent<Animator>().SetFloat("Speed", 1);
                entity.transform.GetChild(i).GetComponent<Animator>().CrossFade("turn_left", 0.3f, -1, 0);
            }

            return entity.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
        }

        public float ActionReverse(PlayableModel entity, PuzzleEntityModel dummy)
        {
            entity.GetComponent<Animator>().SetFloat("Speed", -1);
            entity.GetComponent<Animator>().Play("turn_left", -1, 1);

            for (int i = 0; i < entity.transform.childCount; ++i) {
                entity.transform.GetChild(i).GetComponent<Animator>().SetFloat("Speed", 1);
                entity.transform.GetChild(i).GetComponent<Animator>().CrossFade("turn_left", 0.3f, -1, 1);
            }

            return entity.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
        }
    }

    public class TurnRightSystem : IActionSystem
    {
        public TurnRightSystem() : base() { }

        public float Action(PlayableModel entity, PuzzleEntityModel dummy)
        {
            entity.GetComponent<Animator>().SetFloat("Speed", 1);
            entity.GetComponent<Animator>().Play("turn_right", -1, 0);

            for (int i = 0; i < entity.transform.childCount; ++i)
            {
                entity.transform.GetChild(i).GetComponent<Animator>().SetFloat("Speed", 1);
                entity.transform.GetChild(i).GetComponent<Animator>().CrossFade("turn_right", 0.3f, -1, 0);
            }

            return entity.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
        }

        public float ActionReverse(PlayableModel entity, PuzzleEntityModel dummy)
        {
            entity.GetComponent<Animator>().SetFloat("Speed", -1);
            entity.GetComponent<Animator>().Play("turn_right", -1, 1);

            for (int i = 0; i < entity.transform.childCount; ++i) {                
                entity.transform.GetChild(i).GetComponent<Animator>().SetFloat("Speed", -1);
                entity.transform.GetChild(i).GetComponent<Animator>().CrossFade("turn_right", 0.3f, -1, 1);
            }

            return entity.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
        }
    }
}