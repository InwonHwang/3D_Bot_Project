using UnityEngine;
using System.Collections;

namespace PuzzleMaker
{
    public class CoroutineUtility : Singleton<CoroutineUtility>
    {
        public Coroutine StartCoroutineWithoutMonoBehaviour(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        public void StopCoroutineWithoutMonoBehaviour(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }

        public void DelayAction(float time, UnityEngine.Events.UnityAction action)
        {
            StartCoroutine(delayAction(time, action));
        }

        IEnumerator delayAction(float time, UnityEngine.Events.UnityAction action)
        {
            yield return new WaitForSeconds(time);

            action();
        }

    }
}

