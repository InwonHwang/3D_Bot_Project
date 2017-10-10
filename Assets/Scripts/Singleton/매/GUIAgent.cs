using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace Robot.Singleton
{
    public class GUIAgent : Singleton<GUIAgent>
    {
        Dictionary<string, GameObject> guiObjects = new Dictionary<string, GameObject>();

        public GameObject SetActionButton(GameObject actionButton, string buttonName, string buttonText, Sprite img, GameObject parentObject)
        {
            //생성을 할까말까 구현쪽에서 할까;
            GameObject temp = Instantiate(actionButton);

            temp.name = buttonName;
            temp.transform.FindChild("Text").GetComponent<UnityEngine.UI.Text>().text = buttonText;
            temp.GetComponent<UnityEngine.UI.Image>().sprite = img;

            temp.transform.SetParent(parentObject.transform);
            temp.GetComponent<RectTransform>().localScale = Vector3.one;

            return temp;
        }

        #region public method

        public Dictionary<string, GameObject> GuiObjects { get { return guiObjects; } }

        public void registerObject(string parent, string path)
        {
            internalRegisterObject(parent, path);
        }

        public void registerObject(string name)
        {
            internalRegisterObject(name);
        }

        public void unregisterObject(string parent, string path)
        {
            internalUnregisterObject(parent, path);
        }

        public void unregisterObject(string name)
        {
            internalUnregisterObject(name);
        }  
              
        public void setSprite(string parent, string path, Sprite sprite)
        {
            var key = parent + "/" + path;

            internalSetSprite(key, sprite);
        }

        public void setSprite(string key, Sprite sprite)
        {
            internalSetSprite(key, sprite);
        }

        public void setText(string parent, string path, string text)
        {
            var key = parent + "/" + path;

            GameObjectAgent.Instance.addComponent<Text>(guiObjects[key]).text = text;
        }

        public void setText(string key, string text)
        {
            GameObjectAgent.Instance.addComponent<Text>(guiObjects[key]).text = text;
        }

        public void setEnabled(string parent, string path, bool value)
        {
            var key = parent + "/" + path;

            internalSetEnabled(key, value);
        }

        public void setEnabled(string key, bool value)
        {
            internalSetEnabled(key, value);
        }

        public void delaySetEnabled(string parent, string path, bool value, float time = 0)
        {
            var key = parent + "/" + path;

            internalDelaySetEnabled(key, value, time);
        }

        public void delaySetEnabled(string key, bool value, float time = 0)
        {
            internalDelaySetEnabled(key, value, time);
        }

        public void setButtonSpriteState(string parent, string path, Sprite highlighted, Sprite pressed, Sprite disabled)
        {
            var key = parent + "/" + path;

            internalSetSpriteState(key, highlighted, pressed, disabled);
        }

        public void setButtonSpriteState(string key, Sprite highlighted, Sprite pressed, Sprite disabled)
        {
            internalSetSpriteState(key, highlighted, pressed, disabled);
        }

        public void addListener(string parent, string path, UnityAction call)
        {
            var key = parent + "/" + path;
            internalAddListener(key, call);
        }

        public void addListener(string key, UnityAction call)
        {
            internalAddListener(key, call);
        }

        public void setRaycastTarget(string parent, string path, bool value)
        {
            var key = parent + "/" + path;

            internalSetRaycastTarget(key, value);
        }

        public void setRaycastTarget(string name, bool value)
        {
            internalSetRaycastTarget(name, value);
        }

        public GameObject createSub(GameObject prefab, Vector2 anchoredPosition, Sprite sprite, string name, GameObject parent, bool active)
        {
            return internalCreateSub(prefab, anchoredPosition, sprite, name, parent, active);
        }

        ///////////////////////////////////////////////////////////////////////////////////

        //    public void setSprite(GameObject guiObject, Sprite sprite)
        //    {
        //        internalSetSprite(guiObject, sprite);
        //    }

        //    public void setText(GameObject guiObject, string text)
        //    {
        //        internalSetText(guiObject, text);
        //    }

        //    public void setEnabled(GameObject guiObject, bool value, float time = 0)
        //    {
        //        StartCoroutine(internalDelaySetEnabled(guiObject, value, time));
        //    }

        //    public void setButtonSpriteState(GameObject guiObject, Sprite highlighted, Sprite pressed, Sprite disabled)
        //    {
        //        internalSetSpriteState(guiObject, highlighted, pressed, disabled);
        //    }

        //    public void addListener(GameObject guiObject, UnityAction call)
        //    {
        //        internalAddListener(guiObject, call);
        //    }

        //    public GameObject createSub(GameObject prefab, Vector2 anchoredPosition, Sprite sprite, string name, GameObject parent, bool active)
        //    {
        //        return internalCreateSub(prefab, anchoredPosition, sprite, name, parent, active);
        //}

        //    public void setRaycastTarget(GameObject guiObject, bool value)
        //    {
        //        internalSetRaycastTarget(guiObject, value);
        //    }

        #endregion public mathod

        #region private method

        void internalRegisterObject(string parent, string path)
        {
            if (parent == null || path == null) return;

            string key = parent + "/" + path;

            var guiObject = GameObjectAgent.Instance.findChild(parent, path);

            if (guiObjects.ContainsKey(key))
            {
                Debug.Log(key + " already exists");
                return;
            }

            guiObjects.Add(key, guiObject);
        }

        void internalRegisterObject(string name)
        {
            if (name == null) return;
                        
            var guiObject = GameObject.Find(name);

            if (guiObjects.ContainsKey(name))
            {
                Debug.Log("already exists");
                return;
            }

            guiObjects.Add(name, guiObject);
        }

        void internalUnregisterObject(string parent, string path)
        {
            if (parent == null || path == null) return;

            string key = parent + "/" + path;

            if (!guiObjects.ContainsKey(key))
            {
                Debug.Log("does not exists");
                return;
            }

            guiObjects.Remove(key);
        }

        void internalUnregisterObject(string name)
        {
            if (name == null) return;

            if (!guiObjects.ContainsKey(name))
            {
                Debug.Log("does not exists");
                return;
            }

            guiObjects.Remove(name);
        }

        void internalSetSprite(string key, Sprite sprite)
        {
            if (!guiObjects.ContainsKey(key)) return;
            GameObjectAgent.Instance.addComponent<Image>(guiObjects[key]).sprite = sprite;
        }

        void internalSetText(string key, string text)
        {
            if (!guiObjects.ContainsKey(key)) return;

            GameObjectAgent.Instance.addComponent<Text>(guiObjects[key]).text = text;
        }

        void internalSetEnabled(string key, bool value)
        {
            if (!guiObjects.ContainsKey(key)) return;

            if (guiObjects[key].GetComponent<Canvas>())
                guiObjects[key].GetComponent<Canvas>().enabled = value;
        }

        IEnumerator internalDelaySetEnabled(string key, bool value, float time = 0)
        {
            yield return new WaitForSeconds(time);

            if (!guiObjects.ContainsKey(key)) yield break;

            if (guiObjects[key].GetComponent<Canvas>())
                guiObjects[key].GetComponent<Canvas>().enabled = value;
        }

        void internalSetSpriteState(string key, Sprite highlighted, Sprite pressed, Sprite disabled)
        {
            if (!guiObjects.ContainsKey(key) || !GameObjectAgent.Instance.getComponent<Button>(guiObjects[key])) return;            

            var spriteState = new SpriteState();
            spriteState.highlightedSprite = highlighted;
            spriteState.pressedSprite = pressed;
            spriteState.disabledSprite = disabled;

            if (guiObjects[key].GetComponent<Button>())
                guiObjects[key].GetComponent<Button>().spriteState = spriteState;
        }

        void internalAddListener(string key, UnityAction call)
        {
            
            if (!guiObjects.ContainsKey(key) || !GameObjectAgent.Instance.getComponent<Button>(guiObjects[key])) return;
            //Debug.Log(key);
            guiObjects[key].GetComponent<Button>().onClick.AddListener(call);
        }

        GameObject internalCreateSub(GameObject prefab, Vector2 anchoredPosition, Sprite sprite, string name, GameObject parent, bool active)
        {
            if (!parent) return null;

            var sub = Instantiate<GameObject>(prefab);
            if (sub)
            {
                sub.transform.SetParent(parent.transform, false);
                GameObjectAgent.Instance.addComponent<Image>(sub.name).sprite = sprite;
                GameObjectAgent.Instance.getComponent<Image>(sub.name).SetNativeSize();
                GameObjectAgent.Instance.addComponent<RectTransform>(sub.name).anchoredPosition = anchoredPosition;
                GameObjectAgent.Instance.setActive(sub, active);
                sub.transform.name = name;
            }

            return sub;
        }

        void internalSetRaycastTarget(string key, bool value)
        {
            if (!guiObjects.ContainsKey(key) || !GameObjectAgent.Instance.getComponent<Button>(guiObjects[key])) return;

            guiObjects[key].GetComponent<Image>().raycastTarget = value;
        }


        //void internalRegisterObject(string parent, string path)
        //{
        //    if (parent == null || path == null) return;

        //    string key = parent + "/" + path;
        //    var guiObject = GameObjectAgent.Instance.findChild(parent, path);

        //    if (guiObjects.ContainsKey(key))
        //    {
        //        Debug.Log("already exists");
        //        return;
        //    }

        //    guiObjects.Add(key, guiObject);
        //}

        //void internalUnregisterObject(string parent, string path)
        //{
        //    if (parent == null || path == null) return;

        //    string key = parent + "/" + path;            

        //    if (!guiObjects.ContainsKey(key))
        //    {
        //        Debug.Log("does not exists");
        //        return;
        //    }

        //    guiObjects.Remove(key);
        //}

        //void internalSetSprite(GameObject guiObject, Sprite sprite)
        //{
        //    GameObjectAgent.Instance.addComponent<Image>(guiObject).sprite = sprite;
        //}

        //void internalSetText(GameObject guiObject, string text)
        //{
        //    GameObjectAgent.Instance.addComponent<Text>(guiObject).text = text;
        //}        

        //IEnumerator internalDelaySetEnabled(GameObject guiObject, bool value, float time = 0)
        //{
        //    yield return new WaitForSeconds(time);

        //    if (guiObject.GetComponent<Canvas>())
        //        guiObject.GetComponent<Canvas>().enabled = value;
        //}

        //void internalSetSpriteState(GameObject guiObject, Sprite highlighted, Sprite pressed, Sprite disabled)
        //{
        //    if (!guiObject || !GameObjectAgent.Instance.getComponent<Button>(guiObject)) return;

        //    var spriteState = new SpriteState();
        //    spriteState.highlightedSprite = highlighted;
        //    spriteState.pressedSprite = pressed;
        //    spriteState.disabledSprite = disabled;

        //    if (guiObject.GetComponent<Button>())
        //        guiObject.GetComponent<Button>().spriteState = spriteState;
        //}

        //void internalAddListener(GameObject guiObject, UnityAction call)
        //{
        //    if (!guiObject || !GameObjectAgent.Instance.getComponent<Button>(guiObject)) return;

        //    guiObject.GetComponent<Button>().onClick.AddListener(call);
        //}

        //GameObject internalCreateSub(GameObject prefab, Vector2 anchoredPosition, Sprite sprite, string name, GameObject parent, bool active)
        //{
        //    if (!parent) return null;

        //    var sub = Instantiate<GameObject>(prefab);
        //    if (sub)
        //    {
        //        sub.transform.SetParent(parent.transform, false);
        //        GameObjectAgent.Instance.addComponent<Image>(sub.name).sprite = sprite;
        //        GameObjectAgent.Instance.getComponent<Image>(sub.name).SetNativeSize();
        //        GameObjectAgent.Instance.addComponent<RectTransform>(sub.name).anchoredPosition = anchoredPosition;
        //        GameObjectAgent.Instance.setActive(sub, active);
        //        sub.transform.name = name;
        //    }

        //    return sub;
        //}

        //void internalSetRaycastTarget(GameObject guiObject, bool value)
        //{
        //    if (!guiObject || !guiObject.GetComponent<Image>()) return;

        //    guiObject.GetComponent<Image>().raycastTarget = value;
        //}

        #endregion private method
    }   //class GUIObject

}   //namespace GUI