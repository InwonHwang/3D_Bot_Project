using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AnimationAgent : Singleton<AnimationAgent>
{
    Dictionary<string, Animator> animators;

    public Dictionary<string, Animator> Animators { get { return animators; } } //임시

    #region public method
    private AnimationAgent()
    {
        animators = new Dictionary<string, Animator>();
    }

    public void registerAnimator(string key, Animator animator)
    {
        internalRegisterAnimation(key, animator);
    }

    public void unregisterAnimator(string key)
    {
        internalUnregisterAnimation(key);
    }

    public void setBool(string key, string name, bool value)
    {
        internalSetBool(key, name, value);
    }

    public void setInteger(string key, string name, int value)
    {        
        internalSetInteger(key, name, value);
    }

   
    public void setFloat(string key, string name, float value)
    {
        internalSetFloat(key, name, value);
    }

    public void setBlendParam(string key, string name, float value, float time)
    {
        StartCoroutine(internalSetBlendParam(key, name, value, time));
    }
    #endregion public method


    #region private method
    void internalRegisterAnimation(string key, Animator animator)
    {
        if (key == null || animator == null) return;

        if (animators.ContainsKey(key))
        {
            Debug.Log("already exist: " + key);
            return;
        }

        animators.Add(key, animator);
    }

    void internalUnregisterAnimation(string key)
    {
        if (key == null) return;

        if (!animators.ContainsKey(key))
        {
            Debug.Log("doesn't exist");
            return;
        }

        animators.Remove(key);
    }

    void internalSetBool(string key, string name, bool value)
    {
        if (key == null) return;

        if (!animators.ContainsKey(key))
        {
            Debug.Log("doesn't exist: " + key);
            return;
        }

        animators[key].SetBool(name, value);
    }

    void internalSetInteger(string key, string name, int value)
    {
        if (key == null) return;

        if (!animators.ContainsKey(key))
        {
            Debug.Log("doesn't exist: " + key);
            return;
        }

        animators[key].SetInteger(name, value);
    }

    void internalSetFloat(string key, string name, float value)
    {
        if (key == null) return;

        if (!animators.ContainsKey(key))
        {
            Debug.Log("doesn't exist: " + key);
            return;
        }

        animators[key].SetFloat(name, value);
    }

    IEnumerator internalSetBlendParam(string key, string name, float value, float time)
    {
        if (!animators.ContainsKey(key))
            yield break;

        float lerpValue = 0;
        while (Mathf.Abs(value - lerpValue) > 0.01f)
        {
            lerpValue = Mathf.Lerp(animators[key].GetFloat(name), value, Time.deltaTime / time);
            animators[key].SetFloat(name, lerpValue);

            yield return null;
        }
    }



    #endregion private method

}
