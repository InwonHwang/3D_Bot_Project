using UnityEngine;
using System.Collections;

public class Utility : Singleton<Utility> {

    public void delayAction(float time, UnityEngine.Events.UnityAction action)
    {
        StartCoroutine(internalDelayAction(time, action));
    }

    IEnumerator internalDelayAction(float time, UnityEngine.Events.UnityAction action)
    {
        if (action == null) yield break;

        yield return new WaitForSeconds(time);

        action();
    }
    
}
