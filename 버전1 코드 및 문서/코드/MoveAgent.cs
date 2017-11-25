using UnityEngine;
using System.Collections;

public class MoveAgent : Singleton<MoveAgent> {

    Coroutine Inst = null;

    public void stop()
    {
        if (Inst == null) return;

        StopCoroutine(Inst);
    }

    public void MoveTo(GameObject gameObject, Vector3 endPos, float time)
    {
        Inst = StartCoroutine(intenalMoveTo(gameObject, endPos, time));

    }

    IEnumerator intenalMoveTo(GameObject gameObject, Vector3 endPos, float time)
    {
        float elapsedTime = 0;
        Vector3 startingPos = gameObject.transform.position;
        endPos = offset(endPos);

        while (elapsedTime < time)
        {
            gameObject.transform.position = Vector3.Lerp(startingPos, endPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    public IEnumerator turnRight(GameObject gameObject, float time)
    {
        iTween.RotateAdd(gameObject, iTween.Hash("y", 90, "time", time - 0.05f, "easetype", iTween.EaseType.linear));
        yield return null;
    }

    public IEnumerator turnLeft(GameObject gameObject, float time)
    {
        iTween.RotateAdd(gameObject, iTween.Hash("y", -90, "time", time - 0.05f, "easetype", iTween.EaseType.linear));
        yield return null;
    }

    Vector3 offset(Vector3 position)
    {
        return new Vector3(position.x, position.y + 0.23f , position.z);
    }
}
