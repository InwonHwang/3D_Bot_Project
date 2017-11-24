using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;
using Robot.Singleton;
using Robot.GUI;


// 없어도 될 듯
// 스크립트 수정 하기(스크립트 이름 함수 등등) PartManager?
public class HighlightAgent : Singleton<HighlightAgent> 
{
    Dictionary<string, Highlighter> highlighers = new Dictionary<string, Highlighter>();

    #region public method

    public void constantOffImmediateAll()
    {
        internalConstantOffImmediateAll();
    }

    public void constantOnImmediate(string key, Color color)
    {
        internalConstantOnImmediate(key, color);
    }

    public void constantOffImmediate(string key)
    {
        internalConstantOffImmediate(key);
    }

    public void registerHighlighter(string key, Highlighter highlighter)
    {
        internalRegisterHighlighter(key, highlighter);
    }

    public void unregisterHighlighter(string key)
    {
        internalUnregisterHighlighter(key);
    }

    #endregion

    #region private method

    void internalRegisterHighlighter(string key, Highlighter highlighter)
    {
        if (key == null || highlighter == null) return;

        if (highlighers.ContainsKey(key))
        {
            Debug.Log("already exist: " + key);
            return;
        }

        highlighers.Add(key, highlighter);        
    }

    void internalUnregisterHighlighter(string key)
    {
        if (key == null) return;

        if (!highlighers.ContainsKey(key))
        {
            Debug.Log("doesn't exist");
            return;
        }

        highlighers.Remove(key);
    }

    void internalConstantOnImmediate(string key, Color color)
    {
        if (key == null) return;

        if (!highlighers.ContainsKey(key))
        {
            Debug.Log("doesn't exist");
            return;
        }

        highlighers[key].ConstantOnImmediate(color);
    }

    void internalConstantOffImmediate(string key)
    {
        if (key == null) return;

        if (!highlighers.ContainsKey(key))
        {
            Debug.Log("doesn't exist");
            return;
        }

        highlighers[key].ConstantOffImmediate();
    }

    void internalConstantOffImmediateAll()
    {
        var values = highlighers.Values;

        foreach(var value in values)        
            value.ConstantOffImmediate();        
    }    

    #endregion
}